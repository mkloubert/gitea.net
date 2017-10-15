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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Gitea.API.Extensions;
using Newtonsoft.Json;

namespace Gitea.API.v1.Users {
    /// <summary>
    /// Endpoint for 'user' resources.
    /// </summary>
    public class UserEndpoint {
        internal UserEndpoint(Client client) {
            Client = client;
        }

        /// <summary>
        /// Gets the underlying client.
        /// </summary>
        /// <returns></returns>
        public Client Client { get; internal protected set; }

        /// <summary>
        /// Returns the current user.
        /// </summary>
        /// <returns>The current user.</returns>
        public async Task<User> GetCurrentAsync() {
            using (var rest = Client.CreateBaseClient()) {
                var resp = await rest.GetAsync("user");

                Exception ex = null;

                switch ((int)resp.StatusCode) {
                    case 200:
                        break;

                    default:
                        ex = await UnexpectedResponseException.FromResponseAsync(resp);
                        break;
                }

                if (ex != null) {
                    throw ex;
                }

                var user = await resp.Content.DeserializeAsync<User>();
                Client.SetupForMe(new [] { user });

                return user;
            }
        }

        /// <summary>
        /// Searches for users.
        /// </summary>
        /// <param name="q">The search query.</param>
        /// <returns>The list of matching users.</returns>
        public async Task<List<User>> SearchAsync(string q) {
            return await SearchAsync<List<User>>(q);
        }

        /// <summary>
        /// Searches for users.
        /// </summary>
        /// <typeparam name="TCollection">Type of collection to (create and) return.</typeparam>
        /// <param name="q">The search query.</param>
        /// <returns>The collection of matching users.</returns>
        public async Task<TCollection> SearchAsync<TCollection>(string q)
            where TCollection : global::System.Collections.Generic.ICollection<User>, new()
        {
            using (var rest = Client.CreateBaseClient()) {
                var resp = await rest.GetAsync("users/search?q=" + HttpUtility.UrlEncode(q));

                Exception ex = null;

                switch ((int)resp.StatusCode) {
                    case 200:
                        break;

                    case 500:
                        ex = await ApiException.FromResponseAsync(resp);
                        break;

                    default:
                        ex = await UnexpectedResponseException.FromResponseAsync(resp);
                        break;
                }

                if (ex != null) {
                    throw ex;
                }

                var answer = await resp.Content.DeserializeAsync<OKResponse<List<User>>>();

                IEnumerable<User> foundUsers = null;
                if (answer != null) {
                    foundUsers = answer.Data;
                }

                if (foundUsers is TCollection) {
                    return (TCollection)foundUsers;
                }
                
                var users = default(TCollection);

                if (foundUsers != null) {
                    users = new TCollection();
                    using (var e = foundUsers.GetEnumerator()) {
                        while (e.MoveNext()) {
                            users.Add(e.Current);
                        }
                    }
                }

                Client.SetupForMe(users);

                return users;
            }
        }
    }
}
