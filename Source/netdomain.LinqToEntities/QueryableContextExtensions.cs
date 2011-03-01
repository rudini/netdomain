//-------------------------------------------------------------------------------
// <copyright file="QueryableContextExtensions.cs" company="bbv Software Services AG">
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
//
// Contains parts from "Thomas Levesque's .NET blog" 
// published on http://tomlev2.wordpress.com/2010/10/03/entity-framework-using-include-with-lambda-expressions/
// </copyright>
//------------------------------------------------------------------------------

namespace netdomain.LinqToEntities
{
    using System;
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
            var specificQueriableType = queryableContext as LinqToEntitiesQueryableContext<T>;
            return specificQueriableType != null ? specificQueriableType.Include(new PropertyPathVisitor().GetPropertyPath(selector)) : queryableContext;
        }
    }
}