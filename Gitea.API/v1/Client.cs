// The MIT License (MIT)
//
// gitea.net (https://github.com/mkloubert/gitea.net)
// Copyright (c) Marcel Joachim Kloubert <marcel.kloubert@gmx.net>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Gitea.API.v1.Users;
using Newtonsoft.Json;

namespace Gitea.API.v1 {
    /// <summary>
    /// An API client for version 1.
    /// </summary>
    public class Client : System.IDisposable {
        /// <summary>
        /// The default hostname.
        /// </summary>
        public const string DEFAULT_HOST = "localhost";

        /// <summary>
        /// The default TCP port.
        /// </summary>
        public const int DEFAULT_PORT = 3000;

        /// <summary>
        /// Initializes a new instance of that class.
        /// </summary>
        /// <param name="authorizer">The authorizer to use.</param>
        /// <param name="host">The hostname.</param>
        /// <param name="port">The TCP port.</param>
        /// <param name="isSecure">User secure HTTP or not.</param>
        /// <exception cref="ArgumentNullException"><paramref name="authorizer" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="port" /> is invalid.</exception>
        public Client(IAuthorizer authorizer,
                      string host = DEFAULT_HOST, int port = DEFAULT_PORT, bool isSecure = false) {
            if (null == authorizer) {
                throw new ArgumentNullException(nameof(authorizer));
            }

            Authorizer = authorizer;

            SetupBaseUrl(host, port, isSecure);
            SetupEndpoints();
        }

        /// <summary>
        /// Gets the authorizer.
        /// </summary>
        public IAuthorizer Authorizer {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the base URL.
        /// </summary>
        public Uri BaseUrl {
            get;
            protected set;
        }

        /// <summary>
        /// Creates a basic HTTP client instance.
        /// </summary>
        /// <returns>The new instance.</returns>
        public HttpClient CreateBaseClient() {
            var msgHandlerFactory = MessageHandlerFactory;

            HttpMessageHandler msgHandler = null;
            if (msgHandlerFactory != null)
            {
                msgHandler = msgHandlerFactory(this);
            }

            HttpClient newClient;
            if (msgHandler == null)
            {
                newClient = new HttpClient();
            }
            else
            {
                newClient = new HttpClient(msgHandler);  // use custom handler
            }

            newClient.BaseAddress = BaseUrl;
            newClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Authorizer.PrepareClient(newClient);

            return newClient;
        }

        /// <inheritdoc />
        public void Dispose() {
            BaseUrl = null;
            Authorizer = null;
        }

        /// <summary>
        /// Gets the Gitea version.
        /// </summary>
        /// <returns>The version.</returns>
        public async Task<GiteaVersion> GetVersionAsync() {
            using (var rest = CreateBaseClient()) {
                var resp = await rest.GetAsync("version");

                var version = JsonConvert.DeserializeObject<GiteaVersion>
                    (
                        await resp.Content.ReadAsStringAsync()
                    );
                version.Client = this;

                return version;
            }
        }

        /// <summary>
        /// Gets or sets the factory for HTTP message handler.
        /// </summary>
        public Func<Client, HttpMessageHandler> MessageHandlerFactory { get; set; }

        /// <summary>
        /// Sets up the base URL.
        /// </summary>
        /// <param name="host">The hostname.</param>
        /// <param name="port">The TCP port.</param>
        /// <param name="isSecure">User secure HTTP or not.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="port" /> is invalid.</exception>
        protected virtual void SetupBaseUrl(string host, int port, bool isSecure) {
            if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort) {
                throw new ArgumentOutOfRangeException(nameof(port));
            }

            if (string.IsNullOrWhiteSpace(host)) {
                host = DEFAULT_HOST;
            }

            BaseUrl = new Uri(string.Format("http{0}://{1}:{2}/api/v1/",
                                            isSecure ? "s" : "",
                                            host, port));
        }

        /// <summary>
        /// Sets up the endpoints.
        /// </summary>
        protected virtual void SetupEndpoints() {
            Users = new UserEndpoint(this);
        }

        /// <summary>
        /// Gets the endpoint for accessing user resources.
        /// </summary>
        public UserEndpoint Users { get; protected set; }
    }
}
