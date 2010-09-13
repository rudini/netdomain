//-------------------------------------------------------------------------------
// <copyright file="SeverityLevel.cs" company="bbv Software Services AG">
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
    /// <summary>
    /// Defines severity level to classify the validation results.
    /// </summary>
    public class SeverityLevel
    {
        /// <summary>
        /// Defines a constant for an error.
        /// An error indicate an object in an error state that cannot be persisted.
        /// </summary>
        public const string Error = "severity://Error";

        /// <summary>
        /// Defines a constant for a warning.
        /// A warning indicate that an object has warnings, but can be persisted.
        /// </summary>
        public const string Warning = "severity://Warning";

        /// <summary>
        /// Defines a constant for a annotation.
        /// Indicates some annotations usefull for the consumer.
        /// </summary>
        public const string Annotation = "severity://Annotation";
    }
}