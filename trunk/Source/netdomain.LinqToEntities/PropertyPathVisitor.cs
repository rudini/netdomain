//-------------------------------------------------------------------------------
// <copyright file="PropertyPathVisitor.cs" company="bbv Software Services AG">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;

    /// <summary>
    /// Implements a visitor to get the property path of an expression.
    /// </summary>
    public class PropertyPathVisitor : ExpressionVisitor
    {
        /// <summary>
        /// The stack to store the sequence of properties.
        /// </summary>
        private Stack<string> stack;

        /// <summary>
        /// Gets the property path.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>the property path as a string.</returns>
        public string GetPropertyPath(Expression expression)
        {
            this.stack = new Stack<string>();
            Visit(expression);
            return this.stack
            .Aggregate(
            new StringBuilder(),
            (sb, name) =>
            (sb.Length > 0 ? sb.Append(".") : sb).Append(name))
            .ToString();
        }

        /// <summary>
        /// Visits the children of the <see cref="MemberExpression"/>.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitMember(MemberExpression expression)
        {
            if (this.stack != null)
            {
                this.stack.Push(expression.Member.Name);
            }

            return base.VisitMember(expression);
        }

        /// <summary>
        /// Visits the children of the <see cref="MethodCallExpression"/>.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (IsLinqOperator(expression.Method))
            {
                for (int i = 1; i < expression.Arguments.Count; i++)
                {
                    Visit(expression.Arguments[i]);
                }

                Visit(expression.Arguments[0]);
                return expression;
            }

            return base.VisitMethodCall(expression);
        }

        /// <summary>
        /// Determines whether the specified method is a linq operator.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>
        /// <c>true</c> if it is a linq operator; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsLinqOperator(MethodInfo method)
        {
            if (method.DeclaringType != typeof(Queryable) && method.DeclaringType != typeof(Enumerable))
            {
                return false;
            }

            return Attribute.GetCustomAttribute(method, typeof(ExtensionAttribute)) != null;
        }
    }
}