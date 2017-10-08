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
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web;

namespace Gitea.API.v1.Users
{
    /// <summary>
    /// An user.
    /// </summary>
    [DataContract]
    public class User : JsonEntityBase
    {
        /// <summary>
        /// Initializes a new instance of that class.
        /// </summary>
        public User()
        {
            Repositories = new RepositoriesEndpoint(this);
        }

        /// <summary>
        /// avatar_url
        /// </summary>
        [DataMember]
        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }

        /// <summary>
        /// email
        /// </summary>
        [DataMember]
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets the underlying endpoint.
        /// </summary>
        public UsersEndpoint Endpoint { get; internal set; }

        /// <summary>
        /// full_name
        /// </summary>
        [DataMember]
        [JsonProperty("full_name")]
        public string FullName { get; set; }

        /// <summary>
        /// id
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public long ID { get; set; }

        /// <summary>
        /// login
        /// </summary>
        [DataMember]
        [JsonProperty("login")]
        public string Login { get; set; }

        /// <summary>
        /// Gets the underlying endpoint of repositories of that user.
        /// </summary>
        public RepositoriesEndpoint Repositories { get; internal protected set; }

        /// <summary>
        /// username
        /// </summary>
        [DataMember]
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// Returns a list of all followers.
        /// </summary>
        /// <returns>The list of followers.</returns>
        public async Task<List<User>> GetFollowers()
        {
            return await GetFollowers<List<User>>();
        }

        /// <summary>
        /// Returns a collection of all followers.
        /// </summary>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <returns>The collection of followers.</returns>
        public async Task<TCollection> GetFollowers<TCollection>()
            where TCollection : global::System.Collections.Generic.ICollection<User>, new()
        {
            using (var rest = Endpoint.Client.CreateBaseClient())
            {
                var resp = await rest.GetAsync("users/" + HttpUtility.UrlEncode(Username) + "/followers");

                var users = JsonConvert.DeserializeObject<IEnumerable<User>>
                    (
                        await resp.Content.ReadAsStringAsync()
                    );

                var followers = new TCollection();
                using (var e = users.GetEnumerator())
                {
                    while (e.MoveNext())
                    {
                        followers.Add(e.Current);
                    }
                }

                return followers;
            }
        }
    }
}