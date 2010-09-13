//-------------------------------------------------------------------------------
// <copyright file="TrackingState.cs" company="bbv Software Services AG">
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

    /// <summary>
    /// Defines an enum of entity state.
    /// </summary>
    [Flags]
    public enum TrackingState
    {
        /// <summary>
        /// Added entity.
        /// </summary>
        Added = 1,

        /// <summary>
        /// Deleted entity.
        /// </summary>
        Deleted = 2,

        /// <summary>
        /// Modified entity.
        /// </summary>
        Modified = 4,
    }
}