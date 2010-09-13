//-------------------------------------------------------------------------------
// <copyright file="ObjectContextExtender.cs" company="bbv Software Services AG">
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
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Globalization;
    using System.Linq;
    using Helpers;

    /// <summary>
    /// Helper class to extend missing functionality of the <see cref="ObjectContext"/>.
    /// </summary>
    public static class ObjectContextExtender
    {
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <typeparam name="T">The type of the entity to get the <see cref="T:System.Data.Metadata.Edm.EntityType"/> for.</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="dataSpace">The data space.</param>
        /// <remarks>
        /// If you are using inheritence mapping, you must use the derived type to get all properies of it.
        /// </remarks>
        /// <returns>The <see cref="T:System.Data.Metadata.Edm.EntityType"/> for the type T.</returns>
        public static EntityType GetEntityType<T>(this ObjectContext context, DataSpace dataSpace)
        {
            return (EntityType)context.MetadataWorkspace.GetType(typeof(T).Name, typeof(T).Namespace, true, dataSpace);
        }

        /// <summary>
        /// Gets the entities from the object state manager.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="state">The state.</param>
        /// <returns>An <see cref="T:System.Collections.IEnumerable"/>.</returns>
        internal static IEnumerable<object> GetEntitiesFromObjectStateManager(this ObjectContext context, EntityState state)
        {
            return context.ObjectStateManager.GetObjectStateEntries(state).
                Where(
                objectStateEntry =>
                objectStateEntry.Entity != null).
                Select(objectStateEntry => objectStateEntry.Entity);
        }

        /// <summary>
        /// Gets the name of the entity set.
        /// </summary>
        /// <typeparam name="T">The type of the entity to get the name for.</typeparam>
        /// <param name="context">The context.</param>
        /// <returns>A string containing the name of the entity set.</returns>
        /// <exception cref="NotSupportedException">Thrown, if there is no EntitySet supporting the type 'T'.</exception>
        internal static string GetEntitySetName<T>(this ObjectContext context) where T : class
        {
            var entitySet = context.GetEntitySet<T>(DataSpace.CSpace);

            return entitySet.Name;
        }

        /// <summary>
        /// Gets the entity set.
        /// </summary>
        /// <typeparam name="T">The type of the entity to get the <see cref="T:System.Data.Metadata.Edm.EntitySetBase"/> for.</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="dataSpace">The data space.</param>
        /// <returns>
        /// The <see cref="T:System.Data.Metadata.Edm.EntitySetBase"/> for the type T.
        /// </returns>
        /// <exception cref="NotSupportedException">Thrown, if there is no EntitySet supporting the type 'T'.</exception>
        internal static EntitySetBase GetEntitySet<T>(this ObjectContext context, DataSpace dataSpace)
        {
            ItemCollection col;
            if (!context.MetadataWorkspace.TryGetItemCollection(dataSpace, out col))
            {
                context.MetadataWorkspace.RegisterItemCollection(new StoreItemCollection("res://*/"));
            }

            var container = context.MetadataWorkspace.GetItems<EntityContainer>(dataSpace).Single();

            var entitySet =
                container.BaseEntitySets.Where(
                    ebs => typeof(T).IsAssignableFrom(typeof(T).Module.GetType(typeof(T).Namespace + "." + ebs.ElementType.Name))).FirstOrDefault();

            if (entitySet == null)
            {
                throw new NotSupportedException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "There is no EntitySet supporting the type {0}",
                        typeof(T)));
            }

            return entitySet;
        }

        /// <summary>
        /// Create a delegate to clear the cache of the ObjectContext.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>An delegate of type <see cref="T:System.Action"/></returns>
        internal static Action CreateClearCacheDelegate(this ObjectContext context)
        {
            var setValueDelegate = ReflectionHelper.CreateSetterDelegateOfNonPublicBaseTypeField("_cache", context);
            return () => setValueDelegate(context, null);
        }

        /// <summary>
        /// Create a delegate to the internal instance method EnsureConnection of the ObjectContext.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>An delegate of type <see cref="T:System.Action"/></returns>
        internal static Action CreateEnsureConnectionDelegate(this ObjectContext context)
        {
            return ReflectionHelper.CreateDelegateOfNonPublicInstanceMethod<Action>("EnsureConnection", context);
        }

        /// <summary>
        /// Create a delegate to the internal instance method ReleaseConnection of the ObjectContext.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>An delegate of type <see cref="T:System.Action"/></returns>
        internal static Action CreateReleaseConnectionDelegate(this ObjectContext context)
        {
            return ReflectionHelper.CreateDelegateOfNonPublicInstanceMethod<Action>("ReleaseConnection", context);
        }
    }
}
