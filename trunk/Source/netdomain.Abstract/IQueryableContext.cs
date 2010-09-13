//-------------------------------------------------------------------------------
// <copyright file="IQueryableContext.cs" company="bbv Software Services AG">
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
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Defines an interface of a querable context of a particular type.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="T:netdomain.Abstract.IQueryableContext`1"/></typeparam>
    public interface IQueryableContext<T> : IQueryable<T>
    {
        /// <summary>
        /// Specifies the related objects to include in the query results.
        /// </summary>
        /// <param name="path">Dot-separated list of related objects to return in the query results.</param>
        /// <returns>A new <see cref="T:netdomain.Abstract.IQueryableContext`1"/> with the defined query path.</returns>
        IQueryableContext<T> Include(string path);

        /// <summary>
        /// Specifies which sub-objects to retrieve when a query is submitted for an object of type T.
        /// </summary>
        /// <param name="expression">Identifies the field or property to be retrieved.
        /// If the expression does not identify a field or property that represents a one-to-one or one-to-many relationship, an exception is thrown.</param>
        /// <returns>A new <see cref="T:netdomain.Abstract.IQueryableContext`1"/> with the defined query path.</returns>
        /// <remarks>
        /// You cannot specify the loading of two levels of relationships (for example, Orders.OrderDetails). In these scenarios you must specify two separate Include methods.
        /// </remarks>
        IQueryableContext<T> Include(Expression<Func<T, object>> expression);
    }
}
