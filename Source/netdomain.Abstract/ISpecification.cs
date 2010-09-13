//-------------------------------------------------------------------------------
// <copyright file="ISpecification.cs" company="bbv Software Services AG">
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
// Contains parts from "Architecting LINQ to SQL applications, part 4" 
// of Ian Cooper published on http://codebetter.com.
// </copyright>
//------------------------------------------------------------------------------

namespace netdomain.Abstract
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Defines an interface of a specification based on the DDD pattern by Eric Evans.
    /// </summary>
    /// <typeparam name="T">The type of the specification.</typeparam>
    public interface ISpecification<T>
    {
        /// <summary>
        /// Gets or sets the predicate as a strongly typed lambda expression.
        /// </summary>
        /// <value>The predicate.</value>
        Expression<Func<T, bool>> Predicate { get; set; }

        /// <summary>
        /// Determines whether a specification is satisfied by a particular candidate.
        /// </summary>
        /// <param name="candidate">The candidate to determine.</param>
        /// <returns>
        /// <c>true</c> if the specification is satisfied by a particular candidate; otherwise, <c>false</c>.
        /// </returns>
        bool IsSatisfiedBy(T candidate);

        /// <summary>
        /// Assign another specification as a bitwise AND operation.
        /// </summary>
        /// <param name="other">The other specification to assign as a bitwise AND operation.</param>
        /// <returns>A new composite specification.</returns>
        ISpecification<T> And(ISpecification<T> other);

        /// <summary>
        /// Assign another specification as a bitwise OR operation.
        /// </summary>
        /// <param name="other">The other specification to assign as a bitwise OR operation.</param>
        /// <returns>A new composite specification.</returns>
        ISpecification<T> Or(ISpecification<T> other);

        /// <summary>
        /// Get the current specification as a unary expression.
        /// </summary>
        /// <returns>The current specification as a unary expression.</returns>
        ISpecification<T> Not();
    }
}
