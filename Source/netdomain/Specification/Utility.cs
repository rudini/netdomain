//-------------------------------------------------------------------------------
// <copyright file="Utility.cs" company="bbv Software Services AG">
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
// Contains parts from "LINQ to Entities: Combining Predicates" of Colin Meek 
// published on 
// http://blogs.msdn.com/meek/archive/2008/05/02/linq-to-entities-combining-predicates.aspx.
// </copyright>
//------------------------------------------------------------------------------

namespace netdomain.Specification
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// The Utility class provides some methods to compose lambda expressions without using invoke, 
    /// and leverage it to implement EF-friendly AND and OR builder methods.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Compose two expressions to an bitwise OR expression.
        /// </summary>
        /// <typeparam name="T">The type of the expression</typeparam>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>A bitwise AND expression of the two expressions.</returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }

        /// <summary>
        /// Compose two expressions to an bitwise AND expression.
        /// </summary>
        /// <typeparam name="T">The type of the expression.</typeparam>
        /// <param name="first">The first expression.</param>
        /// <param name="second">The second expression.</param>
        /// <returns>A bitwise AND expression of the two expressions.</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }

        /// <summary>
        /// Composes two lambda expressions with a AND or OR expression without using invoke.
        /// </summary>
        /// <typeparam name="T">The type of the expression.</typeparam>
        /// <param name="first">The first expression to compose.</param>
        /// <param name="second">The second expression to compose.</param>
        /// <param name="merge">The merge expression.</param>
        /// <returns>A new expression.</returns>
        private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }
    }
}
