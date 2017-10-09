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
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Gitea.API.v1.Users
{
    /// <summary>
    /// A builder for creating an user.
    /// </summary>
    public class UserBuilder : IDisposable
    {
        /// <summary>
        /// Stores the properties for the request.
        /// </summary>
        protected IDictionary<string, object> _properties = new Dictionary<string, object>();

        internal UserBuilder(UsersEndpoint endpoint)
        {
            Endpoint = endpoint;

            _properties["source_id"] = 0;
            _properties["send_notify"] = false;
        }

        public async Task<User> Create()
        {
            using (var rest = Endpoint.Client.CreateBaseClient())
            {
                var resp = await rest.PostAsync(
                    "admin/users",
                    new StringContent(ToJson(), Encoding.UTF8, "application/json")
                );

                Exception exception = null;

                if (resp.StatusCode != HttpStatusCode.Created)
                {
                    switch ((int)resp.StatusCode)
                    {
                        case 403:
                        case 422:
                        case 500:
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

                var newUser = JsonConvert.DeserializeObject<User>
                    (
                        await resp.Content.ReadAsStringAsync()
                    );

                if (newUser != null)
                {
                    newUser.Endpoint = Endpoint;
                }

                return newUser;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _properties?.Clear();
            _properties = null;
        }

        /// <summary>
        /// Sets the email address.
        /// </summary>
        /// <param name="full_name">The email address.</param>
        /// <returns>That instance.</returns>
        public UserBuilder Email(string email)
        {
            _properties["email"] = email;

            return this;
        }

        /// <summary>
        /// Gets the underlying endpoint.
        /// </summary>
        public UsersEndpoint Endpoint { get; internal protected set; }

        /// <summary>
        /// Sets the full name.
        /// </summary>
        /// <param name="full_name">The full name.</param>
        /// <returns>That instance.</returns>
        public UserBuilder FullName(string full_name)
        {
            _properties["full_name"] = full_name;

            return this;
        }

        /// <summary>
        /// Sets the login name.
        /// </summary>
        /// <param name="login_name">The login name.</param>
        /// <returns>That instance.</returns>
        public UserBuilder LoginName(string login_name)
        {
            _properties["login_name"] = login_name;

            return this;
        }

        /// <summary>
        /// Sets the password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>That instance.</returns>
        public UserBuilder Password(string password)
        {
            _properties["password"] = password;

            return this;
        }

        /// <summary>
        /// Defines if email notification should be send or not.
        /// </summary>
        /// <param name="send_notify">Send email notification or not.</param>
        /// <returns>That instance.</returns>
        public UserBuilder SendNotification(bool send_notify = true)
        {
            _properties["send_notify"] = send_notify;

            return this;
        }

        /// <summary>
        /// Sets the source ID.
        /// </summary>
        /// <param name="source_id">The ID.</param>
        /// <returns>That instance.</returns>
        public UserBuilder SourceID(int source_id)
        {
            _properties["source_id"] = source_id;

            return this;
        }

        /// <summary>
        /// Sets the user name.
        /// </summary>
        /// <param name="username">The user name.</param>
        /// <returns>That instance.</returns>
        public UserBuilder UserName(string username)
        {
            _properties["username"] = username;

            return this;
        }

        /// <summary>
        /// Converts the current data to a JSON.
        /// </summary>
        /// <returns>The JSON.</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(_properties);
        }
    }
}