//-------------------------------------------------------------------------------
// <copyright file="WorkspaceExtender.cs" company="bbv Software Services AG">
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

namespace netdomain.Workspace
{
    using Abstract;

    using netdomain.Helpers;
    using netdomain.IdGenerator;

    /// <summary>
    /// Implements extension methods for IUnitOfWork.
    /// </summary>
    public static class WorkspaceExtender
    {
        /// <summary>
        /// Generates an id defined by a IdentifierGeneratorAttribute.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="workspace">The unit of work.</param>
        /// <param name="entity">The entity to create the id for.</param>
        public static void GenerateId<T>(this IWorkspace workspace, T entity)
        {
            var t = entity.GetType();
            foreach (IdentifierGeneratorAttribute attr in t.GetCustomAttributes(typeof(IdentifierGeneratorAttribute), true))
            {
                var current = t.GetProperty(attr.Propertyname).GetValue(entity, null);
                var defaultValue = TypeHelper.GetDefault(current.GetType());

                if (defaultValue.Equals(current))
                {
                    var idgen = IdentifierGeneratorFactory.Instance.Create(attr.GeneratorType);
                    var id = idgen.Generate(workspace, entity);
                    t.GetProperty(attr.Propertyname).SetValue(entity, id, null);
                }
            }
        }
    }
}