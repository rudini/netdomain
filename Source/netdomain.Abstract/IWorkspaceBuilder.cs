//-------------------------------------------------------------------------------
// <copyright file="IWorkspaceBuilder.cs" company="bbv Software Services AG">
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

namespace netdomain.Abstract
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines an interface responsible for dynamically building a workspace.
    /// </summary>
    public interface IWorkspaceBuilder
    {
        /// <summary>
        /// Sets the default workspace factory.
        /// </summary>
        /// <param name="factory">The workspace factory.</param>
        /// <exception cref="ArgumentNullException">The factory parameter is null.</exception>
        /// <exception cref="InvalidOperationException">There is already a workspace factory registered.</exception>
        void RegisterDefaultWorkspaceFactory(IWorkspaceFactory factory);

        /// <summary>
        /// Removes the workspace factory.
        /// </summary>
        /// <exception cref="InvalidOperationException">When no workspace factory is registered.</exception>
        void RemoveDefaultWorkspaceFactory();

        /// <summary>
        /// Gets the associated workspace factory.
        /// </summary>
        /// <returns>The controller factory.</returns>
        /// <exception cref="InvalidOperationException">If there is no workspace factory registered.</exception>
        IWorkspaceFactory GetWorkspaceFactory();

        /// <summary>
        /// Adds the specified extension. The extension will be considered in any future operation.
        /// </summary>
        /// <typeparam name="T">The type of the extension to add.</typeparam>
        void AddExtension<T>() where T : IWorkspaceExtension;

        /// <summary>
        /// Removes the extension.
        /// </summary>
        /// <typeparam name="T">The type of the extension to remove.</typeparam>
        /// <exception cref="InvalidOperationException">If the type to remove not registered.</exception>
        void RemoveExtension<T>() where T : IWorkspaceExtension;

        /// <summary>
        /// Gets the extension instances.
        /// </summary>
        /// <returns>An enumerable of all registered extensions.</returns>
        IEnumerable<IWorkspaceExtension> GetExtensionInstances();

        /// <summary>
        /// Sets the default workspace factory type.
        /// </summary>
        /// <typeparam name="T">The type of the workspace factory.</typeparam>
        void RegisterDefaultWorkspaceFactory<T>() where T : IWorkspaceFactory;
    }
}