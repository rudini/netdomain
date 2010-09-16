//-------------------------------------------------------------------------------
// <copyright file="InMemoryWorkspace.cs" company="bbv Software Services AG">
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
    using System.Linq;
    using Abstract;

    /// <summary>
    /// Implements a unit of work as a in memory data source.
    /// This class is useful for unit tests.
    /// </summary>
    public class InMemoryWorkspace : IWorkspace<InMemoryContext>
    {
        /// <summary>
        /// Holds a reference to the <see cref="InMemoryContext"/>.
        /// </summary>
        private readonly InMemoryContext context;

        /// <summary>
        /// The extension registered to the workspace.
        /// </summary>
        private readonly List<IWorkspaceExtension> extensions = new List<IWorkspaceExtension>();

        /// <summary>
        /// Holds the validator to validate the entities.
        /// </summary>
        private readonly IValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryWorkspace"/> class.
        /// </summary>
        public InMemoryWorkspace()
        {
            this.context = new InMemoryContext();
            WorkspaceBuilder.Current.GetExtensionInstances().ToList().ForEach(ex => { ex.Workspace = this; this.extensions.Add(ex); });
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryWorkspace"/> class.
        /// </summary>
        /// <param name="validator">The validator.</param>
        [Obsolete("Implement a WorkspaceExtension for validation purposes instead of using the IValidator parameter.")]
        public InMemoryWorkspace(IValidator validator) : this()
        {
            this.validator = validator;
        }

        /// <summary>
        /// Gets the wrapped instance.
        /// </summary>
        /// <value>The wrapped instance.</value>
        public InMemoryContext WrappedInstance
        {
            get { return this.context; }
        }

        /// <summary>
        /// Gets the connection manager.
        /// </summary>
        /// <value>The connection manager.</value>
        public virtual IConnectionManager ConnectionManager
        {
            get { throw new NotSupportedException("Connection property is not supported by the InMemoryUnitOfWork"); }
        }

        /// <summary>
        /// Gets a boolean value indicating whether pending changes exist in the workspace.
        /// </summary>
        /// <returns>
        /// True if there are pending changes, otherwise false.
        /// </returns>
        public bool IsDirty()
        {
            var changeSet = this.context.GetChangeSet();
            return changeSet.Inserts.Count > 0 || changeSet.Updates.Count > 0 || changeSet.Deletes.Count > 0;
        }

        /// <summary>
        /// Creates an <see cref="T:netdomain.Abstract.IQueryableContext`1"/> in the current object context
        /// </summary>
        /// <typeparam name="T">The type of the queryable context.</typeparam>
        /// <returns>An <see cref="T:netdomain.Abstract.IQueryableContext`1"/> of the specified type.</returns>
        public IQueryableContext<T> CreateQuery<T>() where T : class
        {
            return this.context.CreateQuery<T>();
        }

        /// <summary>
        /// Persists all pending updates to the data source.
        /// </summary>
        public void SubmitChanges()
        {
            IEnumerable<object> deletedEntities, addedEntities, modifiedEntities;
            deletedEntities = addedEntities = modifiedEntities = null;

            foreach (var extension in this.extensions)
            {
                if (deletedEntities == null)
                {
                    deletedEntities = this.context.GetEntitiesFromTrackingManager(TrackingState.Deleted);
                }

                if (addedEntities == null)
                {
                    addedEntities = this.context.GetEntitiesFromTrackingManager(TrackingState.Added);
                }

                if (modifiedEntities == null)
                {
                    modifiedEntities = this.context.GetEntitiesFromTrackingManager(TrackingState.Modified);
                }

                extension.OnSubmittingChanges(deletedEntities, addedEntities, modifiedEntities);
            }

            if (this.validator != null)
            {
                // get all modified entities which implement the IValidatable interface
                IEnumerable<object> validatableEntities = this.context.GetEntitiesFromTrackingManager(TrackingState.Added | TrackingState.Modified);

                // hocks in validation code
                this.validator.ValidateEntities(validatableEntities);
            }

            this.context.SubmitChanges();
        }

        /// <summary>
        /// Return the persistent instance of the given named entity with the given identifier,
        /// or null if there is no such persistent instance. (If the instance, or a proxy for the
        /// instance, is already associated with the session, return that instance or proxy.)
        /// </summary>
        /// <typeparam name="T">the type of the entity</typeparam>
        /// <param name="entityWithKey">an entity with a key</param>
        /// <returns>a persistent instance or null</returns>
        public T Get<T>(T entityWithKey) where T : class
        {
            throw new NotSupportedException("The method Get is not supported by the InMemoryUnitOfWork");
        }

        /// <summary>
        /// Inserts an entity in a pending insert state to the <see cref="IWorkspace"/> context.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">A transient instance of a persistent class.</param>
        public void Add<T>(T entity) where T : class
        {
            this.context.GetTable<T>().InsertOnSubmit(entity);
            this.extensions.ForEach(e => e.OnEntityAdded(entity));
        }

        /// <summary>
        /// Remove an entity from this table into a pending delete state.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The instance to be removed.</param>
        public void Delete<T>(T entity) where T : class
        {
            this.context.GetTable<T>().DeleteOnSubmit(entity);
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
            this.context.GetTable<T>().UpdateOnSubmit(entity);
            this.extensions.ForEach(e => e.OnEntityUpdated(entity));
        }

        /// <summary>
        /// Cleans all caches of the <see cref="IWorkspace"/>
        /// and removes all objects from the identity map.
        /// </summary>
        public void Clean()
        {
            this.context.Clean();
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
            throw new NotSupportedException("The method Refresh is not supported by the InMemoryUnitOfWork");
        }

        /// <summary>
        /// Attach an object or object graph to the object context if the object
        /// has an entity key.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entityWithKey">The entity.</param>
        public void Attach<T>(T entityWithKey) where T : class
        {
            this.context.GetTable<T>().Attach(entityWithKey);
            this.extensions.ForEach(e => e.OnEntityAttached(entityWithKey));
        }

        /// <summary>
        /// Removes the specified object from the <see cref="IWorkspace"/> context.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public void Detach<T>(T entity) where T : class
        {
            // not used by the InMemoryUnitOfWork
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // no unmanaged resources are used
        }
    }
}