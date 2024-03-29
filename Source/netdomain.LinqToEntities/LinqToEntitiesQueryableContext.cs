﻿//-------------------------------------------------------------------------------
// <copyright file="LinqToEntitiesQueryableContext.cs" company="bbv Software Services AG">
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

namespace netdomain.LinqToEntities
{
    using System;
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Linq;
    using System.Linq.Expressions;
    using Abstract;

    /// <summary>
    /// Implements a Linq to entities queryable context.
    /// </summary>
    /// <typeparam name="T">The type of the queryable context.</typeparam>
    internal class LinqToEntitiesQueryableContext<T> : IQueryableContext<T> where T : class
    {
        /// <summary>
        /// The object query acts as a persisted table here.
        /// </summary>
        private ObjectQuery<T> objectQuery;

        /// <summary>
        /// The query provider.
        /// </summary>
        private EFQueryProvider queryProvider;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:netdomain.LinqToEntities.LinqToEntitiesQueryableContext`1"/> class.
        /// </summary>
        /// <param name="objectContext">The object context.</param>
        /// <param name="queryLogger">The query logger.</param>
        internal LinqToEntitiesQueryableContext(ObjectContext objectContext, QueryLogger queryLogger)
        {
            this.objectQuery = objectContext.CreateQuery<T>(objectContext.GetEntitySetName<T>());
            this.queryProvider = new EFQueryProvider(this.objectQuery.AsQueryable().Provider, queryLogger);
        }

        #region IQueryable Members

        /// <summary>
        /// Gets the type of the element(s) that are returned if the expression tree associated with this instance of <see cref="T:System.Linq.IQueryable"/> is executed.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Type"/> that represents the type of the element(s) that are returned if the expression tree associated with this object is executed.</returns>
        public Type ElementType
        {
            get { return this.objectQuery.AsQueryable().ElementType; }
        }

        /// <summary>
        /// Gets the expression tree that is associated with the instance of <see cref="T:System.Linq.IQueryable"/>.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.Linq.Expressions.Expression"/> that is associated with this instance of <see cref="T:System.Linq.IQueryable"/>.</returns>
        public Expression Expression
        {
            get { return this.objectQuery.AsQueryable().Expression; }
        }

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.Linq.IQueryProvider"/> that is associated with this data source.</returns>
        public IQueryProvider Provider
        {
            get { return this.queryProvider; }
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.objectQuery.AsQueryable().GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.objectQuery.AsQueryable().GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Specifies the related objects to include in the query results.
        /// </summary>
        /// <param name="path">Dot-separated list of related objects to return in the query results.</param>
        /// <returns>A new <see cref="T:netdomain.Abstract.IQueryableContext`1"/> with the defined query path.</returns>
        internal IQueryableContext<T> Include(string path)
        {
            this.objectQuery = this.objectQuery.Include(path);
            return this;
        }
    }
}
