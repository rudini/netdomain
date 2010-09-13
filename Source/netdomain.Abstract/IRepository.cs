//-------------------------------------------------------------------------------
// <copyright file="IRepository.cs" company="bbv Software Services AG">
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

namespace netdomain.Abstract
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines an interface of a repository based on the DDD pattern by Eric Evans.
    /// </summary>
    /// <typeparam name="T">The type of the repository.</typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// Finds entities by a given specification.
        /// </summary>
        /// <param name="specification">The specification of type <see cref="T:netdomain.Abstract.ISpecification`1"/></param>
        /// <returns>A new <see cref="T:System.Collections.Generic.IEnumerable`1"/> with the found entities of the type T.</returns>
        IEnumerable<T> FindBySpecification(ISpecification<T> specification);
    }
}
