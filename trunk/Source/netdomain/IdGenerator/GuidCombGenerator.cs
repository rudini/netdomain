//-------------------------------------------------------------------------------
// <copyright file="GuidCombGenerator.cs" company="bbv Software Services AG">
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
// Contains parts from "NHibernate" of Donald Mull see http://nhibernate.org.
// </copyright>
//------------------------------------------------------------------------------

namespace netdomain.IdGenerator
{
    using System;
    using Abstract;

    /// <summary>
    /// An <see cref="IIdentifierGenerator" /> that generates <see cref="System.Guid"/> values using a strategy suggested Jimmy Nilsson's 
    /// <a href="http://www.informit.com/articles/article.asp?p=25862">article</a> on <a href="http://www.informit.com">informit.com</a>. 
    /// </summary>
    public class GuidCombGenerator : IIdentifierGenerator
    {
        /// <summary>
        /// Generate a new <see cref="Guid"/> using the comb algorithm.
        /// </summary>
        /// <typeparam name="T">The type of the entity the identifier is being generated.</typeparam>
        /// <param name="context">The <see cref="IWorkspace"/> this id is being generated in.</param>
        /// <param name="obj">The entity for which the id is being generated.</param>
        /// <returns>
        /// The new identifier as a <see cref="Guid"/>.
        /// </returns>
        public object Generate<T>(IWorkspace context, T obj)
        {
            return this.GenerateComb();
        }

        /// <summary>
        /// Generate a new <see cref="Guid"/> using the comb algorithm.
        /// </summary>
        /// <returns>The new generated id of <see cref="T:System.Guid"/></returns>
        private Guid GenerateComb()
        {
            byte[] guidArray = Guid.NewGuid().ToByteArray();

            var baseDate = new DateTime(1900, 1, 1);
            var now = DateTime.Now;

            // Get the days and milliseconds which will be used to build the byte string 
            var days = new TimeSpan(now.Ticks - baseDate.Ticks);
            var msecs = now.TimeOfDay;

            // Convert to a byte array 
            // Note that SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333 
            var daysArray = BitConverter.GetBytes(days.Days);
            var msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));

            // Reverse the bytes to match SQL Servers ordering 
            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);

            // Copy the bytes into the guid 
            Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
            Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

            return new Guid(guidArray);
        }
    }
}