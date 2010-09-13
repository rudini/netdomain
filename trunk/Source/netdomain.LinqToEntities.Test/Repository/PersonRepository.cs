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

namespace netdomain.LinqToEntities.Test.Repository
{
    using System.Linq;
    using Abstract;
    using netdomain.LinqToEntities.Test.BusinessObjects;

    public class PersonRepository : Repository<PersonPoco>
    {
        public PersonRepository(IWorkspace context) : base(context)
        {
            this.Query.Include("Addressliste");
        }

        public PersonPoco FindPerson(string name)
        {
            return (from c in this.Query
                    where c.Name == name
                    select c).First<PersonPoco>(); // "Single" won't supported by LinqToEntities(Beta3) !!!
        }
    }
}
