//-------------------------------------------------------------------------------
// <copyright file="IdGeneratorExtension.cs" company="bbv Software Services AG">
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

namespace netdomain.WorkspaceExtensions
{
    using netdomain;
    using netdomain.Workspace;

    /// <summary>
    /// Implements a workspace extension to generate an identifier using the IIdentifierGeneratorFactory.
    /// </summary>
    public class IdGeneratorExtension : WorkspaceExtension
    {
        /// <summary>
        /// Called after an entitie has been added.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public override void OnEntityAdded<T>(T entity)
        {
            this.Workspace.GenerateId(entity);
        }
    }
}