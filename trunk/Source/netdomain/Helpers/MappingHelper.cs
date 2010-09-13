//-------------------------------------------------------------------------------
// <copyright file="MappingHelper.cs" company="bbv Software Services AG">
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

namespace netdomain.Helpers
{
    using System.Configuration;
    using System.Data.Linq.Mapping;
    using System.Reflection;

    /// <summary>
    /// Implements helper methods to load the mapping from a mapping file or embedded resource.
    /// </summary>
    public static class MappingHelper
    {
        /// <summary>
        /// Gets the <see cref="XmlMappingSource"/> to configure the DataContext.
        /// </summary>
        /// <returns>The <see cref="XmlMappingSource"/> to configure the DataContext.</returns>
        public static XmlMappingSource GetMapping()
        {            
            XmlMappingSource mapping = null;
            var mappingResource = ConfigurationManager.AppSettings["LinqToSqlMapping"];

            if (mappingResource.StartsWith("res://"))
            {
                using (var stream = Assembly.GetCallingAssembly().GetManifestResourceStream(mappingResource.Substring(6)))
                {
                    if (stream != null)
                    {
                        mapping = XmlMappingSource.FromStream(stream);
                    }
                }
            }
            else
            {
                mapping = XmlMappingSource.FromUrl(mappingResource);
            }

            return mapping;
        }
    }
}
