//-------------------------------------------------------------------------------
// <copyright file="IValidationHelper.cs" company="bbv Software Services AG">
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

namespace netdomain.Abstract
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines an interface for a entity validator.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Validates the entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <exception cref="T:netdomain.Abstract.ValidationException`1"></exception>
        void ValidateEntities(IEnumerable<object> entities);
    }
}
