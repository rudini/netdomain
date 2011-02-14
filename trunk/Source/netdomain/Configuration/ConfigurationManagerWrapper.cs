//-------------------------------------------------------------------------------
// <copyright file="ConfigurationManagerWrapper.cs" company="bbv Software Services AG">
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
    using System.Collections.Specialized;
    using System.Configuration;

    using netdomain.Abstract.Configuration;

    /// <summary>
    /// Implements a configuration manager wrapper to enable TDD.
    /// </summary>
    public class ConfigurationManagerWrapper : IConfigurationManager
    {
        /// <summary>
        /// Gets the <see cref="T:System.Configuration.AppSettingsSection"/> object configuration section that applies to this <see cref="T:System.Configuration.Configuration"/> object.
        /// </summary>
        /// <value>The app settings.</value>
        /// <returns>An <see cref="T:System.Configuration.AppSettingsSection"/> object representing the appSettings configuration section that applies to this <see cref="T:System.Configuration.Configuration"/> object.</returns>
        public virtual NameValueCollection AppSettings
        {
            get { return ConfigurationManager.AppSettings; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Configuration.ConnectionStringsSection"/> data for the current application's default configuration.
        /// </summary>
        /// <value>The connection strings.</value>
        /// <returns>Returns a <see cref="T:System.Configuration.ConnectionStringSettingsCollection"/> object that contains the contents of the <see cref="T:System.Configuration.ConnectionStringsSection"/> object for the current application's default configuration. </returns>
        /// <exception cref="T:System.Configuration.ConfigurationErrorsException">Could not retrieve a <see cref="T:System.Configuration.ConnectionStringSettingsCollection"/> object.</exception>
        public virtual ConnectionStringSettingsCollection ConnectionStrings
        {
            get { return ConfigurationManager.ConnectionStrings; }
        }

        /// <summary>
        /// Opens the configuration file for the current application as a <see cref="T:System.Configuration.Configuration"/> object.
        /// </summary>
        /// <param name="userLevel">The <see cref="T:System.Configuration.ConfigurationUserLevel"/> for which you are opening the configuration.</param>
        /// <returns>
        /// A <see cref="T:System.Configuration.Configuration"/> object.
        /// </returns>
        /// <exception cref="T:System.Configuration.ConfigurationErrorsException">A configuration file could not be loaded.</exception>
        public virtual IConfiguration OpenExeConfiguration(ConfigurationUserLevel userLevel)
        {
            return new ConfigurationWrapper(ConfigurationManager.OpenExeConfiguration(userLevel));
        }
    }
}