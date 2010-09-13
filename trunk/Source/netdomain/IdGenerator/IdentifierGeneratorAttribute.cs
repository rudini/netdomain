//-------------------------------------------------------------------------------
// <copyright file="IdentifierGeneratorAttribute.cs" company="bbv Software Services AG">
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

namespace netdomain.IdGenerator
{
    using System;

    /// <summary>
    /// Defines an attribute to define the id property and its id generator type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class IdentifierGeneratorAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifierGeneratorAttribute"/> class.
        /// </summary>
        /// <param name="generatorType">Type of the generator.</param>
        /// <param name="propertyname">The propertyname.</param>
        public IdentifierGeneratorAttribute(Type generatorType, string propertyname)
        {
            this.GeneratorType = generatorType;
            this.Propertyname = propertyname;
        }

        /// <summary>
        /// Gets or sets the type of the id to generate.
        /// </summary>
        /// <value>The type of the generator.</value>
        public Type GeneratorType { get; set; }

        /// <summary>
        /// Gets or sets the propertyname.
        /// </summary>
        /// <value>The propertyname.</value>
        public string Propertyname { get; set; }
    }
}
