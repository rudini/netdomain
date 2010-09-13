//-------------------------------------------------------------------------------
// <copyright file="ParameterRebinder.cs" company="bbv Software Services AG">
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
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// Implementats a visitor that rebind parameters 
    /// because LINQ to Entities does not support InvocationExpressions.
    /// </summary>
    internal class ParameterRebinder : ExpressionVisitor
    {
        /// <summary>
        /// Holds a dictionary to  
        /// </summary>
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterRebinder"/> class.
        /// </summary>
        /// <param name="map">TThe map holding all parameter expressions to replace.</param>
        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        /// <summary>
        /// Replaces parameters of an expression by a given map.
        /// <remarks>Calls the Visit method of the <see cref="ExpressionVisitor"/> class.</remarks>
        /// </summary>
        /// <param name="map">The map holding all parameter expressions to replace.</param>
        /// <param name="exp">The expression where the parameter shall be replaced.</param>
        /// <returns>A new expression where the parameter are replaced.</returns>
        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        /// <summary>
        /// Overrides the VisitParameter method of the <see cref="ExpressionVisitor"/> class to replace parameters.
        /// </summary>
        /// <param name="p">The <see cref="ParameterExpression"/>.</param>
        /// <returns>A new expression where the parameter are replaced.</returns>
        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;

            if (this.map.TryGetValue(p, out replacement))
            {
                p = replacement;
            }

            return base.VisitParameter(p);
        }
    }
}
