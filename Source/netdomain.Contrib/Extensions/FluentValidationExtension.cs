//-------------------------------------------------------------------------------
// <copyright file="FluentValidationExtension.cs" company="bbv Software Services AG">
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

namespace netdomain.Contrib.Extensions
{
    using System.Linq;
    using System.Collections.Generic;
    using Abstract;
    using FluentValidation.Results;

    /// <summary>
    /// Implements a workspace extension for validation purposes using fluent validation.
    /// </summary>
    public class FluentValidationExtension : WorkspaceExtension
    {
        /// <summary>
        /// Called before all changes have been submitted.
        /// </summary>
        /// <param name="deletedEntities">The deleted entities.</param>
        /// <param name="addedEntities">The added entities.</param>
        /// <param name="modifiedEntities">The modified entities.</param>
        public override void OnSubmittingChanges(IEnumerable<object> deletedEntities, IEnumerable<object> addedEntities, IEnumerable<object> modifiedEntities)
        {
            var entities = new List<object>();
            entities.AddRange(addedEntities);
            entities.AddRange(modifiedEntities);
            var validatableEntities = entities.Where(entity => typeof(IValidatable).IsAssignableFrom(entity.GetType()));
            var validationResults = new ValidationResult();

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