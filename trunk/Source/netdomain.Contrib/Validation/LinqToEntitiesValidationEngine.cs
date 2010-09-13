//-------------------------------------------------------------------------------
// <copyright file="LinqToEntitiesValidationEngine.cs" company="bbv Software Services AG">
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
    using System.Data.Objects;
    using System.Linq;
    using Microsoft.Practices.EnterpriseLibrary.Validation;
    using netdomain.Abstract;

    /// <summary>
    /// Validation Engine to validate entities by using the Validation Application Block 
    /// and the Metadata of the enitites in the <see cref="T:System.Data.Objects.ObjectContext"/>.
    /// </summary>
    public class LinqToEntitiesValidationEngine : IValidationEngine<ValidationResults>
    {
        /// <summary>
        /// Validator map caches the validators by its type.
        /// </summary>
        private readonly Dictionary<Type, Validator> validatorMap;

        /// <summary>
        /// Metadata validation helper.
        /// </summary>
        private readonly MetadataValidationHelper metadataValidationHelper;

        /// <summary>
        /// The object context.
        /// </summary>
        private readonly ObjectContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinqToEntitiesValidationEngine"/> class.
        /// </summary>
        /// <param name="helper">The validation helper.</param>
        /// <param name="context">The context <see cref="T:System.Data.Objects.ObjectContext"/>.</param>
        public LinqToEntitiesValidationEngine(MetadataValidationHelper helper, ObjectContext context)
        {
            this.context = context;
            this.metadataValidationHelper = helper;
            this.validatorMap = new Dictionary<Type, Validator>();
        }

        /// <summary>
        /// Validates the specified entity against a given ruleset and against the metadata properties of the <see cref="T:System.Data.Objects.ObjectContext"/>.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity to validate.</param>
        /// <param name="results">The validation results.</param>
        /// <param name="rulesets">The ruleset to validate.</param>
        public void Validate<T>(T entity, ValidationResults results, string[] rulesets) where T : class
        {
            // persistent rule
            if (rulesets.Contains(Rulesets.PersistentRelatedRules))
            {
                Validator validator;

                if (!this.validatorMap.TryGetValue(typeof(T), out validator))
                {
                    validator = this.metadataValidationHelper.CreateValidator<T>(this.context);
                    this.validatorMap.Add(typeof(T), validator);
                }

                results.AddAllResults(validator.Validate(entity));
            }

            // specific rules
            // results.AddAllResults(ValidationFactory.CreateValidator<T>(ruleset).Validate(entity));
            results.AddAllResults(Validation.Validate(entity, rulesets));
        }
    }
}