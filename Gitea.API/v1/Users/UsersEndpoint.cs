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
using System.Threading.Tasks;

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
        /// Returns the current user.
        /// </summary>
        /// <returns>The current user.</returns>
        public async Task<User> GetCurrent()
        {
            using (var rest = Client.CreateBaseClient())
            {
                var resp = await rest.GetAsync("user");

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

                var user = JsonConvert.DeserializeObject<User>
                    (
                        await resp.Content.ReadAsStringAsync()
                    );

                if (user != null)
                {
                    user.Endpoint = this;
                }

                return user;
            }
        }
    }
}