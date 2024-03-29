//-------------------------------------------------------------------------------
// <copyright file="WorkspaceExtension.cs" company="bbv Software Services AG">
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
    using System.Collections.Generic;

    /// <summary>
    /// Interface for workspace extensions.
    /// Implements this interface if you want to implement an extension. />
    /// </summary>
    public interface IWorkspaceExtension
    {
        /// <summary>
        /// Gets or sets the workspace.
        /// </summary>
        /// <value>The workspace.</value>
        IWorkspace Workspace { get; set; }

        /// <summary>
        /// Called before all changes has been submitted.
        /// </summary>
        /// <param name="deletedEntities">The deleted entities.</param>
        /// <param name="addedEntities">The added entities.</param>
        /// <param name="modifiedEntities">The modified entities.</param>
        void OnSubmittingChanges(IEnumerable<object> deletedEntities, IEnumerable<object> addedEntities, IEnumerable<object> modifiedEntities);

        /// <summary>
        /// Called after an entitie has been added.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        void OnEntityAdded<T>(T entity) where T : class;

        /// <summary>
        /// Called after an entity has been deleted.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        void OnEntityDeleted<T>(T entity) where T : class;

        /// <summary>
        /// Called after an entity has been updated.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        void OnEntityUpdated<T>(T entity) where T : class;

        /// <summary>
        /// Called after the cache has been cleaned up.
        /// </summary>
        void OnCacheCleaned();

        /// <summary>
        /// Called after an entity has been refreshed from the store.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        void OnEntityRefreshed<T>(T entity) where T : class;

        /// <summary>
        /// Called after attaching an entity from context.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        void OnEntityAttached<T>(T entity) where T : class;

        /// <summary>
        /// Called after detaching an entity from context.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        void OnEntityDetached<T>(T entity) where T : class;

        /// <summary>
        /// Called after an optimistic offline lock exception has ocurred.
        /// </summary>
        /// <param name="exception">The exception.</param>
        void OnOptimisticOfflineLockExceptionThrown(OptimisticOfflineLockException exception);

        /// <summary>
        /// Called after an exception has occurred.
        /// </summary>
        /// <param name="exception">The exception.</param>
        void OnExceptionThrown(Exception exception);

        /// <summary>
        /// Called before a query is executing.
        /// </summary>
        /// <param name="query">The query as a string.</param>
        void OnPreQueryExecuted(string query);
    }
}