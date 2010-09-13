//-------------------------------------------------------------------------------
// <copyright file="Repository.cs" company="bbv Software Services AG">
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

namespace netdomain
{
    using System.Collections.Generic;
    using System.Linq;
    using Abstract;

    /// <summary>
    /// Implements a base class of a repository based on the DDD pattern by Eric Evans.
    /// </summary>
    /// <typeparam name="T">The type of the repository.</typeparam>
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        /// <summary>
        /// The context which the repository uses.
        /// </summary>
        private readonly IWorkspace context;

        /// <summary>
        /// The query used by the repository.
        /// </summary>
        private readonly IQueryableContext<T> query;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:netdomain.Repository`1"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        protected Repository(IWorkspace context)
        {
            this.context = context;
            this.query = context.CreateQuery<T>();
        }

        /// <summary>
        /// Gets the <see cref="T:netdomain.Abstract.IQueryableContext`1"/> to query on.
        /// </summary>
        /// <value>The query.</value>
        /// <returns>A <see cref="T:netdomain.Abstract.IQueryableContext`1"/></returns>
        protected IQueryableContext<T> Query
        {
            get { return this.query; }
        }

        /// <summary>
        /// Adds the specified item to the <see cref="IWorkspace"/>.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(T item)
        {
            this.context.Add(item);
        }

        /// <summary>
        /// Deletes the specified item to the <see cref="IWorkspace"/>.
        /// </summary>
        /// <param name="item">The item to delete.</param>
        public void Delete(T item)
        {
            this.context.Delete(item);
        }

        /// <summary>
        /// Updates the specified item to the <see cref="IWorkspace"/>.
        /// </summary>
        /// <param name="item">The item to update.</param>
        public void Update(T item)
        {
            this.context.Update(item);
        }

        /// <summary>
        /// Find a particular entity or entity set by a given specification.
        /// </summary>
        /// <param name="specification">The specification.</param>
        /// <returns>
        /// An enumerable of type <see cref="T:System.Collections.Generic.IEnumerable`1"/>.
        /// </returns>
        public virtual IEnumerable<T> FindBySpecification(ISpecification<T> specification)
        {
            return this.query.Where(specification.Predicate);
        }

        /// <summary>
        /// Creates an <see cref="T:netdomain.Abstract.IQueryableContext`1"/> to query on.
        /// </summary>
        /// <returns>A <see cref="T:netdomain.Abstract.IQueryableContext`1"/></returns>
        protected IQueryableContext<T> CreateQuery()
        {
            return this.query;
        }
    }
}
