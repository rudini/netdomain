//-------------------------------------------------------------------------------
// <copyright file="Adresse.cs" company="bbv Software Services AG">
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

    public partial class AdressePoco
    {
        /// <summary>
        /// Validates the specified results.
        /// </summary>
        /// <param name="result">The validation result.</param>
        public void ValidateRegardingPersistence(ValidationResult result)
        {
            var validationResults = new InlineValidator<AdressePoco>
                          {
                              v => v.RuleFor(e => e.Name).Length(1, 20).WithMessage("The length of the field Name must fall within the range '1'-'20').").
                                       WithState(e => SeverityLevel.Error),
                          }.Validate(this);

            foreach (var validationResult in validationResults.Errors)
            {
                result.Errors.Add(validationResult);
            }
        }
    }
}