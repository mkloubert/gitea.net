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

using Gitea.API.v1.Users;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Gitea.API.v1.Repositories
{
    /// <summary>
    /// A repository.
    /// </summary>
    [DataContract]
    public class Repository : JsonEntityBase
    {
        protected RepositoryPermissions _permissions;


        public Repository()
        {
            Releases = new ReleasesEndpoint(this);
        }

        /// <summary>
        /// clone_url
        /// </summary>
        [DataMember]
        [JsonProperty("clone_url")]
        public string CloneUrl { get; set; }

        /// <summary>
        /// created_at
        /// </summary>
        [DataMember]
        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// default_branch
        /// </summary>
        [DataMember]
        [JsonProperty("default_branch")]
        public string DefaultBranch { get; set; }

        /// <summary>
        /// description
        /// </summary>
        [DataMember]
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// forks_count
        /// </summary>
        [DataMember]
        [JsonProperty("forks_count")]
        public long Forks { get; set; }

        /// <summary>
        /// full_name
        /// </summary>
        [DataMember]
        [JsonProperty("full_name")]
        public string FullName { get; set; }

        /// <summary>
        /// html_url
        /// </summary>
        [DataMember]
        [JsonProperty("html_url")]
        public string HtmlUrl { get; set; }

        /// <summary>
        /// id
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public long ID { get; set; }

        /// <summary>
        /// empty
        /// </summary>
        [DataMember]
        [JsonProperty("empty")]
        public bool IsEmpty { get; set; }

        /// <summary>
        /// fork
        /// </summary>
        [DataMember]
        [JsonProperty("fork")]
        public bool IsFork { get; set; }

        /// <summary>
        /// mirror
        /// </summary>
        [DataMember]
        [JsonProperty("mirror")]
        public bool IsMirror { get; set; }

        /// <summary>
        /// private
        /// </summary>
        [DataMember]
        [JsonProperty("private")]
        public bool IsPrivate { get; set; }

        /// <summary>
        /// open_issues_count
        /// </summary>
        [DataMember]
        [JsonProperty("open_issues_count")]
        public long OpenIssuesCount { get; set; }

        /// <summary>
        /// name
        /// </summary>
        [DataMember]
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// owner
        /// </summary>
        [DataMember]
        [JsonProperty("owner")]
        public User Owner { get; set; }

        /// <summary>
        /// parent
        /// </summary>
        // [DataMember]
        // [JsonProperty("parent")]
        // public Repository Parent { get; set; }

        /// <summary>
        /// permissions
        /// </summary>
        /// <exception cref="ArgumentException">Cannot set permissions.</exception>
        [DataMember]
        [JsonProperty("permissions")]
        public RepositoryPermissions Permissions
        {
            get { return _permissions; }

            set
            {
                if (value != null)
                {
                    if (!Equals(value, _permissions))
                    {
                        if (value.Repository == null)
                        {
                            value.Repository = this;
                        }
                        else
                        {
                            throw new ArgumentException(nameof(Permissions));
                        }
                    }
                }

                _permissions = value;
            }
        }

        /// <summary>
        /// size
        /// </summary>
        [DataMember]
        [JsonProperty("size")]
        public long Size { get; set; }

        /// <summary>
        /// stars_count
        /// </summary>
        [DataMember]
        [JsonProperty("stars_count")]
        public long StarsCount { get; set; }

        /// <summary>
        /// ssh_url
        /// </summary>
        [DataMember]
        [JsonProperty("ssh_url")]
        public string SSHUrl { get; set; }

        /// <summary>
        /// updated_at
        /// </summary>
        [DataMember]
        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        /// <summary>
        /// watchers_count
        /// </summary>
        [DataMember]
        [JsonProperty("watchers_count")]
        public long WatchersCount { get; set; }

        /// <summary>
        /// website
        /// </summary>
        [DataMember]
        [JsonProperty("website")]
        public string Website { get; set; }
        
        
        /// <summary>
        /// Gets the underlying endpoint of repositories of that user.
        /// </summary>
        public ReleasesEndpoint Releases { get; internal protected set; }
    }
}