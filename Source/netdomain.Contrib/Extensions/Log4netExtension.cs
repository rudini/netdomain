//-------------------------------------------------------------------------------
// <copyright file="Log4netExtension.cs" company="bbv Software Services AG">
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

namespace netdomain.Contrib.Extensions
{
    using System;
    using Abstract;
    using log4net;

    /// <summary>
    /// Implements an workspace extension to log using log4net
    /// </summary>
    public class Log4netExtension : WorkspaceExtension
    {
        #region logger

        /// <summary>
        /// Type specific logger 
        /// </summary>
        private readonly ILog log;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4netExtension"/> class.
        /// </summary>
        public Log4netExtension()
        {
            this.log = LogManager.GetLogger(this.Workspace.GetType().DeclaringType);
        }

        /// <summary>
        /// Called after an optimistic offline lock exception has ocurred.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public override void OnOptimisticOfflineLockExceptionThrown(OptimisticOfflineLockException exception)
        {
            this.log.Debug("Optimistic concurrency error.");
            this.log.Debug(exception.Message);
        }

        /// <summary>
        /// Called after an exception has occurred.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public override void OnExceptionThrown(Exception exception)
        {
            this.log.Error(exception.Message);
        }
    }
}