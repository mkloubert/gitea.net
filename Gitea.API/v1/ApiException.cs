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

namespace Gitea.API.v1
{
    /// <summary>
    /// An API exception.
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// Initializes a new instance of that class.
        /// </summary>
        /// <param name="error">The underlying error.</param>
        /// <param name="code">The HTTP response code.</param>
        /// <param name="status">The HTTP status text.</param>
        public ApiException(ApiError error = null,
                            int? code = 500, string status = null)
        {
            Error = error;

            Code = code;
            Status = status;
        }

        /// <summary>
        /// Gets the HTTP response code.
        /// </summary>
        public int? Code
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the underlying error.
        /// </summary>
        public ApiError Error
        {
            get;
            protected set;
        }

        /// <inheritdoc />
        public override string Message
        {
            get { return Error?.Message; }
        }

        /// <summary>
        /// Gets the HTTP status text.
        /// </summary>
        public string Status
        {
            get;
            protected set;
        }
    }
}