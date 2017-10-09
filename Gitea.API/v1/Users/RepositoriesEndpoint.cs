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

using Gitea.API.v1.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Gitea.API.v1.Users
{
    /// <summary>
    /// An endpoint for an user repository.
    /// </summary>
    public class RepositoriesEndpoint
    {
        internal RepositoriesEndpoint(User user)
        {
            User = user;
        }

        /// <summary>
        /// Returns a list of all repositories of the underlying user.
        /// </summary>
        /// <returns>The list of repositories.</returns>
        public async Task<List<Repository>> GetAll()
        {
            return await GetAll<List<Repository>>();
        }

        /// <summary>
        /// Returns a collection of all repositories of the underlying user.
        /// </summary>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <returns>The collection of repositories.</returns>
        public async Task<TCollection> GetAll<TCollection>()
            where TCollection : global::System.Collections.Generic.ICollection<Repository>, new()
        {
            using (var rest = User.Endpoint.Client.CreateBaseClient())
            {
                var resp = await rest.GetAsync("users/" + HttpUtility.UrlEncode(User.Username) + "/repos");

                Exception exception = null;

                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    switch (resp.StatusCode)
                    {
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

                var json = await resp.Content.ReadAsStringAsync();

                var repoList = JsonConvert.DeserializeObject<IEnumerable<Repository>>
                    (
                        await resp.Content.ReadAsStringAsync()
                    );

                var userRepos = new TCollection();
                using (var e = repoList.GetEnumerator())
                {
                    while (e.MoveNext())
                    {
                        var r = e.Current;
                        r.Owner.Endpoint = User.Endpoint;

                        userRepos.Add(r);
                    }
                }

                return userRepos;
            }
        }

        /// <summary>
        /// Starts migrating an external repository.
        /// </summary>
        /// <returns>The builder.</returns>
        public MigrationBuilder Migrate()
        {
            return new MigrationBuilder(User);
        }

        /// <summary>
        /// Gets the underlying user.
        /// </summary>
        public User User { get; internal protected set; }
    }
}