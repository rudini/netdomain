//-------------------------------------------------------------------------------
// <copyright file="PersonRepository.cs" company="bbv Software Services AG">
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

namespace netdomain.LinqToSql.Test.Repository
{
    using System.Linq;
    using Abstract;
    using netdomain.LinqToSql.Test.BusinessObjects;

    /// <summary>
    /// Implements a person repository to access the person entiy as the root entity.
    /// </summary>
    public class PersonRepository : Repository<Person>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersonRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public PersonRepository(IWorkspace context) : base(context)
        {
            this.Query.Include("Adressliste");
        }

        /// <summary>
        /// Finds the person.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Person FindPerson(string name)
        {
            return (from c in this.Query
                    where c.Name == name
                    select c).Single<Person>();
        }

        /// <summary>
        /// Finds the person light.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public PersonLight FindPersonLight(string name)
        {
            return (from c in this.Query
                                  where c.Name == name
                                  select new PersonLight { Name = c.Name }).First();
        }

        /// <summary>
        /// Finds the person light list.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public IQueryable<PersonLight> FindPersonLightList(string name)
        {
            return from c in this.Query
                   where c.Name == name
                   select new PersonLight { Name = c.Name };
        }
    }

    /// <summary>
    /// Implement a ui optimized data object to reduce io.
    /// </summary>
    public class PersonLight
    {
        public string Name;
    }
}
