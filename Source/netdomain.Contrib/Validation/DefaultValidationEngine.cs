//-------------------------------------------------------------------------------
// <copyright file="DefaultValidationEngine.cs" company="bbv Software Services AG">
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
    using Microsoft.Practices.EnterpriseLibrary.Validation;
    using netdomain.Abstract;

    /// <summary>
    /// Validation Engine to validate entities by using the Validation Application Block.
    /// </summary>
    public class DefaultValidationEngine : IValidationEngine<ValidationResults>
    {
        /// <summary>
        /// Validates the specified entity against a given ruleset.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity to validate.</param>
        /// <param name="results">The validation results.</param>
        /// <param name="rulesets">The ruleset to validate.</param>
        public void Validate<T>(T entity, ValidationResults results, string[] rulesets) where T : class
        {
            results.AddAllResults(Validation.Validate(entity, rulesets));
        }
    }
}
