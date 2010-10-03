//-------------------------------------------------------------------------------
// <copyright file="Person.cs" company="bbv Software Services AG">
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

namespace netdomain.LinqToEntities.Test.BusinessObjects
{
    using Abstract;
    using FluentValidation;
    using FluentValidation.Results;

    public partial class PersonPoco : IValidatable<ValidationResult>
    {
        /// <summary>
        /// Validates this instance and all aggregated entities and returns the validation results.
        /// </summary>
        /// <param name="results">The validation results as <see cref="T:System.Collections.Generic.IEnumerable`1"/>.</param>
        public void Validate(ValidationResult results)
        {
            // context based validation
        }

        /// <summary>
        /// Validates this instance and all aggregated entities regarding persistence validation rules and returns the validation results.
        /// </summary>
        /// <param name="result">The validation results as <see cref="T:System.Collections.Generic.IEnumerable`1"/>.</param>
        public void ValidateRegardingPersistence(ValidationResult result)
        {
            var validationResults = new InlineValidator<PersonPoco>
                          {
                              v => v.RuleFor(e => e.Name).Length(1, 10).WithMessage("The length of the field Name must fall within the range '1'-'20').").
                                       WithState(e => SeverityLevel.Error),
                              v => v.RuleFor(e => e.Beruf).Length(1, 20).WithMessage("The length of the field Beruf must fall within the range '1'-'20').").
                                       WithState(e => SeverityLevel.Error)
                          }.Validate(this);


            foreach (var validationResult in validationResults.Errors)
            {
                result.Errors.Add(validationResult);
            }

            foreach (var address in this.Adressliste)
            {
                address.ValidateRegardingPersistence(result);
            }
        }
    }
}