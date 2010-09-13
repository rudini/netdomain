//-------------------------------------------------------------------------------
// <copyright file="IWorkspace.cs" company="bbv Software Services AG">
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
    using System;

    /// <summary>
    /// Defines an interface to implement a Unit of Work workspace
    /// based on the Unit of Work pattern of Martin Fowler.
    /// </summary>
    public interface IWorkspace : IDisposable
    {
        /// <summary>
        /// Gets the connection manager.
        /// </summary>
        /// <value>The connection manager.</value>
        IConnectionManager ConnectionManager { get; }

        /// <summary>
        /// Persists all pending updates to the data source.
        /// </summary>
        /// <exception cref="T:netdomain.Abstract.OptimisticOfflineLockException"/>
        /// <exception cref="T:netdomain.Abstract.ValidationException`1"></exception>
        void SubmitChanges();

        /// <summary>
        /// Returns the persistent instance of the given named entity with the given identifier,
        /// or null if there is no such persistent instance. (If the instance, or a proxy for the
        /// instance, is already associated with the session, return that instance or proxy.)
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entityWithKey">An entity with a key.</param>
        /// <returns>A persistent instance or null.</returns>
        T Get<T>(T entityWithKey) where T : class;

        /// <summary>
        /// Inserts an entity in a pending insert state to the <see cref="IWorkspace"/> context.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">A transient instance of a persistent class.</param>
        void Add<T>(T entity) where T : class;

        /// <summary>
        /// Remove an entity from this table into a pending delete state.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The instance to be removed.</param>
        void Delete<T>(T entity) where T : class;

        /// <summary>
        /// Update the persistent instance with the identifier of the given transient instance.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The detached object that has property updates
        /// to apply to the original object.</param>
        /// <remarks>
        /// If there is a persistent instance with the same identifier, an exception is thrown. If
        /// the given transient instance has a <see langword="null"/> identifier, an exception will be thrown.
        /// </remarks>
        void Update<T>(T entity) where T : class;

        /// <summary>
        /// Cleans all cached objects within the <see cref="IWorkspace"/>.
        /// </summary>
        void Clean();

        /// <summary>
        /// Updates an object in the object context with data from the persisted store.
        /// Overrides all the current values with the values from the database.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity to be refreshed.</param>
        void Refresh<T>(T entity) where T : class;

        /// <summary>
        /// Attach an object or object graph to the object context if the object
        /// has an entity key.
        /// </summary>
        /// <remarks>
        /// Changed properties of an attached entity are ignored and will not be saved to the database.
        /// </remarks>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entityWithKey">The entity to attach.</param>
        void Attach<T>(T entityWithKey) where T : class;

        /// <summary>
        /// Removes the specified object from the <see cref="IWorkspace"/> context.
        /// </summary>
        /// <remarks>
        /// The entity will only be removed from the tracking manager and not removed from the id map.
        /// </remarks>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity to detach from the context.</param>
        void Detach<T>(T entity) where T : class;

        /// <summary>
        /// Gets a boolean value indicating whether pending changes exist in the workspace.
        /// </summary>
        /// <returns>
        /// True if there are pending changes, otherwise false.
        /// </returns>
        bool IsDirty();

        /// <summary>
        /// Creates an <see cref="T:netdomain.Abstract.IQueryableContext`1"/> in the current object context
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="T:netdomain.Abstract.IQueryableContext`1"/>.</typeparam>
        /// <returns>An <see cref="T:netdomain.Abstract.IQueryableContext`1"/> of the specified type.</returns>
        IQueryableContext<T> CreateQuery<T>() where T : class;
    }

    /// <summary>
    /// Defines an interface to implement a Unit of Work workspace
    /// based on the Unit of Work pattern of Martin Fowler.
    /// </summary>
    /// <typeparam name="TWrapped">The type of the wrapped instance.</typeparam>
    public interface IWorkspace<TWrapped> : IWorkspace
    {
        /// <summary>
        /// Gets the wrapped instance.
        /// </summary>
        /// <value>The wrapped instance.</value>
        TWrapped WrappedInstance { get; }
    }
}
