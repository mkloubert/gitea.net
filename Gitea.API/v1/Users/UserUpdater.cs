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
using System.Web;

namespace Gitea.API.v1.Users
{
    /// <summary>
    /// A builder for updating an user.
    /// </summary>
    public class UserUpdater : IDisposable
    {
        /// <summary>
        /// Stores the properties for the request.
        /// </summary>
        protected IDictionary<string, object> _properties = new Dictionary<string, object>();

        internal UserUpdater(User user)
        {
            User = user;
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
        public UserUpdater Email(string email)
        {
            _properties["email"] = email;

            return this;
        }

        /// <summary>
        /// Sets the full name.
        /// </summary>
        /// <param name="full_name">The full name.</param>
        /// <returns>That instance.</returns>
        public UserUpdater FullName(string full_name)
        {
            _properties["full_name"] = full_name;

            return this;
        }

        /// <summary>
        /// Sets if the user is active or not.
        /// </summary>
        /// <param name="active">Is active or not.</param>
        /// <returns>That instance.</returns>
        public UserUpdater IsActive(bool active = true)
        {
            _properties["active"] = active;

            return this;
        }

        /// <summary>
        /// Sets the location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>That instance.</returns>
        public UserUpdater Location(string location)
        {
            _properties["location"] = location;

            return this;
        }

        /// <summary>
        /// Sets the login name.
        /// </summary>
        /// <param name="login_name">The login name.</param>
        /// <returns>That instance.</returns>
        public UserUpdater LoginName(string login_name)
        {
            _properties["login_name"] = login_name;

            return this;
        }

        /// <summary>
        /// Sets if the user should become an admin or not.
        /// </summary>
        /// <param name="admin">Should become an admin or not.</param>
        /// <returns>That instance.</returns>
        public UserUpdater MakeAdmin(bool admin = true)
        {
            _properties["admin"] = admin;

            return this;
        }

        /// <summary>
        /// Sets the number of repositories that user is able to create.
        /// </summary>
        /// <param name="max_repo_creation">The maximum number of repositories to create.</param>
        /// <returns>That instance.</returns>
        public UserUpdater MaximumRepositoryCreations(long max_repo_creation)
        {
            _properties["max_repo_creation"] = max_repo_creation;

            return this;
        }

        /// <summary>
        /// Sets no repository creation limit.
        /// </summary>
        /// <returns>That instance.</returns>
        public UserUpdater NoRepositoryCreationLimit()
        {
            return MaximumRepositoryCreations(-1);
        }

        /// <summary>
        /// Sets the password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>That instance.</returns>
        public UserUpdater Password(string password)
        {
            _properties["password"] = password;

            return this;
        }


        public async Task<User> Save()
        {
            using (var rest = User.Endpoint.Client.CreateBaseClient())
            {
                var req = new HttpRequestMessage
                    (
                        new HttpMethod("PATCH"),
                        "admin/users/" + HttpUtility.UrlEncode(User.Username)
                    );
                req.Content = new StringContent(ToJson(), Encoding.UTF8, "application/json");

                var resp = await rest.SendAsync(req);

                Exception exception = null;

                if (resp.StatusCode != HttpStatusCode.OK)
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

                var updatedUser = JsonConvert.DeserializeObject<User>
                    (
                        await resp.Content.ReadAsStringAsync()
                    );

                User.Endpoint.SetupUser(updatedUser);

                return updatedUser;
            }
        }

        /// <summary>
        /// Sets the source ID.
        /// </summary>
        /// <param name="source_id">The ID.</param>
        /// <returns>That instance.</returns>
        public UserUpdater SourceID(int source_id)
        {
            _properties["source_id"] = source_id;

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

        /// <summary>
        /// Gets the underlying user.
        /// </summary>
        public User User { get; internal protected set; }

        /// <summary>
        /// Sets the website.
        /// </summary>
        /// <param name="website">The website.</param>
        /// <returns>That instance.</returns>
        public UserUpdater Website(string website)
        {
            _properties["website"] = website;

            return this;
        }
    }
}