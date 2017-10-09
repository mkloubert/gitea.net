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
using System.Runtime.Serialization;

namespace Gitea.API.v1.Repositories
{
    /// <summary>
    /// Permissions of a repository.
    /// </summary>
    [DataContract]
    public class RepositoryPermissions : JsonEntityBase
    {
        /// <summary>
        /// admin
        /// </summary>
        [DataMember]
        [JsonProperty("admin")]
        public bool CanAdministrate { get; set; }

        /// <summary>
        /// pull
        /// </summary>
        [DataMember]
        [JsonProperty("pull")]
        public bool CanPull { get; set; }

        /// <summary>
        /// push
        /// </summary>
        [DataMember]
        [JsonProperty("push")]
        public bool CanPush { get; set; }

        /// <summary>
        /// Gets the underlying repository.
        /// </summary>
        public Repository Repository { get; internal set; }
    }
}