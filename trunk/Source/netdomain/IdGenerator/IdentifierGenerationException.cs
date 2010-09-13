//-------------------------------------------------------------------------------
// <copyright file="IdentifierGenerationException.cs" company="bbv Software Services AG">
//   Copyright (c) 2010 Roger Rudin, bbv Software Services AG
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//------------------------------------------------------------------------------

namespace netdomain.IdGenerator
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Thrown by <see cref="IIdentifierGenerator" /> implementation class if ID generation fails.
    /// </summary>
    [Serializable]
    public class IdentifierGenerationException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifierGenerationException"/> class.
        /// </summary>
        public IdentifierGenerationException() : base("An exception occurred during ID generation.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifierGenerationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public IdentifierGenerationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifierGenerationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="e">
        /// The exception that is the cause of the current exception. If the innerException parameter 
        /// is not a null reference, the current exception is raised in a catch block that handles 
        /// the inner exception.
        /// </param>
        public IdentifierGenerationException(string message, Exception e) : base(message, e)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifierGenerationException"/> class
        /// with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object 
        /// data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        protected IdentifierGenerationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}