//-------------------------------------------------------------------------------
// <copyright file="IIdentifierGenerator.cs" company="bbv Software Services AG">
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
    using Abstract;

    /// <summary>
    /// Defines an interface of a identifier generator.
    /// </summary>
    public interface IIdentifierGenerator
    {
        /// <summary>
        /// Generates a new identifier of a particular object.
        /// </summary>
        /// <typeparam name="T">The type of the entity the identifier is being generated.</typeparam>
        /// <param name="context">The <see cref="IWorkspace"/> this id is being generated in.</param>
        /// <param name="obj">The entity for which the id is being generated.</param>
        /// <returns>The new identifier</returns>
        object Generate<T>(IWorkspace context, T obj);
    }
}