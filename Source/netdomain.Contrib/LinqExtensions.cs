//-------------------------------------------------------------------------------
// <copyright file="LinqExtensions.cs" company="bbv Software Services AG">
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
// Contains parts from "LINQ and recursive functions" 
// of Microsoft Developer Network
// published on http://social.msdn.microsoft.com/Forums/en-US/linqprojectgeneral/thread/fe3d441d-1e49-4855-8ae8-60068b3ef741.
// </copyright>
//------------------------------------------------------------------------------

namespace netdomain.Contrib
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Enhance the IEnumerable interface.
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Traverses an object tree e.g. an composite component structure and returns an enumerable/&gt;.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <param name="source">The source enumerable list e.g. the root nodes.</param>
        /// <param name="functionRecurse">The recursive function.</param>
        /// <returns>An enumerable of all objects of type T.</returns>
        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> source, Func<T, object> functionRecurse)
        {
            foreach (T value in source)
            {
                yield return value;

                var tmp = functionRecurse(value);
                var seqRecursive = tmp as IEnumerable<T>;

                if (seqRecursive != null)
                {
                    foreach (T child in seqRecursive.Traverse(functionRecurse))
                    {
                        yield return child;
                    }
                }
            }
        }
    }
}