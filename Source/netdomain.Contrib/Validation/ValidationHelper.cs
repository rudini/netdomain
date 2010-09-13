//-------------------------------------------------------------------------------
// <copyright file="ValidationHelper.cs" company="bbv Software Services AG">
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

namespace netdomain.Contrib.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentValidation.Results;
    using netdomain.Abstract;

    /// <summary>
    /// Implements helper methods for validation purposes.
    /// </summary>
    [Obsolete("Implement a WorkspaceExtension for validation purposes.")]
    public class ValidationHelper : IValidator
    {
        /// <summary>
        /// Validates the entities using Validation Application Block.
        /// </summary>
        /// <param name="entities">The validatable entities.</param>
        /// <exception cref="T:netdomain.Abstract.ValidationException`1"></exception>
        [Obsolete("Implement a WorkspaceExtension for validation purposes.")]
        public void ValidateEntities(IEnumerable<object> entities)
        { 
            var validationResults = new ValidationResult();
            var validatableEntities = entities.Where(entity => typeof(IValidatable).IsAssignableFrom(entity.GetType()));

            foreach (IValidatable<ValidationResult> validatable in validatableEntities)
            {
                validatable.ValidateRegardingPersistence(validationResults);
            }

            var validationErrors = validationResults;
            if (!validationErrors.IsValid)
            {
                throw new ValidationException<ValidationResult>(validationErrors);
            }
        }
    }
}