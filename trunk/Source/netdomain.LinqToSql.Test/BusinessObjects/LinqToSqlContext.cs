//-------------------------------------------------------------------------------
// <copyright file="LinqToNHibernateContext.cs" company="bbv Software Services AG">
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

namespace netdomain.LinqToSql.Test.BusinessObjects
{
    using System.Configuration;
    using System.Data.Linq;
    using Helpers;

    public class LinqToSqlContext : DataContext
    {
        public Table<Person> Persons { get; set; }
        public Table<Adresse> Adresses { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinqToSqlContext"/> class.
        /// </summary>
        public LinqToSqlContext()
            : this(ConfigurationManager.ConnectionStrings["netdomain.LinqToSql.Test.Properties.Settings.LinqTestConnectionString"].ConnectionString, MappingHelper.GetMapping())
        {
        }

        /// <summary>
        /// Initializes a new instance of the KeySafeContext class.
        /// </summary>
        public LinqToSqlContext(string connectionString, System.Data.Linq.Mapping.MappingSource mapping)
            : base(connectionString, mapping)
        {
            this.Persons = GetTable<Person>();
            this.Adresses = GetTable<Adresse>();
        }
    }
}
