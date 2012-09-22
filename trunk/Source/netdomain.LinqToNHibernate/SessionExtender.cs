//-------------------------------------------------------------------------------
// <copyright file="SessionExtender.cs" company="bbv Software Services AG">
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
    using System.Linq;
    using netdomain.Abstract;
    using netdomain.Helpers;
    using NHibernate;
    using NHibernate.Action;
    using NHibernate.Engine;
    using NHibernate.Event;
    using NHibernate.Impl;

    /// <summary>
    /// Helper class to extend missing functionality of the <see cref="ISession"/>.
    /// </summary>
    public static class SessionExtender
    {
        /// <summary>
        /// Gets the entities from the change manager.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="state">The state.</param>
        /// <returns>An <see cref="T:System.Collections.IEnumerable"/>.</returns>
        [Obsolete("Do not use this method anymore. Use the EventListener mechanism of the SessionFactory configuration.")]
        internal static IEnumerable<object> GetEntitiesFromActionQueues(this ISession context, TrackingState state)
        {
            var sessionImpl = (SessionImpl)context;
            ActionQueue actionQueue = sessionImpl.ActionQueue;
            int collectionRemovalsCount = actionQueue.CollectionRemovalsCount;

            var dirtyCheckEvent = new DirtyCheckEvent(sessionImpl);
            foreach (var listener in sessionImpl.Listeners.DirtyCheckEventListeners)
            {
                ReflectionHelper.CallNoPublicInstanceMethod("FlushEverythingToExecutions", listener, dirtyCheckEvent);
            }

            var updates = (List<EntityUpdateAction>)ReflectionHelper.GetNoPublicInstanceField("updates", actionQueue);
            var insertions = (List<IExecutable>)ReflectionHelper.GetNoPublicInstanceField("insertions", actionQueue);
            var deletions = (List<EntityDeleteAction>)ReflectionHelper.GetNoPublicInstanceField("deletions", actionQueue);

            var changedEntities = new List<object>();

            if ((state & TrackingState.Deleted) == TrackingState.Deleted)
            {
                changedEntities.AddRange(deletions.Select(action => action.Instance));
            }

            if ((state & TrackingState.Added) == TrackingState.Added)
            {
                changedEntities.AddRange(from EntityAction action in insertions select action.Instance);
            }

            if ((state & TrackingState.Modified) == TrackingState.Modified)
            {
                changedEntities.AddRange(updates.Select(action => action.Instance));
            }

            actionQueue.ClearFromFlushNeededCheck(collectionRemovalsCount);

            return changedEntities.Where(
                entity =>
                entity != null);
        }
    }
}
