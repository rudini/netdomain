//-------------------------------------------------------------------------------
// <copyright file="LinqToSqlQueryableContext.cs" company="bbv Software Services AG">
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

namespace netdomain.LinqToSql
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Linq;
    using System.Linq.Expressions;
    using Abstract;

    /// <summary>
    /// Implements a Linq to entities queryable context.
    /// </summary>
    /// <typeparam name="T">The type of the queryable context.</typeparam>
    internal class LinqToSqlQueryableContext<T> : IQueryableContext<T> where T : class
    {
        /// <summary>
        /// The table holds the persistent object.
        /// </summary>
        private readonly Table<T> table;

        /// <summary>
        /// The data load options.
        /// </summary>
        private DataLoadOptions dataLoadOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:netdomain.LinqToSql.LinqToSqlQueryableContext`1"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        internal LinqToSqlQueryableContext(DataContext dataContext)
        {
            this.table = dataContext.GetTable<T>();
        }

        #region IQueryable Members

        /// <summary>
        /// Gets the type of the element(s) that are returned if the expression tree associated with this instance of <see cref="T:System.Linq.IQueryable"/> is executed.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Type"/> that represents the type of the element(s) that are returned if the expression tree associated with this object is executed.</returns>
        public Type ElementType
        {
            get { return ((IQueryable)this.table).ElementType; }
        }

        /// <summary>
        /// Gets the expression tree that is associated with the instance of <see cref="T:System.Linq.IQueryable"/>.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.Linq.Expressions.Expression"/> that is associated with this instance of <see cref="T:System.Linq.IQueryable"/>.</returns>
        public Expression Expression
        {
            get { return ((IQueryable)this.table).Expression; }
        }

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.Linq.IQueryProvider"/> that is associated with this data source.</returns>
        public IQueryProvider Provider
        {
            get
            {
                if (this.dataLoadOptions != null)
                {
                    this.table.Context.LoadOptions = this.dataLoadOptions;
                }

                this.dataLoadOptions = null;

                return ((IQueryable)this.table).Provider;
            }
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
            return this.table.GetEnumerator();
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
            return this.table.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Specifies which sub-objects to retrieve when a query is submitted for an object of type T.
        /// </summary>
        /// <param name="expression">Identifies the field or property to be retrieved.
        /// If the expression does not identify a field or property that represents a one-to-one or one-to-many relationship, an exception is thrown.</param>
        /// <remarks>
        /// You cannot specify the loading of two levels of relationships (for example, Orders.OrderDetails). In these scenarios you must specify two separate Include methods.
        /// </remarks>
        internal void LoadWith(LambdaExpression expression)
        {
            if (this.dataLoadOptions == null)
            {
                this.dataLoadOptions = new DataLoadOptions();
            }

            this.dataLoadOptions.LoadWith(expression);
        }
    }
}
