//-------------------------------------------------------------------------------
// <copyright file="ConfigurationWrapper.cs" company="bbv Software Services AG">
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

namespace netdomain.Configuration
{
    using System.Configuration;
    using netdomain.Abstract.Configuration;

    /// <summary>
    /// Wrapps a configuration to enable TDD.
    /// </summary>
    public class ConfigurationWrapper : IConfiguration
    {
        /// <summary>
        /// The wrapped configuration.
        /// </summary>
        private readonly Configuration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationWrapper"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public ConfigurationWrapper(Configuration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Gets or sets the connection string at the specified index in the collection.
        /// </summary>
        /// <param name="index">The index of the connection string</param>
        /// <returns>The <see cref="T:System.Configuration.ConnectionStringSettings"/> object at the specified index.</returns>
        public virtual ConnectionStringSettings this[int index]
        {
            get { return this.configuration.ConnectionStrings.ConnectionStrings[index]; }
        }

        /// <summary>
        /// Gets the <see cref="System.Configuration.ConnectionStringSettings"/> with the specified name.
        /// </summary>
        /// <param name="name">The name of the conection string.</param>
        public virtual ConnectionStringSettings this[string name]
        {
            get { return this.configuration.ConnectionStrings.ConnectionStrings[name]; }
        }
    }
}