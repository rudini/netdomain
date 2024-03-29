﻿//-------------------------------------------------------------------------------
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
    using System.Data.Linq.Mapping;

    using Helpers;

    using netdomain.Abstract.Configuration;

    /// <summary>
    /// Implements a Linq to sql context.
    /// </summary>
    public class LinqToSqlContext : DataContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinqToSqlContext"/> class.
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        public LinqToSqlContext(IConfigurationManager configurationManager)
            : this(configurationManager.ConnectionStrings["netdomain.LinqToSql.Test.Properties.Settings.LinqTestConnectionString"].ConnectionString, MappingHelper.GetMapping(configurationManager))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinqToSqlContext"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="mapping">The mapping.</param>
        public LinqToSqlContext(string connectionString, MappingSource mapping)
            : base(connectionString, mapping)
        {
            this.Persons = GetTable<Person>();
            this.Adresses = GetTable<Adresse>();
        }

        /// <summary>
        /// Gets or sets the persons.
        /// </summary>
        /// <value>The persons.</value>
        public Table<Person> Persons { get; set; }

        /// <summary>
        /// Gets or sets the adresses.
        /// </summary>
        /// <value>The adresses.</value>
        public Table<Adresse> Adresses { get; set; }
    }
}
