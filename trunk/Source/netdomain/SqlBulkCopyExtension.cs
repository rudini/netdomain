//-------------------------------------------------------------------------------
// <copyright file="SqlBulkCopyExtension.cs" company="bbv Software Services AG">
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
    using System.Collections.Generic;
    using System.Data.SqlClient;

    /// <summary>
    /// Enhances the SqlBulkCopy class to support IEnumerable in the WriteToServer method.
    /// </summary>
    public static class SqlBulkCopyExtension
    {
        /// <summary>
        /// Writes all objects in the supplied <see cref="T:System.Collections.Generic.IEnumerable`1"/>
        /// to a destination table specified by the System.Data.SqlClient.SqlBulkCopy.DestinationTableName property of the
        /// System.Data.SqlClient.SqlBulkCopy.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="T:System.Collections.Generic.IEnumerable`1"/></typeparam>
        /// <param name="source">The source <see cref="T:System.Data.SqlClient.SqlBulkCopy"/>.</param>
        /// <param name="data">The data to write to a server table.</param>
        public static void WriteToServer<T>(this SqlBulkCopy source, IEnumerable<T> data)
        {
            source.WriteToServer(new DataReaderWrapper(data));
        }
    }
}