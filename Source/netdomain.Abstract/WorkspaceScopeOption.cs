//-------------------------------------------------------------------------------
// <copyright file="WorkspaceScopeOption.cs" company="bbv Software Services AG">
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
    /// <summary>
    /// Provides additional options for creating a workspace scope.
    /// </summary>
    public enum WorkspaceScopeOption
    {
        /// <summary>
        /// A workspace is required by the scope. It uses an ambient workspace if one already exists. 
        /// Otherwise, it creates a new workspace before entering the scope. This is the default value.
        /// </summary>
        Required,

        /// <summary>
        /// A new workspace is always created for the scope.
        /// </summary>
        RequiresNew
    }
}