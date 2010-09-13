//-------------------------------------------------------------------------------
// <copyright file="MetadataValidationHelper.cs" company="bbv Software Services AG">
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
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Abstract;
    using LinqToEntities;
    using Microsoft.Practices.EnterpriseLibrary.Validation;
    using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

    /// <summary>
    /// Implements helper methods to create validators from the metadata of a persistence framework
    /// using the Validation Application Block.
    /// </summary>
    public class MetadataValidationHelper
    {
        /// <summary>
        /// Regex validator to validate the length value of a db type e.g. varchar(20).
        /// </summary>
        private readonly Regex lengthRegexMatcher = new Regex(@"\w+(\d+)");

        /// <summary>
        /// Creates a validator of a particular type.
        /// </summary>
        /// <typeparam name="T">The type to get the validator for.</typeparam>
        /// <param name="context">The context to get access to the metadata.</param>
        /// <returns>A validator of type <see cref="T:Microsoft.Practices.EnterpriseLibrary.Validation.Validator"/></returns>
        /// <exception cref="NotSupportedException">Thrown, if there is no EntitySet supporting the type 'T'.</exception>
        public Validator CreateValidator<T>(ObjectContext context) where T : class 
        {
            var entityType = context.GetEntityType<T>(DataSpace.CSpace);

            return new AndCompositeValidator(CreatePropertyValueValidator<T>(entityType).ToArray());
        }

        /// <summary>
        /// Creates a validator of a particular type.
        /// </summary>
        /// <typeparam name="T">The type to get the validator for.</typeparam>
        /// <param name="context">The context to get access to the metadata.</param>
        /// <returns>A validator of type <see cref="T:Microsoft.Practices.EnterpriseLibrary.Validation.Validator"/></returns>
        /// <exception cref="NotSupportedException">Thrown, if there is no EntitySet supporting the type 'T'.</exception>
        public Validator CreateValidator<T>(DataContext context) where T : class
        {
            var metaType = context.Mapping.GetMetaType(typeof(T));

            return new AndCompositeValidator(CreatePropertyValueValidator<T>(metaType).ToArray());
        }

        /// <summary>
        /// Creates the property value validator of all facets found in the given entity type.
        /// </summary>
        /// <typeparam name="T">The type to get the property validator for.</typeparam>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>An enumerable over the created property validators.</returns>
        protected virtual IEnumerable<PropertyValueValidator<T>> CreatePropertyValueValidator<T>(EntityType entityType)
        {
            foreach (var property in entityType.Properties)
            {
                // Nullable
                if (
                    !property.TypeUsage.Facets.Where(f => f.Name == "Nullable").Select(f => f.Value).Cast<bool>().
                        SingleOrDefault())
                {
                    yield return new NotNullValidator { Tag = SeverityLevel.Error, MessageTemplate = Resources.NonNullNonNegatedValidatorDefaultMessageTemplate }
                        .PropertyValue<T>(property.Name);
                }

                // StringLength
                EdmProperty edmProperty = property;
                var maxLength =
                    property.TypeUsage.Facets.Where(f => f.Name == "MaxLength" && ((PrimitiveType)edmProperty.TypeUsage.EdmType).PrimitiveTypeKind == PrimitiveTypeKind.String).
                        Select(f => f.Value).Cast<int>().SingleOrDefault();

                if (maxLength > 0)
                {
                    yield return new StringLengthValidator(1, maxLength) { Tag = SeverityLevel.Error, MessageTemplate = Resources.StringLengthValidatorNonNegatedDefaultMessageTemplate }
                        .PropertyValue<T>(property.Name);
                }
            }
        }

        /// <summary>
        /// Creates the property value validator of all facets found in the given entity type.
        /// </summary>
        /// <typeparam name="T">The type to get the property validator for.</typeparam>
        /// <param name="metaType">Type of the entity.</param>
        /// <returns>An enumerable over the created property validators.</returns>
        protected virtual IEnumerable<PropertyValueValidator<T>> CreatePropertyValueValidator<T>(MetaType metaType)
        {
            foreach (var dataMember in metaType.DataMembers)
            {
                // Nullable
                if (!dataMember.CanBeNull && !dataMember.IsDbGenerated)
                {
                    yield return new NotNullValidator { Tag = SeverityLevel.Error, MessageTemplate = Resources.NonNullNonNegatedValidatorDefaultMessageTemplate }
                    .PropertyValue<T>(dataMember.Name);
                }

                // StringLength
                int maxLength = (dataMember.Type == typeof(string)) ? int.Parse(this.lengthRegexMatcher.Match(dataMember.DbType).Value) : 0;

                if (maxLength > 0)
                {
                    yield return new StringLengthValidator(1, maxLength) { Tag = SeverityLevel.Error, MessageTemplate = Resources.StringLengthValidatorNonNegatedDefaultMessageTemplate }
                    .PropertyValue<T>(dataMember.Name);
                }
            }
        }
    }
}