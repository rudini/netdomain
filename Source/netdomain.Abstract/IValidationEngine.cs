//-------------------------------------------------------------------------------
// <copyright file="IValidationEngine.cs" company="bbv Software Services AG">
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
    /// <summary>
    /// Defines an interface of a Validation Engine to validate entities.
    /// </summary>
    /// <typeparam name="TResults">The type of the results.</typeparam>
    public interface IValidationEngine<TResults>
    {
        /// <summary>
        /// Validates the specified entity against a given ruleset and against the metadata properties of the <see cref="T:System.Data.Objects.ObjectContext"/>.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity to validate.</param>
        /// <param name="results">The validation results.</param>
        /// <param name="ruleset">The ruleset to validate.</param>
        void Validate<T>(T entity, TResults results, string[] ruleset) where T : class;
    }
}