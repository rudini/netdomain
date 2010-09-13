//-------------------------------------------------------------------------------
// <copyright file="WorkspaceScope.cs" company="bbv Software Services AG">
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
    using System.Collections.Generic;
    using Abstract;

    /// <summary>
    /// Implements a workspace scope to share a workspace between method calls. 
    /// </summary>
    public class WorkspaceScope : IDisposable
    {
        /// <summary>
        /// The workspace type to use.
        /// </summary>
        private readonly Type workspaceType;

        /// <summary>
        /// The lock object.
        /// </summary>
        private static readonly object lockObject = new object();

        /// <summary>
        /// A stack to arrange workspaces between scopes. 
        /// </summary>
        [ThreadStatic]
        private static Stack<IWorkspace> workspaceStack;

        /// <summary>
        /// A flag to find out wheter the scope is already disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:netdomain.WorkspaceScope"/> class.
        /// </summary>
        public WorkspaceScope() : this(WorkspaceScopeOption.Required)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:netdomain.WorkspaceScope"/> class.
        /// </summary>
        /// <param name="workspaceType">Type of the workspace.</param>
        public WorkspaceScope(Type workspaceType) : this(workspaceType, WorkspaceScopeOption.Required)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:netdomain.WorkspaceScope"/> class.
        /// </summary>
        /// <param name="workspaceScopeOption">The workspace scope option.</param>
        public WorkspaceScope(WorkspaceScopeOption workspaceScopeOption) : this(null, workspaceScopeOption)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceScope"/> class.
        /// </summary>
        /// <param name="workspaceType">Type of the workspace.</param>
        /// <param name="workspaceScopeOption">The workspace scope option.</param>
        public WorkspaceScope(Type workspaceType, WorkspaceScopeOption workspaceScopeOption)
        {
            this.workspaceType = workspaceType;

            switch (workspaceScopeOption)
            {
                case WorkspaceScopeOption.RequiresNew:
                    this.SetCurrentWorkspace(true);
                    break;
                case WorkspaceScopeOption.Required:
                    this.SetCurrentWorkspace(false);
                    break;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:netdomain.WorkspaceScope"/> class.
        /// </summary>
        /// <param name="workspaceToUse">The workspace to use.</param>
        public WorkspaceScope(IWorkspace workspaceToUse)
        {
            this.CurrentWorkspace = workspaceToUse;
            WorkspaceStack.Push(this.CurrentWorkspace);
        }

        /// <summary>
        /// Gets a stack to arrange workspaces between scopes. 
        /// </summary>
        public static Stack<IWorkspace> WorkspaceStack
        {
            get { return workspaceStack ?? (workspaceStack = new Stack<IWorkspace>()); }
        }

        /// <summary>
        /// Gets the current workspace.
        /// </summary>
        /// <value>The current workspace.</value>
        public IWorkspace CurrentWorkspace { get; private set; }

        /// <summary>
        /// Submits the changes.
        /// </summary>
        public void SubmitChanges()
        {
            this.CurrentWorkspace.SubmitChanges();
        }

        /// <summary>
        /// Cleans this instance.
        /// </summary>
        public void Clean()
        {
            this.CurrentWorkspace.Clean();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!this.disposed)
            {
                if (this.CurrentWorkspace != WorkspaceStack.Pop())
                {
                    throw new InvalidOperationException(
                        "The current workspace must be the same as the workspace on top of the stack.");
                }

                if (workspaceStack.Count == 0 || (this.CurrentWorkspace != WorkspaceStack.Peek()))
                {
                    lock (lockObject)
                    {
                        WorkspaceBuilder.Current.GetWorkspaceFactory().ReleaseWorkspace(this.CurrentWorkspace);
                    }
                }

                GC.SuppressFinalize(this);
                this.disposed = true;
            }
        }

        /// <summary>
        /// Sets the current workspace.
        /// </summary>
        /// <param name="requiresNew">if set to <c>true</c> [requires new].</param>
        private void SetCurrentWorkspace(bool requiresNew)
        {
            if (WorkspaceStack.Count == 0 || requiresNew)
            {
                lock (lockObject)
                {
                    var workspaceFactory = WorkspaceBuilder.Current.GetWorkspaceFactory();
                    this.CurrentWorkspace = this.workspaceType != null
                                                ? workspaceFactory.GetWorkspaceInstance(this.workspaceType)
                                                : workspaceFactory.GetWorkspaceInstance();
                }
            }
            else
            {
                this.CurrentWorkspace = WorkspaceStack.Peek();
            }

            WorkspaceStack.Push(this.CurrentWorkspace);
        }
    }
}