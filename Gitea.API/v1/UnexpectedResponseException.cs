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
using System.Net.Http;
using System.Threading.Tasks;

namespace Gitea.API.v1 {
    /// <summary>
    /// An exception for an unexpected API response.
    /// </summary>
    public class UnexpectedResponseException : Exception {
        /// <summary>
        /// Initializes a new instance of that class.
        /// </summary>
        /// <param name="code">The HTTP response code.</param>
        /// <param name="status">The HTTP status text.</param>
        public UnexpectedResponseException(int? code = 500, string status = null) {
            Code = code;
            Status = status;
        }

        /// <summary>
        /// Gets the HTTP response code.
        /// </summary>
        public int? Code  { get; protected set; }

        /// <summary>
        /// Creates a new instance from a HTTP response.
        /// </summary>
        /// <param name="resp">The response message.</param>
        /// <returns>The new instance.</returns>
        public static async Task<UnexpectedResponseException> FromResponseAsync(HttpResponseMessage resp) {
            if (resp != null) {
                return new UnexpectedResponseException((int)resp.StatusCode, resp.ReasonPhrase);
            }
            
            return null;
        }

        /// <inheritdoc />
        public override string Message
        {
            get { return string.Format("Unexpected response: [{0}] '{1}'"); }
        }

        /// <summary>
        /// Gets the HTTP status text.
        /// </summary>
        public string Status { get; protected set; }
    }
}
