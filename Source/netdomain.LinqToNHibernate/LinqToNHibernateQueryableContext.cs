//-------------------------------------------------------------------------------
// <copyright file="LinqToNHibernateQueryableContext.cs" company="bbv Software Services AG">
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

namespace netdomain.LinqToNHibernate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using netdomain.Abstract;
    using NHibernate;
    using NHibernate.Linq;

    /// <summary>
    /// Queryable context for NHibernate.
    /// </summary>
    /// <typeparam name="T">Type to query</typeparam>
    internal class LinqToNHibernateQueryableContext<T> : IQueryableContext<T> where T : class
    {
        /// <summary>
        /// The NHIbernate query object
        /// </summary>
        private readonly IQueryable<T> query;

        /// <summary>
        /// The Query context (NHibernate session)
        /// </summary>
        private readonly ISession context;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:netdomain.LinqToNHibernate.LinqToNHibernateQueryableContext`1"/> class.
        /// </summary>
        /// <param name="session">NHibernate session.</param>
        internal LinqToNHibernateQueryableContext(ISession session)
        {
            this.context = session;
            this.query = this.context.Query<T>();
        }

        /// <summary>
        /// Gets the type of the element(s) that are returned if the expression tree associated with this instance of <see cref="T:System.Linq.IQueryable"/> is executed.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// A <see cref="T:System.Type"/> that represents the type of the element(s) that are returned if the expression tree associated with this object is executed.
        /// </returns>
        public Type ElementType
        {
            get { return this.query.ElementType; }
        }

        /// <summary>
        /// Gets the expression tree that is associated with the instance of <see cref="T:System.Linq.IQueryable"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The <see cref="T:System.Linq.Expressions.Expression"/> that is associated with this instance of <see cref="T:System.Linq.IQueryable"/>.
        /// </returns>
        public Expression Expression
        {
            get { return this.query.Expression; }
        }

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The <see cref="T:System.Linq.IQueryProvider"/> that is associated with this data source.
        /// </returns>
        public IQueryProvider Provider
        {
            get { return this.query.Provider; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.query.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.query.GetEnumerator();
        }
    }
}
