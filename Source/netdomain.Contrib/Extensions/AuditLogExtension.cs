//-------------------------------------------------------------------------------
// <copyright file="LogExtension.cs" company="bbv Software Services AG">
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

namespace netdomain.Contrib.Extensions
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Abstract;

    /// <summary>
    /// Implements an extension to log audit messages.
    /// </summary>
    public abstract class AuditLogExtension : WorkspaceExtension
    {
        /// <summary>
        /// Called before all changes have been submitted.
        /// </summary>
        /// <param name="deletedEntities">The deleted entities.</param>
        /// <param name="addedEntities">The added entities.</param>
        /// <param name="modifiedEntities">The modified entities.</param>
        public sealed override void OnSubmittingChanges(IEnumerable<object> deletedEntities, IEnumerable<object> addedEntities, IEnumerable<object> modifiedEntities)
        {
            foreach (IAuditable deletedEntity in deletedEntities.OfType<IAuditable>())
            {
                this.LogAudit(deletedEntity, AuditAction.Delete, null, null, null);
            }

            foreach (IAuditable addedEntity in addedEntities.OfType<IAuditable>())
            {
                this.LogAudit(addedEntity, AuditAction.Insert, null, null, null);
            }

            foreach (IAuditable modifiedEntity in modifiedEntities.OfType<IAuditable>())
            {
                IEnumerable<ChangedProperty> changedProperties = this.GetChangedProperties(modifiedEntity);
                changedProperties.ToList().ForEach(p => this.LogAudit(modifiedEntity, AuditAction.Update, p.Name, p.NewValue.ToString(), p.OldValue.ToString()));
            }
        }

        /// <summary>
        /// Called after an entity has been added. 
        /// If you are using database generated identifier for some entities, 
        /// override this method and call the base method with the filtered entities using db generated idenitifier.
        /// If you don't use any db generated identifier, override this method with an empty implementation.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public override void OnEntityAdded<T>(T entity)
        {
            if (typeof(IAuditable).IsAssignableFrom(entity.GetType()))
            {
                this.LogAudit((IAuditable)entity, AuditAction.Insert, null, null, null);
            }
        }

        /// <summary>
        /// Called after an entity has been deleted.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public sealed override void OnEntityDeleted<T>(T entity)
        {
        }

        /// <summary>
        /// Called after attaching an entity from context.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public sealed override void OnEntityAttached<T>(T entity)
        {
        }

        /// <summary>
        /// Called after detaching an entity from context.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public sealed override void OnEntityDetached<T>(T entity)
        {
        }

        /// <summary>
        /// Called after an entity has been refreshed from the store.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public sealed override void OnEntityRefreshed<T>(T entity)
        {
        }

        /// <summary>
        /// Called after an entity has been updated.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public sealed override void OnEntityUpdated<T>(T entity)
        {
        }

        /// <summary>
        /// Called after the cache has been cleaned up.
        /// </summary>
        public sealed override void OnCacheCleaned()
        {
        }

        /// <summary>
        /// Called after an exception has occurred.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public sealed override void OnExceptionThrown(System.Exception exception)
        {
        }

        /// <summary>
        /// Called after an optimistic offline lock exception has ocurred.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public sealed override void OnOptimisticOfflineLockExceptionThrown(OptimisticOfflineLockException exception)
        {
        }

        /// <summary>
        /// Logs the audit. Override this method in your derived class to store the audit log entry.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="auditAction">Type of the operation.</param>
        /// <param name="property">The property.</param>
        /// <param name="propertyNewValue">The property new value.</param>
        /// <param name="propertyOldValue">The property old value.</param>
        public abstract void LogAudit(IAuditable entity, AuditAction auditAction, string property, string propertyNewValue, string propertyOldValue);

        /// <summary>
        /// Gets the changed properties.
        /// </summary>
        /// <param name="modifiedEntity">The modified entity.</param>
        /// <returns>An enumerable of type <see cref="T:netdomain.Contrib.Extensions.AuditLogExtension.ChangedProperty"/></returns>
        private IEnumerable<ChangedProperty> GetChangedProperties(object modifiedEntity)
        {
            object original = WorkspaceBuilder.Current.GetWorkspaceFactory().GetWorkspaceInstance().Get(modifiedEntity);

            foreach (var propertyInfo in modifiedEntity.GetType().GetProperties())
            {
                if (!typeof(ICollection).IsAssignableFrom(propertyInfo.PropertyType) 
                    /*&& (propertyInfo.PropertyType.IsPrimitive
                    || typeof(string).IsAssignableFrom(propertyInfo.PropertyType))*/)
                {
                    var newValue = this.GetValue(modifiedEntity, propertyInfo) ?? "null";
                    var oldValue = this.GetValue(original, propertyInfo) ?? "null";

                    if (!newValue.Equals(oldValue))
                    {
                        yield return new ChangedProperty(propertyInfo.Name, newValue, oldValue);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns>
        /// If the property is a primitive type or a string, the value of the property is returned.
        /// If the property is a reference type and is assignable to IAuditable, the Typename and the id is returned.
        /// If the property is a null reference, a "null" string is returned.
        /// </returns>
        private object GetValue(object entity, PropertyInfo propertyInfo)
        {
            var value = propertyInfo.GetValue(entity, null);

            if (value != null && typeof(IAuditable).IsAssignableFrom(value.GetType()))
            {
                value = value.GetType().Name + ":" + ((IAuditable)value).GetId();
            }

            return value;
        }

        /// <summary>
        /// Implements a changed property container.
        /// </summary>
        internal struct ChangedProperty
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ChangedProperty"/> struct.
            /// </summary>
            /// <param name="name">The property name.</param>
            /// <param name="newValue">The new value.</param>
            /// <param name="oldValue">The old value.</param>
            public ChangedProperty(string name, object newValue, object oldValue) : this()
            {
                this.Name = name;
                this.NewValue = newValue;
                this.OldValue = oldValue;
            }

            /// <summary>
            /// Gets the property name.
            /// </summary>
            /// <value>The property name.</value>
            public string Name { get; private set; }

            /// <summary>
            /// Gets the new value.
            /// </summary>
            /// <value>The new value.</value>
            public object NewValue { get; private set; }

            /// <summary>
            /// Gets the old value.
            /// </summary>
            /// <value>The old value.</value>
            public object OldValue { get; private set; }
        }
    }
}