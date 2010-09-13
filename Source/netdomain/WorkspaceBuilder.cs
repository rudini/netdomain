//-------------------------------------------------------------------------------
// <copyright file="WorkspaceBuilder.cs" company="bbv Software Services AG">
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

namespace netdomain
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Abstract;

    /// <summary>
    /// Represents a class responsible for dynamically building a workspace.
    /// </summary>
    public class WorkspaceBuilder : IWorkspaceBuilder
    {
        /// <summary>
        /// The lock object.
        /// </summary>
        private static readonly object lockObject = new object();

        /// <summary>
        /// List of all extensions for this event broker.
        /// </summary>
        private readonly List<Type> extensions = new List<Type>();

        /// <summary>
        /// The current workspace builder instance.
        /// </summary>
        private static IWorkspaceBuilder current;

        /// <summary>
        /// The factory to create new workspaces.
        /// </summary>
        private IWorkspaceFactory workspaceFactory;

        /// <summary>
        /// The registered workspace factory typa.
        /// </summary>
        private Type workspaceFactoryType;

        /// <summary>
        /// Gets or sets the current workspace builder object.
        /// </summary>
        /// <value>The current.</value>
        public static IWorkspaceBuilder Current
        {
            get
            {
                lock (lockObject)
                {
                    return current ?? (current = new WorkspaceBuilder());
                }
            }

            set
            {
                lock (lockObject)
                {
                    current = value;
                }
            }
        }

        /// <summary>
        /// Sets the default workspace factory type.
        /// </summary>
        /// <typeparam name="T">The type of the workspace factory.</typeparam>
        public void RegisterDefaultWorkspaceFactory<T>() where T : IWorkspaceFactory 
        {
            lock (lockObject)
            {
                if (this.workspaceFactoryType != null || this.workspaceFactory != null)
                {
                    throw new InvalidOperationException(
                        "The factory is already registered. Call RemoveDefaultWorkspaceFactory before register a new factory.");
                }

                this.workspaceFactoryType = typeof(T);
            }
        }

        /// <summary>
        /// Sets the default workspace factory.
        /// </summary>
        /// <param name="factory">The workspace factory.</param>
        /// <exception cref="ArgumentNullException">The factory parameter is null.</exception>
        /// <exception cref="InvalidOperationException">There is already a workspace factory registered.</exception>
        public void RegisterDefaultWorkspaceFactory(IWorkspaceFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentException("The factory must not be null", "factory");
            }

            lock (lockObject)
            {
                if (this.workspaceFactory != null || this.workspaceFactoryType != null)
                {
                    throw new InvalidOperationException(
                        "The factory is already registered. Call RemoveDefaultWorkspaceFactory before register a new factory.");
                }

                this.workspaceFactory = factory;
            }
        }

        /// <summary>
        /// Removes the workspace factory.
        /// </summary>
        /// <exception cref="InvalidOperationException">When no workspace factory is registered.</exception>
        public void RemoveDefaultWorkspaceFactory()
        {
            if (this.workspaceFactory == null && this.workspaceFactoryType == null)
            {
                throw new InvalidOperationException("There is no registered workspace factory.");
            }

            lock (lockObject)
            {
                this.workspaceFactory = null;
                this.workspaceFactoryType = null;
            }
        }

        /// <summary>
        /// Gets the associated workspace factory.
        /// </summary>
        /// <returns>The controller factory.</returns>
        /// <exception cref="InvalidOperationException">If there is no workspace factory registered.</exception>
        public IWorkspaceFactory GetWorkspaceFactory()
        {
            if (this.workspaceFactory != null)
            {
                return this.workspaceFactory;
            }

            if (this.workspaceFactoryType != null)
            {
                return (IWorkspaceFactory) Activator.CreateInstance(this.workspaceFactoryType);
            }

            throw new InvalidOperationException("There is no workspace factory registered.");
        }

        /// <summary>
        /// Adds the specified extension. The extension will be considered in any future operation.
        /// </summary>
        /// <typeparam name="T">The type of the extension to add.</typeparam>
        public void AddExtension<T>() where T : IWorkspaceExtension
        {
            lock (((ICollection)this.extensions).SyncRoot)
            {
                this.extensions.Add(typeof(T));
            }
        }

        /// <summary>
        /// Removes the extension.
        /// </summary>
        /// <typeparam name="T">The type of the extension to remove.</typeparam>
        /// <exception cref="InvalidOperationException">If the type to remove not registered.</exception>
        public void RemoveExtension<T>() where T : IWorkspaceExtension
        {
            lock (((ICollection)this.extensions).SyncRoot)
            {
                if (!this.extensions.Contains(typeof(T)))
                {
                    throw new InvalidOperationException("There is no such extension type registered.");
                }

                this.extensions.Remove(typeof(T));
            }
        }

        /// <summary>
        /// Gets the extension instances.
        /// </summary>
        /// <returns>An enumerable of all registered extensions.</returns>
        public IEnumerable<IWorkspaceExtension> GetExtensionInstances()
        {
            return this.extensions.Select(extension => (IWorkspaceExtension)Activator.CreateInstance(extension));
        }
    }
}