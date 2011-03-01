//-------------------------------------------------------------------------------
// <copyright file="PropertyAccessorVisitor.cs" company="bbv Software Services AG">
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
// </copyright>
//------------------------------------------------------------------------------

namespace netdomain.LinqToSql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Implements a visitor to get the property accessor as a lambdaexpression.
    /// </summary>
    public class PropertyAccessorVisitor : ExpressionVisitor
    {
        /// <summary>
        /// The stack to store the sequence of lambda expressions.
        /// </summary>
        private readonly Stack<LambdaExpression> stack;

        /// <summary>
        /// The member expression to hold the member expression in memory between two calls.
        /// </summary>
        private MemberExpression memberExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyAccessorVisitor"/> class.
        /// </summary>
        public PropertyAccessorVisitor()
        {
            this.stack = new Stack<LambdaExpression>();
        }

        /// <summary>
        /// Gets the lambda expressions.
        /// </summary>
        /// <param name="selector">The lambda expression to get the property accessor from.</param>
        /// <returns>an enumerable of lambda expressions.</returns>
        public IEnumerable<LambdaExpression> GetPropertyAccessorExpressions(LambdaExpression selector)
        {
            Visit(selector);
            return this.stack;
        }

        /// <summary>
        /// Visits the <see cref="ParameterExpression"/> and pushes a new Lambda expression on the stack.
        /// </summary> 
        /// <param name="expression">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; 
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitParameter(ParameterExpression expression)
        {
            if (this.memberExpression != null)
            {
                this.stack.Push(Expression.Lambda(this.memberExpression, expression));
                this.memberExpression = null;
            }

            return base.VisitParameter(expression);
        }

        /// <summary>
        /// Visits the children of the <see cref="MemberExpression"/>.
        /// Stores the member expression to create a lambda expression for.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitMember(MemberExpression expression)
        {
            this.memberExpression = expression;
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