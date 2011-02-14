//-------------------------------------------------------------------------------
// <copyright file="LinqToSqlContext.cs" company="bbv Software Services AG">
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

namespace netdomain.LinqToNHibernate.Test.BusinessObjects
{
    using NHibernate;
    using NHibernate.Cfg;

    /// <summary>
    /// Nhibernate Context for Persons
    /// </summary>
    public class LinqToNHibernateContext
    {
        /// <summary>
        /// NHIbernate session
        /// </summary>
        private static ISessionFactory sessionFactory;

        /// <summary>
        /// Initializes static members of the <see cref="LinqToNHibernateContext"/> class.
        /// </summary>
        static LinqToNHibernateContext()
        {
            SetupNHibernate();
        }

        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <returns>The configured session</returns>
        public ISession GetSession()
        {
            return sessionFactory.OpenSession().GetSession(EntityMode.Poco);
        }

        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <param name="entityMode">The entity mode.</param>
        /// <returns>The configured session</returns>
        public ISession GetSession(EntityMode entityMode)
        {
            return sessionFactory.OpenSession().GetSession(entityMode);
        }
        
        /// <summary>
        /// Setup the NHibernate session to work with Persons.
        /// </summary>
        private static void SetupNHibernate()
        {
            //Environment.BytecodeProvider = new EnhancedBytecode(container);
            var cfg = new Configuration();
            cfg.SetProperty(Environment.ConnectionDriver, "NHibernate.Driver.SqlClientDriver");
            cfg.SetProperty(Environment.ConnectionString, @"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\App_Data\LinqTest.mdf;Integrated Security=True;User Instance=True");
            cfg.SetProperty(Environment.BatchSize, "10");
            cfg.SetProperty(Environment.ShowSql, "false");
            cfg.SetProperty(Environment.GenerateStatistics, "true");
            cfg.SetProperty(Environment.Dialect, "NHibernate.Dialect.MsSql2005Dialect");
            cfg.SetProperty(Environment.CommandTimeout, "60");
            cfg.SetProperty(Environment.QuerySubstitutions, "true 1, false 0, yes 'Y', no 'N'");
            cfg.SetProperty(Environment.ProxyFactoryFactoryClass, "NHibernate.ByteCode.LinFu.ProxyFactoryFactory, NHibernate.ByteCode.LinFu");

            //var cfg = new Configuration().Configure();
            cfg.AddAssembly(typeof(Person).Assembly);
            sessionFactory = cfg.BuildSessionFactory();
        }
    }
}
