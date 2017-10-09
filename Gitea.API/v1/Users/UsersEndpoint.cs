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

using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Gitea.API.v1.Users
{
    /// <summary>
    /// An endpoint of users.
    /// </summary>
    public class UsersEndpoint : EndpointBase
    {
        internal UsersEndpoint(Client client)
            : base(client)
        { }

        /// <summary>
        /// Creates an user object from a HTTP response.
        /// </summary>
        /// <param name="resp">The response.</param>
        /// <returns>The created object.</returns>
        protected virtual async Task<User> CreateUserObject(HttpResponseMessage resp)
        {
            User user = null;

            if (resp != null)
            {
                Exception exception = null;

                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    switch (resp.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                            exception = new ApiException(null,
                                                         (int)resp.StatusCode, resp.ReasonPhrase);
                            break;

                        case HttpStatusCode.InternalServerError:
                            exception = new ApiException(JsonConvert.DeserializeObject<ApiError>
                                (
                                    await resp.Content.ReadAsStringAsync()
                                ),
                                (int)resp.StatusCode, resp.ReasonPhrase);
                            break;

                        default:
                            exception = new UnexpectedResponseException((int)resp.StatusCode,
                                                                        resp.ReasonPhrase);
                            break;
                    }
                }

                if (exception != null)
                {
                    throw exception;
                }

                user = JsonConvert.DeserializeObject<User>
                    (
                        await resp.Content.ReadAsStringAsync()
                    );
            }

            SetupUser(user);

            return user;
        }

        /// <summary>
        /// Returns the current user.
        /// </summary>
        /// <returns>The current user.</returns>
        public async Task<User> GetCurrent()
        {
            using (var rest = Client.CreateBaseClient())
            {
                var resp = await rest.GetAsync("user");

                return await CreateUserObject(resp);
            }
        }

        /// <summary>
        /// Returns a user by name.
        /// </summary>
        /// <param name="username">The user name.</param>
        /// <returns>The user.</returns>
        public async Task<User> GetByUsername(string username)
        {
            using (var rest = Client.CreateBaseClient())
            {
                var resp = await rest.GetAsync("users/" + HttpUtility.UrlEncode(username));

                return await CreateUserObject(resp);
            }
        }

        /// <summary>
        /// Starts creating a new user.
        /// </summary>
        /// <returns>The builder.</returns>
        public UserBuilder New()
        {
            return new UserBuilder(this);
        }

        /// <summary>
        /// Sets up an user object for that endpoint.
        /// </summary>
        /// <param name="user">The user object.</param>
        /// <exception cref="ArgumentException">Setup failed.</exception>
        public void SetupUser(User user)
        {
            if (user != null)
            {
                if (!Equals(user.Endpoint, this))
                {
                    if (user.Endpoint == null)
                    {
                        user.Endpoint = this;
                    }
                    else
                    {
                        throw new ArgumentException(nameof(user));
                    }
                }
            }
        }
    }
}