//-------------------------------------------------------------------------------
// <copyright file="IWorkspaceFactory.cs" company="bbv Software Services AG">
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

namespace netdomain.Abstract
{
    using System;

    /// <summary>
    /// Defines an interface of a workspace factory.
    /// Implement this interface and register it on the WorkspaceBuilder class using WorkspaceBuilder.Current.RegisterDefaultWorkspaceFactory().
    /// </summary>
    public interface IWorkspaceFactory
    {
        /// <summary>
        /// Gets a new instance of a workspace.
        /// </summary>
        /// <returns>A new created workspace of type <see cref="T:netdomain.Abstract.IWorkspace"/></returns>
        IWorkspace GetWorkspaceInstance();

        /// <summary>
        /// Gets a new workspace instance by the specified type.
        /// </summary>
        /// <param name="workspaceType">Type of the workspace.</param>
        /// <returns>A new created workspace of type <see cref="T:netdomain.Abstract.IWorkspace"/></returns>
        IWorkspace GetWorkspaceInstance(Type workspaceType);

        /// <summary>
        /// Releases the workspace.
        /// </summary>
        /// <param name="workspace">The workspace to release.</param>
        void ReleaseWorkspace(IWorkspace workspace);
    }
}