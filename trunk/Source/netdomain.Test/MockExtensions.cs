//-------------------------------------------------------------------------------
// <copyright file="MockExtensions.cs" company="bbv Software Services AG">
//   Copyright (c) 2010 bbv Software Services AG
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

namespace netdomain
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Moq;
    using Moq.Language.Flow;

    /// <summary>
    /// Provides extension methods for easier usage of <see cref="Mock{T}"/>.
    /// </summary>
    public static class MockExtensions
    {
        /// <summary>
        /// Verifies that a specific invocation matching the given expression was performed only once on the mock. Use in
        /// conjunction with the default <see cref="MockBehavior.Loose"/>.
        /// </summary>
        /// <typeparam name="T">The type of the expression.</typeparam>
        /// <param name="mock">The extended mock.</param>
        /// <param name="expression">Expression to verify.</param>
        /// <exception cref="MockException">The invocation was called never or multiple times.</exception>
        public static void VerifyOnce<T>(this Mock<T> mock, Expression<Action<T>> expression) where T : class
        {
            mock.Verify(expression, Times.Once());
        }

        /// <summary>
        /// Verifies that a specific invocation matching the given expression was performed only once on the mock. Use in
        /// conjunction with the default <see cref="MockBehavior.Loose"/>.
        /// </summary>
        /// <typeparam name="T">The type of the expression.</typeparam>
        /// <param name="mock">The extended mock.</param>
        /// <param name="expression">Expression to verify.</param>
        /// <param name="failMessage">The fail message.</param>
        /// <exception cref="MockException">The invocation was called never or multiple times.</exception>
        public static void VerifyOnce<T>(this Mock<T> mock, Expression<Action<T>> expression, string failMessage) where T : class
        {
            mock.Verify(expression, Times.Once(), failMessage);
        }

        /// <summary>
        /// Verifies that a specific invocation matching the given expression was never performed on the mock. Use in
        /// conjunction with the default <see cref="MockBehavior.Loose"/>.
        /// </summary>
        /// <typeparam name="T">The type of the expression.</typeparam>
        /// <param name="mock">The extended mock.</param>
        /// <param name="expression">Expression to verify.</param>
        /// <exception cref="MockException">The invocation was called one or multiple times.</exception>
        public static void VerifyNever<T>(this Mock<T> mock, Expression<Action<T>> expression) where T : class
        {
            mock.Verify(expression, Times.Never());
        }

        /// <summary>
        /// Verifies that a specific invocation matching the given expression was never performed on the mock. Use in
        /// conjunction with the default <see cref="MockBehavior.Loose"/>.
        /// </summary>
        /// <typeparam name="T">The type of the expression.</typeparam>
        /// <param name="mock">The extended mock.</param>
        /// <param name="expression">Expression to verify.</param>
        /// <param name="failMessage">The fail message.</param>
        /// <exception cref="MockException">The invocation was called one or multiple times.</exception>
        public static void VerifyNever<T>(this Mock<T> mock, Expression<Action<T>> expression, string failMessage) where T : class
        {
            mock.Verify(expression, Times.Never(), failMessage);
        }

        /// <summary>
        /// Verifies that a property was never read on the mock.
        /// </summary>
        /// <typeparam name="T">The type of the expression.</typeparam>
        /// <typeparam name="TProperty">Type of the property to verify. Typically omitted as it can be inferred from the expression's return type.</typeparam>
        /// <param name="mock">The extended mock.</param>
        /// <param name="expression">Expression to verify.</param>
        public static void VerifyNeverGet<T, TProperty>(this Mock<T> mock, Expression<Func<T, TProperty>> expression) where T : class
        {
            mock.VerifyGet(expression, Times.Never());
        }

        /// <summary>
        /// Returns the provided results in order of appearance on each call define in setup.
        /// </summary>
        /// <typeparam name="T">The type of the mock</typeparam>
        /// <typeparam name="TResult">The result of the mock.</typeparam>
        /// <param name="setup">The setup expression.</param>
        /// <param name="results">The result of the setup expression.</param>
        /// <example>
        /// <code>var reader = new Mock&lt;IDataReader&gt;();
        /// reader.Setup(r => r.Read()).ReturnsInOrder(true, true, false);
        /// </code>
        /// </example>
        public static void ReturnsInOrder<T, TResult>(this ISetup<T, TResult> setup, params TResult[] results) 
            where T : class
        {
            setup.Returns(new Queue<TResult>(results).Dequeue);
        }
    }
}