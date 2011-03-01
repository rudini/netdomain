//-------------------------------------------------------------------------------
// <copyright file="Address.cs" company="bbv Software Services AG">
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

namespace netdomain.LinqToObjects.Test.BusinessObjects
{
    using System;
    using System.Collections.Generic;

    using FluentValidation;
    using FluentValidation.Results;
    using netdomain.Abstract;

    /// <summary>
    /// Address for a person
    /// </summary>
    [Serializable]
    public class Adresse 
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The address id.</value>
        public virtual int Id { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version field.</value>
        public virtual DateTime? Version { get; set; }

        /// <summary>
        /// Gets or sets the person ID.
        /// </summary>
        /// <value>The person ID.</value>
        public virtual Person Person { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The address name.</value>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the adressdetails.
        /// </summary>
        /// <value>The adressdetails.</value>
        public virtual IList<AdresseDetail> AdresseDetails { get; set; }

        /// <summary>
        /// Validates the specified results.
        /// </summary>
        /// <param name="result">The results.</param>
        public virtual void ValidateRegardingPersistence(ValidationResult result)
        {
            var validationResults = new InlineValidator<Adresse>
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