//-------------------------------------------------------------------------------
// <copyright file="DataContextExtender.cs" company="bbv Software Services AG">
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

namespace netdomain.LinqToSql
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Linq;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Abstract;
    using Helpers;
    using log4net;
    using Specification;

    /// <summary>
    /// Helper class to extend missing functionality of the <see cref="DataContext"/>.
    /// </summary>
    public static class DataContextExtender
    {
        #region logger

        /// <summary>
        /// Type specific logger 
        /// </summary>
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        /// <summary>
        /// Gets the entities from the change manager.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="state">The state.</param>
        /// <returns>An <see cref="T:System.Collections.IEnumerable"/>.</returns>
        internal static IEnumerable<object> GetEntitiesFromTrackingManager(this DataContext context, TrackingState state)
        {
            var changedEntities = new List<object>();

            if ((state & TrackingState.Deleted) == TrackingState.Deleted)
            {
                changedEntities.AddRange(context.GetChangeSet().Deletes);
            }

            if ((state & TrackingState.Added) == TrackingState.Added)
            {
                changedEntities.AddRange(context.GetChangeSet().Inserts);
            }

            if ((state & TrackingState.Modified) == TrackingState.Modified)
            {
                changedEntities.AddRange(context.GetChangeSet().Updates);
            }

            return changedEntities.
                Where(
                entity =>
                entity != null);
        }

        /// <summary>
        /// Detaches an entity from the specified context.
        /// </summary>
        /// <typeparam name="T">The type of the entity to remove.</typeparam>
        /// <param name="context">The data context <see cref="DataContext"/>.</param>
        /// <param name="entity">The instance of the entity to remove.</param>
        internal static void Detach<T>(this DataContext context,  T entity) where T : class
        {
            StopTracking(context, entity);
            RemoveFromIdentityMap(context, entity);
        }

        /// <summary>
        /// Gets an object by an object already have a key defined.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="context">The data context.</param>
        /// <param name="entity">The entity with a key.</param>
        /// <returns>An entity of the particular type.</returns>
        /// <exception cref="ObjectNotFoundException">Thrown, if object could not be found in the identity manager and the database.</exception>
        internal static T GetObjectByKey<T>(this DataContext context, T entity) where T : class
        {
            // try first to find the entity in the identity manager
            var foundEntity = GetFromIdentityMap(context, entity);

            if (foundEntity != null)
            {
                return foundEntity;
            }

            // try to find the entity in the database
            foundEntity = GetObjectByPrimaryKey(context, entity);

            if (foundEntity == null)
            {
                throw new ObjectNotFoundException("Object could not be found in the identity manager and the database.");
            }

            return foundEntity;
        }

        /// <summary>
        /// Create a delegate to clear the cache of the DataContext.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>An delegate of type <see cref="T:System.Action"/></returns>
        internal static Action CreateClearCacheDelegate(this DataContext context)
        {
            return ReflectionHelper.CreateDelegateOfNonPublicInstanceMethod<Action>("ClearCache", context);
        }

        /// <summary>
        /// Creates the query by primary key.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>An entity of the particular type.</returns>
        /// <exception cref="ObjectNotFoundException">Thrown, if object does not have a key.</exception>
        private static T GetObjectByPrimaryKey<T>(DataContext context, T entity) where T : class
        {
            var primaryKeyMetaDataMember =
                context.Mapping.GetMetaType(typeof(T)).IdentityMembers.Where(mdm => mdm.IsPrimaryKey);

            if (primaryKeyMetaDataMember.Count() < 1)
            {
                throw new ObjectNotFoundException("Object does not have a key.");
            }

            Expression<Func<T, bool>> primaryKeysSpecification = null;

            foreach (var mdm in primaryKeyMetaDataMember)
            {
                // left expression
                var paramLeft = Expression.Parameter(typeof(T), "t");
                var propertyLeft = Expression.Property(paramLeft, mdm.Name);

                // right expression
                var constRight = Expression.Constant(entity);
                var propertyRight = Expression.Property(constRight, mdm.Name);

                var binaryBody = Expression.Equal(propertyLeft, propertyRight);

                Expression<Func<T, bool>> expression = Expression.Lambda<Func<T, bool>>(binaryBody, paramLeft);

                primaryKeysSpecification = primaryKeysSpecification == null ? expression : primaryKeysSpecification.And(expression);
            }

            IQueryable<T> query = context.GetTable<T>();
            return primaryKeysSpecification != null ? query.Where(primaryKeysSpecification).FirstOrDefault() : default(T);
        }

        /// <summary>
        /// Removes an entity from the identity map.
        /// </summary>
        /// <typeparam name="T">The type of the entity to remove.</typeparam>
        /// <param name="context">The data context <see cref="DataContext"/>.</param>
        /// <param name="entity">The instance of the entity to remove.</param>
        private static void RemoveFromIdentityMap<T>(DataContext context, T entity) where T : class
        {
            var commonDataService = ReflectionHelper.GetNoPublicInstanceProperty("Services", context);
            var identityManager = ReflectionHelper.GetNoPublicInstanceProperty("IdentityManager", commonDataService);
            var caches = ReflectionHelper.GetNoPublicInstanceField("caches", identityManager);
            var identityCache = ReflectionHelper.CallPublicInstanceMethod("get_Item", caches, context.Mapping.GetMetaType(typeof(T)));
            ReflectionHelper.CallNoPublicInstanceMethod("RemoveLike", identityCache, entity);
        }

        /// <summary>
        /// Find an entity in the identity map.
        /// </summary>
        /// <typeparam name="T">The type of the entity to find.</typeparam>
        /// <param name="context">The data context <see cref="DataContext"/>.</param>
        /// <param name="entity">The instance of the entity to find.</param>
        /// <returns>An entity of the particular type.</returns>
        private static T GetFromIdentityMap<T>(DataContext context, T entity) where T : class
        {
            try
            {
                var commonDataService = ReflectionHelper.GetNoPublicInstanceProperty("Services", context);
                var identityManager = ReflectionHelper.GetNoPublicInstanceProperty("IdentityManager", commonDataService);
                var caches = ReflectionHelper.GetNoPublicInstanceField("caches", identityManager);
                var identityCache = ReflectionHelper.CallPublicInstanceMethod("get_Item", caches, context.Mapping.GetMetaType(typeof(T)));
                return ReflectionHelper.CallNoPublicInstanceMethod("FindLike", identityCache, entity) as T;
            }
            catch (TargetInvocationException e)
            {
                Log.Debug("Object not found in identity map.", e);
            }

            return default(T);
        }

        /// <summary>
        /// Stops the tracking of an entity.
        /// </summary>
        /// <typeparam name="T">The type of the entity to stop the tracking.</typeparam>
        /// <param name="context">The data context <see cref="DataContext"/>.</param>
        /// <param name="entity">The instance to stop the tracking for.</param>
        private static void StopTracking<T>(DataContext context, T entity) where T : class
        {
            var commonDataService = ReflectionHelper.GetNoPublicInstanceProperty("Services", context);
            var changeTracker = ReflectionHelper.GetNoPublicInstanceProperty("ChangeTracker", commonDataService);
            ReflectionHelper.CallNoPublicInstanceMethod("StopTracking", changeTracker, entity);
        }
    }
}
