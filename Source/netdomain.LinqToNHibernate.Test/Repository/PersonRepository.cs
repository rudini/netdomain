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

namespace netdomain.LinqToNHibernate.Test.Repository
{
    using System.Linq;
    using netdomain.Abstract;
    using netdomain.LinqToNHibernate.Test.BusinessObjects;
    using NHibernate.Linq;

    /// <summary>
    /// Repository for Person objects
    /// </summary>
    public class PersonRepository : Repository<Person>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersonRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public PersonRepository(IWorkspace context)
            : base(context)
        {
            this.Query.FetchMany(p => p.Adressliste);
        }

        /// <summary>
        /// Finds the person.
        /// </summary>
        /// <param name="name">Name of the person.</param>
        /// <returns>An instance of a Person</returns>
        public Person FindPerson(string name)
        {
            return this.Query.Where(c => c.Name == name).Single<Person>();
        }
    }
}