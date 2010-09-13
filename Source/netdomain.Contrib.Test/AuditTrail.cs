//-------------------------------------------------------------------------------
// <copyright file="AuditTrail.cs" company="bbv Software Services AG">
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

namespace netdomain.Contrib
{
    using System;

    /// <summary>
    /// A sample AuditTrail class.
    /// </summary>
    public class AuditTrail
    {
        /// <summary>
        /// Gets or sets the id of the audit entry.
        /// </summary>
        /// <value>The id of the audit entry.</value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the entity id.
        /// </summary>
        /// <value>The entity id.</value>
        public string EntityId { get; set; }

        /// <summary>
        /// Gets or sets the name of the entity.
        /// </summary>
        /// <value>The name of the entity.</value>
        public string EntityName { get; set; }

        /// <summary>
        /// Gets or sets the property.
        /// </summary>
        /// <value>The property.</value>
        public string Property { get; set; }

        /// <summary>
        /// Gets or sets the property old value.
        /// </summary>
        /// <value>The property old value.</value>
        public string PropertyOldValue { get; set; }

        /// <summary>
        /// Gets or sets the property new value.
        /// </summary>
        /// <value>The property new value.</value>
        public string PropertyNewValue { get; set; }

        /// <summary>
        /// Gets or sets the type of the operation.
        /// </summary>
        /// <value>The type of the operation.</value>
        public string OperationType { get; set; }

        /// <summary>
        /// Gets or sets the actor.
        /// </summary>
        /// <value>The actor.</value>
        public string Actor { get; set; }

        /// <summary>
        /// Gets or sets the relevance time.
        /// </summary>
        /// <value>The relevance time.</value>
        public DateTime RelevanceTime { get; set; }
    }
}