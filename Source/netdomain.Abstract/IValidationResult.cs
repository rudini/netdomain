//-------------------------------------------------------------------------------
// <copyright file="IValidationResult.cs" company="bbv Software Services AG">
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
    /// Represents the result of an atomic validation. 
    /// </summary>
    public interface IValidationResult
    {
        /// <summary>
        /// Gets a name describing the location of the validation result.
        /// </summary>
        /// <value>The name describing the location of the validation result.</value>
        string Key { get; }

        /// <summary>
        /// Gets a message describing the failure.
        /// </summary>
        /// <value>The message describing the failure.</value>
        string Message { get; }

        /// <summary>
        /// Gets the nested validation results for a composite failed validation.
        /// </summary>
        IEnumerable<IValidationResult> NestedValidationResults { get; }

        /// <summary>
        /// Gets a value characterizing the result.
        /// <remarks>
        /// The meaning for a tag is determined by the client code consuming the ValidationResults.
        /// </remarks>
        /// </summary>
        /// <value>The value characterizing the result.</value>
        string Tag { get; }

        /// <summary>
        /// Gets the object to which the validation rule was applied.
        /// <remarks>
        /// This object might not be the object for which validation was requested initially.
        /// </remarks>
        /// </summary>
        /// <value>The the object to which the validation rule was applied.</value>
        object Target { get; }
    }
}