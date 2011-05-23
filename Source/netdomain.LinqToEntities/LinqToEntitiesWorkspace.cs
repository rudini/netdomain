//-------------------------------------------------------------------------------
// <copyright file="LinqToEntitiesWorkspace.cs" company="bbv Software Services AG">
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
    using System.Data;
    using System.Data.EntityClient;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    using Abstract;
    using Workspace;
    using EntityState = System.Data.EntityState;

    /// <summary>
    /// Implements a unit of work for the ADO .Net object/relational mapping framework 
    /// a.k.a. EntityFramework.
    /// </summary>
    public class LinqToEntitiesWorkspace : IWorkspace<ObjectContext>, IConnectionManager
    {
        /// <summary>
        /// Holds the <see cref="ObjectContext"/> of the Entity framework.
        /// </summary>
        private readonly ObjectContext context;

        /// <summary>
        /// The extension registered to the workspace.
        /// </summary>
        private readonly List<IWorkspaceExtension> extensions = new List<IWorkspaceExtension>();

        /// <summary>
        /// Holds the delegate to the ClearCache method on the <see cref="T:netdomain.LinqToEntities.ObjectContextExtender"/>
        /// </summary>
        private Action clearCacheDelegate;

        /// <summary>
        /// Holds the delegate to the EnsureConnection method on the <see cref="T:netdomain.LinqToEntities.ObjectContextExtender"/>
        /// </summary>
        private Action ensureConnectionDelegate;

        /// <summary>
        /// Holds the delegate to the ReleaseConnection method on the <see cref="T:netdomain.LinqToEntities.ObjectContextExtender"/>
        /// </summary>
        private Action releaseConnectionDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinqToEntitiesWorkspace"/> class.
        /// </summary>
        /// <param name="objectContext">The object context.</param>
        public LinqToEntitiesWorkspace(ObjectContext objectContext)
        {
            if (objectContext == null)
            {
                throw new ArgumentNullException("objectContext");
            }
            
            this.context = objectContext;
            this.context.ObjectStateManager.ObjectStateManagerChanged += this.OnObjectStateManagerChanged;

            WorkspaceBuilder.Current.GetExtensionInstances().ToList().ForEach(ex => { ex.Workspace = this; this.extensions.Add(ex); });
        }

        /// <summary>
        /// Gets the wrapped instance.
        /// </summary>
        /// <value>The wrapped instance.</value>
        public ObjectContext WrappedInstance
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
        /// Gets the underlying ADO .Net database connection of the workspace.
        /// </summary>
        /// <value>
        /// The database connection as a <see cref="T:System.Data.IDbConnection"/>.
        /// </value>
        public IDbConnection Connection
        {
            get { return ((EntityConnection)this.context.Connection).StoreConnection; }
        }

        /// <summary>
        /// Gets a value indicating whether this workspace is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get { return this.Connection.State == ConnectionState.Open; }
        }

        /// <summary>
        /// Disconnects this instance from the current ADO .Net connection.
        /// </summary>
        /// <returns>The current ADO .Net connection.</returns>
        public IDbConnection Disconnect()
        {
            if (this.IsConnected)
            {
                if (this.releaseConnectionDelegate == null)
                {
                    this.releaseConnectionDelegate = this.context.CreateReleaseConnectionDelegate();
                }

                this.releaseConnectionDelegate();
            }

            return this.Connection;
        }

        /// <summary>
        /// Optain a new ADO .Net connection
        /// </summary>
        /// <returns>The current ADO .Net connection.</returns>
        public IDbConnection Reconnect()
        {
            if (!this.IsConnected)
            {
                if (this.ensureConnectionDelegate == null)
                {
                    this.ensureConnectionDelegate = this.context.CreateEnsureConnectionDelegate();
                }

                this.ensureConnectionDelegate();
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
                    deletedEntities = this.context.GetEntitiesFromObjectStateManager(EntityState.Deleted);
                }

                if (addedEntities == null)
                {
                    addedEntities = this.context.GetEntitiesFromObjectStateManager(EntityState.Added);
                }

                if (modifiedEntities == null)
                {
                    modifiedEntities = this.context.GetEntitiesFromObjectStateManager(EntityState.Modified);
                }

                extension.OnSubmittingChanges(deletedEntities, addedEntities, modifiedEntities);
            }

            try
            {
                this.context.SaveChanges();
            }
            catch (OptimisticConcurrencyException e)
            {
                var exception = new OptimisticOfflineLockException(e.Message, e, e.StateEntries.Select(se => new ConflictedObject(se.Entity, se.Entity.GetType())).ToArray());
                this.extensions.ForEach(ex => ex.OnOptimisticOfflineLockExceptionThrown(exception));

                throw exception;
            }
        }

        /// <summary>
        /// Return the persistent instance of the given named entity with the given identifier,
        /// or null if there is no such persistent instance. (If the instance, or a proxy for the
        /// instance, is already associated with the session, return that instance or proxy)
        /// </summary>
        /// <typeparam name="T">the type of the entity.</typeparam>
        /// <param name="entityWithKey">an entity with a key.</param>
        /// <returns>a persistent instance or null.</returns>
        /// <exception cref="ArgumentException">if the argument does not implement the interface <see cref="IEntityWithKey"></see>.</exception>
        public T Get<T>(T entityWithKey) where T : class
        {
            var key = this.context.CreateEntityKey(this.context.GetEntitySetName<T>(), entityWithKey);

            // Get the original item based on the entity key from the context
            // or from the database.
            return this.context.GetObjectByKey(key) as T;
        }

        /// <summary>
        /// Save an entity in a pending insert state to the <see cref="IWorkspace"/> context.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">A transient instance of a persistent class.</param>
        public void Add<T>(T entity) where T : class
        {
            if (entity.GetType().IsAssignableFrom(typeof(EntityObject)))
            {
                throw new ArgumentException("The argument does not inherit from EntityObject", "entity");
            }

            this.context.AddObject(this.context.GetEntitySetName<T>(), entity);

            this.extensions.ForEach(e => e.OnEntityAdded(entity));
        }

        /// <summary>
        /// Remove an entity from this table into a pending delete state.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The instance to be removed.</param>
        public void Delete<T>(T entity) where T : class
        {
            if (entity.GetType().IsAssignableFrom(typeof(EntityObject)))
            {
                throw new ArgumentException("The argument does not inherit from EntityObject", "entity");
            }

            this.context.DeleteObject(entity);

            this.extensions.ForEach(e => e.OnEntityDeleted(entity));
        }

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
        public void Update<T>(T entity) where T : class
        {
            if (entity.GetType().IsAssignableFrom(typeof(EntityObject)))
            {
                throw new ArgumentException("The argument does not inherit from EntityObject", "entity");
            }

            var key = this.context.CreateEntityKey(this.context.GetEntitySetName<T>(), entity);
            object originalItem;

            // Get the original item based on the entity key from the context
            // or from the database.
            if (this.context.TryGetObjectByKey(key, out originalItem))
            {
                // Call the ApplyPropertyChanges method to apply changes
                // from the updated item to the original version.
                this.context.ApplyCurrentValues(key.EntitySetName, entity);
            }
            else
            {
                var exception = new ObjectNotFoundException("Original object cold not been found on ObjectContext and database.");
                this.extensions.ForEach(ex => ex.OnExceptionThrown(exception));

                throw exception;
            }

            this.extensions.ForEach(e => e.OnEntityUpdated(entity));
        }

        /// <summary>
        /// Gets a boolean value indicating whether pending changes exist in the workspace.
        /// </summary>
        /// <returns>
        /// True if there are pending changes, otherwise false.
        /// </returns>
        public bool IsDirty()
        {
            return this.context.ObjectStateManager.GetObjectStateEntries(
                EntityState.Added | EntityState.Modified | EntityState.Deleted).Count() > 0;
        }

        /// <summary>
        /// Cleans all cached objects within the <see cref="IWorkspace"/>.
        /// </summary>
        public void Clean()
        {
            if (this.clearCacheDelegate == null)
            {
                this.clearCacheDelegate = this.context.CreateClearCacheDelegate();
            }

            this.clearCacheDelegate();
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
            this.context.Refresh(RefreshMode.StoreWins, entity);
            this.extensions.ForEach(e => e.OnEntityRefreshed(entity));
        }

        /// <summary>
        /// Attach an object or object graph to the object context if the object
        /// has an entity key.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entityWithKey">The entity.</param>
        public void Attach<T>(T entityWithKey) where T : class
        {
            this.context.AttachTo(this.context.GetEntitySetName<T>(), entityWithKey);
            this.extensions.ForEach(e => e.OnEntityAttached(entityWithKey));
        }

        /// <summary>
        /// Removes the specified object from the <see cref="IWorkspace"/> context.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public void Detach<T>(T entity) where T : class
        {
            if (entity.GetType().IsAssignableFrom(typeof(EntityObject)))
            {
                throw new ArgumentException("The argument does not inherit from EntityObject", "entity");
            }

            this.context.Detach(entity);
            this.extensions.ForEach(e => e.OnEntityDetached(entity));
        }

        /// <summary>
        /// Creates an <see cref="T:netdomain.Abstract.IQueryableContext`1"/> in the current object context
        /// </summary>
        /// <typeparam name="T">The type of the IQueryableContext.</typeparam>
        /// <returns>An <see cref="T:netdomain.Abstract.IQueryableContext`1"/> of the specified type.</returns>
        public IQueryableContext<T> CreateQuery<T>() where T : class
        {
            return new LinqToEntitiesQueryableContext<T>(this.context);
        }

        /// <summary>
        /// Execute the sequence returning query against the database server.
        /// The query is specified using the server's native query language, such as SQL.
        /// </summary>
        /// <typeparam name="T">The element type of the result sequence.</typeparam> 
        /// <param name="commandText">The query specified in the server's native query language.</param>
        /// <param name="parameters">The parameter values to use for the query.</param> 
        /// <returns>An IEnumerable sequence of objects.</returns> 
        public IEnumerable<T> CreateSqlQuery<T>(string commandText, params object[] parameters) where T : class
        {
            return this.context.ExecuteStoreQuery<T>(commandText, parameters);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.context.Dispose();
        }

        /// <summary>
        /// Handles the ObjectStateManagerChanged event of the ObjectStateManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CollectionChangeEventArgs"/> instance containing the event data.</param>
        private void OnObjectStateManagerChanged(object sender, System.ComponentModel.CollectionChangeEventArgs e)
        {
            if (e.Action == System.ComponentModel.CollectionChangeAction.Add)
            {
                this.GenerateId(e.Element);
            }
        }
    }
}
