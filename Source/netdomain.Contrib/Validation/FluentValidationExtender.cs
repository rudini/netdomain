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
    using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

    /// <summary>
    /// Implements a fluent interface for the validation application block.
    /// </summary>
    public static class FluentValidationExtender
    {
        /// <summary>
        /// Sets the property value of a <see cref="T:Microsoft.Practices.EnterpriseLibrary.Validation.Validators.ValueValidator`1"/>
        /// </summary>
        /// <typeparam name="T">The type to get the property validator for.</typeparam>
        /// <param name="validator">The value validator.</param>
        /// <param name="name">The name of the propery to get the property validator for.</param>
        /// <returns>A new property validator <see cref="T:Microsoft.Practices.EnterpriseLibrary.Validation.Validators.ValueValidator`1"/></returns>
        public static PropertyValueValidator<T> PropertyValue<T>(this ValueValidator validator, string name)
        {
            return new PropertyValueValidator<T>(name, validator);
        }

        /// <summary>
        /// Sets the property value of a <see cref="T:Microsoft.Practices.EnterpriseLibrary.Validation.Validators.ValueValidator`1"/>
        /// </summary>
        /// <typeparam name="T">The type to get the property validator for.</typeparam>
        /// <typeparam name="TValue">The type of the value validator.</typeparam>
        /// <param name="validator">The validator.</param>
        /// <param name="name">The name of the propery to get the property validator for.</param>
        /// <returns>A new property validator <see cref="T:Microsoft.Practices.EnterpriseLibrary.Validation.Validators.ValueValidator`1"/></returns>
        public static PropertyValueValidator<T> PropertyValue<T, TValue>(this ValueValidator<TValue> validator, string name)
        {
            return new PropertyValueValidator<T>(name, validator);
        }

        /// <summary>
        /// Sets the property value of a <see cref="T:Microsoft.Practices.EnterpriseLibrary.Validation.Validators.StringLengthValidator"/>
        /// </summary>
        /// <typeparam name="T">The type to get the property validator for.</typeparam>
        /// <param name="validator">The value validator.</param>
        /// <param name="name">The name of the propery to get the property validator for.</param>
        /// <returns>A new property validator <see cref="T:Microsoft.Practices.EnterpriseLibrary.Validation.Validators.ValueValidator`1"/></returns>
        public static PropertyValueValidator<T> PropertyValue<T>(this StringLengthValidator validator, string name)
        {
            return new PropertyValueValidator<T>(name, validator);
        }

        /// <summary>
        /// Sets the field value of a <see cref="T:Microsoft.Practices.EnterpriseLibrary.Validation.Validators.ValueValidator`1"/>
        /// </summary>
        /// <typeparam name="T">The type to get the value validator for.</typeparam>
        /// <param name="validator">The value validator.</param>
        /// <param name="name">The name of the field to get the field validator for.</param>
        /// <returns>A new field validator <see cref="T:Microsoft.Practices.EnterpriseLibrary.Validation.Validators.ValueValidator`1"/></returns>
        public static FieldValueValidator<T> FieldValue<T>(this ValueValidator validator, string name)
        {
            return new FieldValueValidator<T>(name, validator);
        }
    }
}