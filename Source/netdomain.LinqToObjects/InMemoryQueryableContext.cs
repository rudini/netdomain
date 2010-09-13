//-------------------------------------------------------------------------------
// <copyright file="InMemoryQueryableContext.cs" company="bbv Software Services AG">
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

namespace netdomain.LinqToObjects
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using Abstract;

    /// <summary>
    /// Implements an in memory queryable context.
    /// </summary>
    /// <typeparam name="T">The type of the objects to query.</typeparam>
    public class InMemoryQueryableContext<T> : IQueryableContext<T>, ITable<T> where T : class
    {
        /// <summary>
        /// Holds a reference to the associated context.
        /// </summary>
        private readonly InMemoryContext context;

        /// <summary>
        /// A List that holds all objects of a particular type T.
        /// </summary>
        private readonly IList<T> list;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:netdomain.LinqToObjects.InMemoryQueryableContext`1"/> class.
        /// </summary>
        /// <param name="context">The context</param>
        public InMemoryQueryableContext(InMemoryContext context)
        {
            this.context = context;
            this.list = new List<T>(); 
        }

        #region IQueryable Members

        /// <summary>
        /// Gets the type of the element(s) that are returned if the expression tree associated with this instance of <see cref="T:System.Linq.IQueryable"/> is executed.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Type"/> that represents the type of the element(s) that are returned if the expression tree associated with this object is executed.</returns>
        public Type ElementType
        {
            get { return this.list.AsQueryable().ElementType; }
        }

        /// <summary>
        /// Gets the expression tree that is associated with the instance of <see cref="T:System.Linq.IQueryable"/>.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.Linq.Expressions.Expression"/> that is associated with this instance of <see cref="T:System.Linq.IQueryable"/>.</returns>
        public Expression Expression
        {
            get { return this.list.AsQueryable().Expression; }
        }

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.Linq.IQueryProvider"/> that is associated with this data source.</returns>
        public IQueryProvider Provider
        {
            get { return this.list.AsQueryable().Provider; }
        }

        #endregion

        #region ITable<T> Members

        /// <summary>
        /// Inserts the specified entity in to the in memory table.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        public void Insert(T entity)
        {
            this.list.Add(entity);

            if (typeof(INotifyPropertyChanged).IsAssignableFrom(entity.GetType()))
            {
                this.context.AddToChangeTracking(entity as INotifyPropertyChanged);
            }
        }

        /// <summary>
        /// Inserts the specified entity in to the in memory table on submit.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void InsertOnSubmit(T entity)
        {
            this.context.GetChangeSet().Inserts.Add(entity);
        }

        /// <summary>
        /// Deletes the specified entity from in memory table.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public void Delete(T entity)
        {
            this.list.Remove(entity);

            if (entity.GetType().IsAssignableFrom(typeof(INotifyPropertyChanged)))
            {
                this.context.RemoveFromChangeTracking(entity as INotifyPropertyChanged);
            }
        }

        /// <summary>
        /// Deletes the specified entity from in memory table on submit.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public void DeleteOnSubmit(T entity)
        {
            if (this.list.Contains(entity))
            {
                this.context.GetChangeSet().Deletes.Add(entity);
            }

            if (this.context.GetChangeSet().Inserts.Contains(entity))
            {
                this.context.GetChangeSet().Inserts.Remove(entity);
            }
        }

        /// <summary>
        /// Updates the specified entity of the in memory table.
        /// </summary>
        /// <param name="entity">The entity update.</param>
        public void Update(T entity)
        {
            if (this.list.Contains(entity))
            {
                this.list[this.list.IndexOf(entity)] = entity;
            }
        }

        /// <summary>
        /// Updates the specified entity of the in memory table on submit.
        /// </summary>
        /// <param name="entity">The entity update.</param>
        public void UpdateOnSubmit(T entity)
        {
            if (this.list.Contains(entity))
            {
                this.context.GetChangeSet().Updates.Add(entity);
            }
        }

        /// <summary>
        /// Attaches the specified entity to the in memory table.
        /// </summary>
        /// <param name="entity">The entity to attach.</param>
        public void Attach(T entity)
        {
            this.Update(entity);
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
            return this.list.GetEnumerator();
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
            return this.list.GetEnumerator();
        }

        #endregion

        #region IQueryableContext<T> Members

        /// <summary>
        /// Specifies the related objects to include in the query results.
        /// </summary>
        /// <param name="path">Dot-separated list of related objects to return in the query results.</param>
        /// <returns>A new <see cref="T:netdomain.Abstract.IQueryableContext`1"/> with the defined query path.</returns>
        public IQueryableContext<T> Include(string path)
        {
            return this;
        }

        /// <summary>
        /// Specifies which sub-objects to retrieve when a query is submitted for an object of type T.
        /// </summary>
        /// <param name="expression">Identifies the field or property to be retrieved.
        /// If the expression does not identify a field or property that represents a one-to-one or one-to-many relationship, an exception is thrown.</param>
        /// <returns>A new <see cref="T:netdomain.Abstract.IQueryableContext`1"/> with the defined query path.</returns>
        /// <remarks>
        /// You cannot specify the loading of two levels of relationships (for example, Orders.OrderDetails). In these scenarios you must specify two separate Include methods.
        /// </remarks>
        public IQueryableContext<T> Include(Expression<Func<T, object>> expression)
        {
            return this;
        }

        #endregion
    }
}
