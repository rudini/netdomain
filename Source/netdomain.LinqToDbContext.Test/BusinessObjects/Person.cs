//-------------------------------------------------------------------------------
// <copyright file="Person.cs" company="bbv Software Services AG">
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

namespace netdomain.LinqToDbContext.Test.BusinessObjects
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public class Person
    {
        public Person()
        {
            this.Adressliste = new HashSet<Adresse>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Beruf { get; set; }
        public byte[] Version { get; set; }
        //public Nullable<System.DateTime> NVersion { get; set; }

        public virtual ICollection<Adresse> Adressliste { get; set; }
    }
}
