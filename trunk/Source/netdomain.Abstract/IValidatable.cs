//-------------------------------------------------------------------------------
// <copyright file="IValidatable.cs" company="bbv Software Services AG">
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
    /// Defines an interface to enable object validation of an entity or a DTO
    /// </summary>
    public interface IValidatable
    {
    }

    /// <summary>
    /// Defines a typed interface to enable object validation of an entity or a DTO
    /// by using a particular validation library.
    /// </summary>
    /// <typeparam name="TValidationResult">The type of the validation results.</typeparam>
    public interface IValidatable<TValidationResult> : IValidatable
    {
        /// <summary>
        /// Validates this instance and all aggregated entities and returns the validation results.
        /// </summary>
        /// <param name="result">The validation results as <see cref="T:System.Collections.Generic.IEnumerable`1"/>.</param>
        void Validate(TValidationResult result);

        /// <summary>
        /// Validates this instance and all aggregated entities regarding persistence validation rules and returns the validation results.
        /// </summary>
        /// <param name="result">The validation results as <see cref="T:System.Collections.Generic.IEnumerable`1"/>.</param>
        void ValidateRegardingPersistence(TValidationResult result);
    }
}
