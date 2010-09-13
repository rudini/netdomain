//-------------------------------------------------------------------------------
// <copyright file="Specification.cs" company="bbv Software Services AG">
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

namespace netdomain.Specification
{
    using System;
    using System.Linq.Expressions;
    using netdomain.Abstract;

    /// <summary>
    /// Implements a base class of a specification based on the DDD pattern by Eric Evans.
    /// </summary>
    /// <typeparam name="T">The type the specification is based on.</typeparam>
    public class Specification<T> : ISpecification<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:netdomain.Specification.Specification`1"/> class.
        /// </summary>
        public Specification()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:netdomain.Specification.Specification`1"/> class.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public Specification(Expression<Func<T, bool>> predicate)
        {
            this.Predicate = predicate;
        }

        /// <summary>
        /// Gets or sets the predicate as a strongly typed lambda expression.
        /// </summary>
        /// <value>The predicate.</value>
        public Expression<Func<T, bool>> Predicate { get; set; }

        /// <summary>
        /// Determines whether a specification is satisfied by a particular candidate.
        /// </summary>
        /// <param name="candidate">The candidate to determine.</param>
        /// <returns>
        /// <c>true</c> if the specification is satisfied by a particular candidate; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsSatisfiedBy(T candidate)
        {
            return this.Predicate.Compile().Invoke(candidate);
        }

        #region ISpecification<T> Members

        /// <summary>
        /// Assign another specification as a bitwise AND operation.
        /// </summary>
        /// <param name="other">The other specification to assign as a bitwise AND operation.</param>
        /// <returns>A new composite specification.</returns>
        public ISpecification<T> And(ISpecification<T> other)
        {
            return new Specification<T>(other.Predicate.And<T>(this.Predicate));
        }

        /// <summary>
        /// Get the current specification as a unary expression.
        /// </summary>
        /// <returns>
        /// The current specification as a unary expression.
        /// </returns>
        public ISpecification<T> Not()
        {
            return new Specification<T>(Expression.Lambda<Func<T, bool>>(Expression.Not(this.Predicate.Body), this.Predicate.Parameters));
        }

        /// <summary>
        /// Assign another specification as a bitwise OR operation.
        /// </summary>
        /// <param name="other">The other specification to assign as a bitwise OR operation.</param>
        /// <returns>A new composite specification.</returns>
        public ISpecification<T> Or(ISpecification<T> other)
        {
            return new Specification<T>(other.Predicate.Or<T>(this.Predicate));
        }

        #endregion
    }
}
