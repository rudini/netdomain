//-------------------------------------------------------------------------------
// <copyright file="QueryableContextExtensions.cs" company="bbv Software Services AG">
//   Copyright (c) 2011 Roger Rudin, bbv Software Services AG
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
//
// </copyright>
//------------------------------------------------------------------------------

namespace netdomain.LinqToSql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using netdomain.Abstract;

    /// <summary>
    /// Implements an extension to use the Include method with lambda expressions
    /// </summary>
    public static class QueryableContextExtensions
    {
        /// <summary>
        /// Extends the Include method to use with lambda expressions.
        /// </summary>
        /// <typeparam name="T">The type of the queryable context.</typeparam>
        /// <param name="queryableContext">The queryable context.</param>
        /// <param name="selector">The selector.</param>
        /// <returns>the queryable context.</returns>
        public static IQueryableContext<T> Include<T>(this IQueryableContext<T> queryableContext, Expression<Func<T, object>> selector) where T : class
        {
            var specificQueriableType = queryableContext as LinqToSqlQueryableContext<T>;

            if (specificQueriableType != null)
            {
                new PropertyAccessorVisitor()
                    .GetPropertyAccessorExpressions(selector)
                    .ToList()
                    .CatchedForEach(specificQueriableType.LoadWith);
            }

            return queryableContext;
        }

        /// <summary>
        /// Enumerate through the elements in a catch block.
        /// </summary>
        /// <typeparam name="T">the type of the list.</typeparam>
        /// <param name="list">The list to enumerate.</param>
        /// <param name="action">The action.</param>
        public static void CatchedForEach<T>(this List<T> list, Action<T> action)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            foreach (var t in list)
            {
                try
                {
                    action(t);
                }
                catch
                {
                    // do nothing
                }
            }
        }
    }
}