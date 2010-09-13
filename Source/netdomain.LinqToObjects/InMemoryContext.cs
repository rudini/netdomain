//-------------------------------------------------------------------------------
// <copyright file="InMemoryContext.cs" company="bbv Software Services AG">
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
    using Abstract;
    using Helpers;

    /// <summary>
    /// Represents an in memory context.
    /// </summary>
    public class InMemoryContext
    {
        /// <summary>
        /// A Dictionary that represents an in memory database.
        /// </summary>
        private readonly IDictionary<Type, object> dataSources;

        /// <summary>
        /// The change set of the context.
        /// </summary>
        private readonly ChangeSet changeSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryContext"/> class.
        /// </summary>
        public InMemoryContext()
        {
            this.dataSources = new Dictionary<Type, object>();
            this.changeSet = new ChangeSet();
        }

        /// <summary>
        /// Gets the entities from the change manager.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>An <see cref="T:System.Collections.IEnumerable"/>.</returns>
        internal IEnumerable<object> GetEntitiesFromTrackingManager(TrackingState state)
        {
            var changedEntities = new List<object>();

            if ((state & TrackingState.Deleted) == TrackingState.Deleted)
            {
                changedEntities.AddRange(this.GetChangeSet().Deletes);
            }

            if ((state & TrackingState.Added) == TrackingState.Added)
            {
                changedEntities.AddRange(this.GetChangeSet().Inserts);
            }

            if ((state & TrackingState.Modified) == TrackingState.Modified)
            {
                changedEntities.AddRange(this.GetChangeSet().Updates);
            }

            return changedEntities.
                Where(
                    entity =>
                    entity != null);
        }

        /// <summary>
        /// Gets all modified entities without the deleted ones.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> of objects.
        /// </returns>
        internal IEnumerable<object> GetNewOrModifiedEntities()
        {
            var changedEntities = new List<object>();
            changedEntities.AddRange(this.GetChangeSet().Inserts);
            changedEntities.AddRange(this.GetChangeSet().Updates);

            return changedEntities.
                Where(
                entity =>
                entity != null);
        }

        /// <summary>
        /// Creates a query of a particular type.
        /// </summary>
        /// <typeparam name="T">The desired type of the query.</typeparam>
        /// <returns>A new <see cref="T:netdomain.Abstract.IQueryableContext`1"/>.</returns>
        internal IQueryableContext<T> CreateQuery<T>() where T : class
        {
            return GetInMemoryQueryableContext<T>();
        }

        /// <summary>
        /// Gets a table of a particular type.
        /// </summary>
        /// <typeparam name="T">The type of the table.</typeparam>
        /// <returns>The table as <see cref="T:netdomain.Abstract.ITable`1"/></returns>
        internal ITable<T> GetTable<T>() where T : class
        {
            return GetInMemoryQueryableContext<T>();
        }

        /// <summary>
        /// Gets the change set.
        /// </summary>
        /// <returns>An instance of a <see cref="T:netdomain.LinqToObjects.InMemoryContext.ChangeSet"></see></returns>
        internal ChangeSet GetChangeSet()
        {
            return this.changeSet;
        }

        /// <summary>
        /// Cleans the change set.
        /// </summary>
        internal void Clean()
        {
            this.changeSet.Clear();
        }

        /// <summary>
        /// Submit all changes of the change set to the in memory tables.
        /// </summary>
        internal void SubmitChanges()
        {
            foreach (var entity in this.changeSet.Deletes)
            {
                ReflectionHelper.CallPublicInstanceMethod("Delete", ReflectionHelper.CallNoPublicGenericInstanceMethod("GetTable", this, entity.GetType(), null), entity);
            }

            foreach (var entity in this.changeSet.Inserts)
            {
                ReflectionHelper.CallPublicInstanceMethod("Insert", ReflectionHelper.CallNoPublicGenericInstanceMethod("GetTable", this, entity.GetType(), null), entity);
            }

            foreach (var entity in this.changeSet.Updates)
            {
                ReflectionHelper.CallPublicInstanceMethod("Update", ReflectionHelper.CallNoPublicGenericInstanceMethod("GetTable", this, entity.GetType(), null), entity);
            }

            this.Clean();
        }

        /// <summary>
        /// Adds an entity to the change tracking.
        /// </summary>
        /// <param name="notifyPropertyChanged">The notify property changed.</param>
        internal void AddToChangeTracking(INotifyPropertyChanged notifyPropertyChanged)
        {
            notifyPropertyChanged.PropertyChanged += this.OnEntityChanged;
        }

        /// <summary>
        /// Removes from change tracking.
        /// </summary>
        /// <param name="notifyPropertyChanged">The notify property changed.</param>
        internal void RemoveFromChangeTracking(INotifyPropertyChanged notifyPropertyChanged)
        {
            notifyPropertyChanged.PropertyChanged -= this.OnEntityChanged;
        }

        /// <summary>
        /// Gets the in memory queryable context.
        /// </summary>
        /// <typeparam name="T">The type of the desired queryable context.</typeparam>
        /// <returns>An <see cref="T:netdomain.LinqToObjects.InMemoryQueryableContext`1"/>.</returns>
        private InMemoryQueryableContext<T> GetInMemoryQueryableContext<T>() where T : class
        {
            if (!this.dataSources.ContainsKey(typeof(T)))
            {
                this.dataSources.Add(typeof(T), new InMemoryQueryableContext<T>(this));
            }

            return this.dataSources[typeof(T)] as InMemoryQueryableContext<T>;
        }

        /// <summary>
        /// Called when [entity changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnEntityChanged(object sender, PropertyChangedEventArgs e)
        {
            this.GetChangeSet().Updates.Add(sender);
        }

        /// <summary>
        /// Implements a change set class to hold the changes made on an InMemoryContext.
        /// </summary>
        internal class ChangeSet
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ChangeSet"/> class.
            /// </summary>
            internal ChangeSet()
            {
                this.Deletes = new List<object>();
                this.Inserts = new List<object>();
                this.Updates = new List<object>();
            }

            /// <summary>
            /// Gets the deleted items of the context.
            /// </summary>
            /// <value>The deletes.</value>
            internal List<object> Deletes { get; private set; }

            /// <summary>
            /// Gets the inserted items of the context.
            /// </summary>
            /// <value>The inserts.</value>
            internal List<object> Inserts { get; private set; }

            /// <summary>
            /// Gets the updated items of the context.
            /// </summary>
            /// <value>The updates.</value>
            internal List<object> Updates { get; private set; }

            /// <summary>
            /// Clears this instance.
            /// </summary>
            internal void Clear()
            {
                this.Deletes.Clear();
                this.Inserts.Clear();
                this.Updates.Clear();
            }
        }
    }
}
