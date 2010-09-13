//-------------------------------------------------------------------------------
// <copyright file="IConnectionManager.cs" company="bbv Software Services AG">
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
    using System.Data;

    /// <summary>
    /// Defines an interface of a connection manager.
    /// </summary>
    public interface IConnectionManager
    {
        /// <summary>
        /// Gets the underlying ADO .Net database connection of the workspace.
        /// </summary>
        /// <value>The database connection as a <see cref="T:System.Data.IDbConnection"/>.</value>
        IDbConnection Connection { get; }

        /// <summary>
        /// Gets a value indicating whether this workspace is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        bool IsConnected { get; }

        /// <summary>
        /// Disconnects this instance from the current ADO .Net connection.
        /// </summary>
        /// <returns>The current ADO .Net connection.</returns>
        IDbConnection Disconnect();

        /// <summary>
        /// Optain a new ADO .Net connection
        /// </summary>
        /// <returns>The current ADO .Net connection.</returns>
        IDbConnection Reconnect();
    }
}