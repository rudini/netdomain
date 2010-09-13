//-------------------------------------------------------------------------------
// <copyright file="ITable.cs" company="bbv Software Services AG">
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

namespace netdomain.Abstract
{
    using System.Linq;

    /// <summary>
    /// Defines an interface for a collection of elements.
    /// </summary>
    /// <typeparam name="T">The type of the table.</typeparam>
    public interface ITable<T> : IQueryable<T>
    {
        /// <summary>
        /// Inserts the specified entity in to the in memory table.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        void Insert(T entity);

        /// <summary>
        /// Deletes the specified entity from in memory table.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        void Delete(T entity);

        /// <summary>
        /// Updates the specified entity of the in memory table.
        /// </summary>
        /// <param name="entity">The entity update.</param>
        void Update(T entity);

        /// <summary>
        /// Attaches the specified entity to the in memory table.
        /// </summary>
        /// <param name="entity">The entity to attach.</param>
        void Attach(T entity);

        /// <summary>
        /// Inserts the specified entity in to the in memory table on submit.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void InsertOnSubmit(T entity);

        /// <summary>
        /// Deletes the specified entity from in memory table on submit.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        void DeleteOnSubmit(T entity);

        /// <summary>
        /// Updates the specified entity of the in memory table on submit.
        /// </summary>
        /// <param name="entity">The entity update.</param>
        void UpdateOnSubmit(T entity);
    }
}
