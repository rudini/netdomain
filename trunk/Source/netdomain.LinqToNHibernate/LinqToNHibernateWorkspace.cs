//-------------------------------------------------------------------------------
// <copyright file="LinqToNHibernateWorkspace.cs" company="bbv Software Services AG">
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
    using System.Data;
    using System.Linq;
    using netdomain;
    using netdomain.Abstract;
    using NHibernate;

    /// <summary>
    /// Implements a unit of work for the NHibernate object/relational mapping framework 
    /// </summary>
    public class LinqToNHibernateWorkspace : IWorkspace<ISession>, IConnectionManager
    {
        /// <summary>
        /// The NHibernate session
        /// </summary>
        private readonly ISession context;

        /// <summary>
        /// The extension registered to the workspace.
        /// </summary>
        private readonly List<IWorkspaceExtension> extensions = new List<IWorkspaceExtension>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LinqToNHibernateWorkspace"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public LinqToNHibernateWorkspace(ISession context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.context = context;
            WorkspaceBuilder.Current.GetExtensionInstances().ToList().ForEach(ex => { ex.Workspace = this; this.extensions.Add(ex); });
        }

        /// <summary>
        /// Gets the wrapped instance.
        /// </summary>
        /// <value>The wrapped instance.</value>
        public ISession WrappedInstance
        {
            get { return this.context; }
        }

        /// <summary>
        /// Gets the connection manager.
        /// </summary>
        /// <value>The connection manager.</value>
        public IConnectionManager ConnectionManager
        {
            get { return this; }
        }

        /// <summary>
        /// Gets the underlying ADO .Net connection of the unit of work.
        /// </summary>
        /// <value>The connection as a <see cref="IDbConnection"/>.</value>
        public IDbConnection Connection
        {
            get { return this.context.Connection; }
        }

        /// <summary>
        /// Gets a value indicating whether this workspace is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get { return this.context.IsConnected; }
        }

        /// <summary>
        /// Disconnects this instance from the current ADO .Net connection.
        /// </summary>
        /// <returns>The current ADO .Net connection.</returns>
        public IDbConnection Disconnect()
        {
            return this.IsConnected ? this.context.Disconnect() : this.Connection;
        }

        /// <summary>
        /// Optain a new ADO .Net connection
        /// </summary>
        /// <returns>The current ADO .Net connection.</returns>
        public IDbConnection Reconnect()
        {
            if (!this.IsConnected)
            {
                this.context.Reconnect();
            }

            return this.Connection;
        }

        /// <summary>
        /// Persists all pending updates to the data source.
        /// </summary>
        /// <exception cref="T:netdomain.Abstract.OptimisticOfflineLockException"/>
        /// <exception cref="T:netdomain.Abstract.ValidationException`1"></exception>
        public void SubmitChanges()
        {
            IEnumerable<object> deletedEntities, addedEntities, modifiedEntities;
            deletedEntities = addedEntities = modifiedEntities = null;

            foreach (var extension in this.extensions)
            {
                if (deletedEntities == null)
                {
                    deletedEntities = this.context.GetEntitiesFromActionQueues(TrackingState.Deleted);
                }

                if (addedEntities == null)
                {
                    addedEntities = this.context.GetEntitiesFromActionQueues(TrackingState.Added);
                }

                if (modifiedEntities == null)
                {
                    modifiedEntities = this.context.GetEntitiesFromActionQueues(TrackingState.Modified);
                }

                extension.OnSubmittingChanges(deletedEntities, addedEntities, modifiedEntities);
            }

            try
            {
                this.context.Flush();
            }
            catch (StaleObjectStateException e)
            {
                var conflictedObject = this.context.Get(e.EntityName, e.Identifier);

                var exception = new OptimisticOfflineLockException(e.Message, e, new[] { new ConflictedObject(conflictedObject, conflictedObject.GetType()) });
                this.extensions.ForEach(ex => ex.OnOptimisticOfflineLockExceptionThrown(exception));

                throw exception;
            }
        }

        /// <summary>
        /// Accepts all changes made to objects in the <see cref="IWorkspace"/> context.
        /// </summary>
        public void AcceptAllChanges()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the specified entity with key.
        /// </summary>
        /// <typeparam name="T">Type to get</typeparam>
        /// <param name="entityWithKey">The entity with key.</param>
        /// <returns>Instance of T</returns>
        public T Get<T>(T entityWithKey) where T : class
        {
            return this.context.Load<T>(
                this.context.SessionFactory.GetClassMetadata(
                typeof(T)).GetIdentifier(entityWithKey, this.context.ActiveEntityMode));
        }

        /// <summary>
        /// Save an entity in a pending insert state to the <see cref="IWorkspace"/> context.
        /// </summary>
        /// <typeparam name="T">Type to insert</typeparam>
        /// <param name="entity">The entity.</param>
        /// <exception cref="T:netdomain.Abstract.ValidationException`1"></exception>
        public void Add<T>(T entity) where T : class
        {
            try
            {
                // In NHibernate the add method is not like the AddObjects or InsertOnSubmit
                // If there are db generated identities used, the save method performs an insert on the database.
                // The validation rules must be checked if an error on the database occurs.
                this.context.Save(entity);
            }
            catch (ADOException exception)
            {
                this.extensions.ForEach(ex => ex.OnExceptionThrown(exception));
                throw;
            }

            this.extensions.ForEach(e => e.OnEntityAdded(entity));
        }

        /// <summary>
        /// Remove an entity from this table into a pending delete state.
        /// </summary>
        /// <typeparam name="T">Type to delete</typeparam>
        /// <param name="entity">The entity.</param>
        public void Delete<T>(T entity) where T : class
        {
            this.context.Delete(entity);
            this.extensions.ForEach(e => e.OnEntityDeleted(entity));
        }

        /// <summary>
        /// Applies property changes from a detached object
        /// to an object already attached to the <see cref="IWorkspace"/> context.
        /// </summary>
        /// <typeparam name="T">Type to update</typeparam>
        /// <param name="entity">The entity.</param>
        public void Update<T>(T entity) where T : class
        {
            this.context.Update(entity);
            this.extensions.ForEach(e => e.OnEntityUpdated(entity));

        }

        /// <summary>
        /// Cleans all cached objects within the <see cref="IWorkspace"/>.
        /// </summary>
        public void Clean()
        {
            this.context.Clear();
            this.extensions.ForEach(e => e.OnCacheCleaned());
        }

        /// <summary>
        /// Updates an object in the object context with data from the persisted store.
        /// Overrides all the current values with the values from the database.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity to be refreshed.</param>
        public void Refresh<T>(T entity) where T : class
        {
            this.context.Refresh(entity);
            this.extensions.ForEach(e => e.OnEntityRefreshed(entity));
        }

        /// <summary>
        /// Attaches the specified entity with key.
        /// </summary>
        /// <typeparam name="T">Type to attache</typeparam>
        /// <param name="entityWithKey">The entity with key.</param>
        public void Attach<T>(T entityWithKey) where T : class
        {
            this.context.Update(entityWithKey);
            this.extensions.ForEach(e => e.OnEntityAttached(entityWithKey));
        }

        /// <summary>
        /// Removes the specified object from the <see cref="IWorkspace"/> context.
        /// </summary>
        /// <typeparam name="T">The type of the entity</typeparam>
        /// <param name="entity">The entity.</param>
        public void Detach<T>(T entity) where T : class
        {
            this.context.Evict(entity);
            this.extensions.ForEach(e => e.OnEntityDetached(entity));
        }

        /// <summary>
        /// Gets a boolean value indicating whether pending changes exist in the workspace.
        /// </summary>
        /// <returns>
        /// True if there are pending changes, otherwise false.
        /// </returns>
        public bool IsDirty()
        {
            return this.context.IsDirty();
        }

        /// <summary>
        /// Creates an <see cref="T:netdomain.Abstract.IQueryableContext`1"/> in the current object context
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="T:netdomain.Abstract.IQueryableContext`1"/>.</typeparam>
        /// <returns>An <see cref="T:netdomain.Abstract.IQueryableContext`1"/> of the specified type.</returns>
        public IQueryableContext<T> CreateQuery<T>() where T : class
        {
            return new LinqToNHibernateQueryableContext<T>(this.context);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}
