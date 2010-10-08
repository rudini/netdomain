//-------------------------------------------------------------------------------
// <copyright file="TypeHelper.cs" company="bbv Software Services AG">
//   Copyright (c) 2010 bbv Software Services AG
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

namespace netdomain.Helpers
{
    using System;

    /// <summary>
    /// Implements helper methods to inspect a type.
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        /// Gets the default value for a type.
        /// </summary>
        /// <param name="type">The specified type.</param>
        /// <returns>The default value as an object.</returns>
        public static object GetDefault(Type type)
        {
            bool isNullable = !type.IsValueType || IsNullableType(type);
            return !isNullable ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// Determines whether the specified type is nullable.
        /// </summary>
        /// <param name="type">The specified type.</param>
        /// <returns>
        /// <c>true</c> if nullable; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullableType(Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}