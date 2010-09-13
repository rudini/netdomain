//-------------------------------------------------------------------------------
// <copyright file="RepositoryTestFixture.cs" company="bbv Software Services AG">
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

namespace netdomain
{
    using System.Linq;
    using Abstract;
    using LinqToObjects;
    using LinqToObjects.Test.BusinessObjects;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NMock2;
    using netdomain.Specification;

    /// <summary>
    /// Summary description for RepositoryTestFixture
    /// </summary>
    [TestClass]
    public class RepositoryTestFixture
    {
        private Mockery mockery;
        private IWorkspace workspace;
        private InMemoryQueryableContext<Person> inMemoryQueryContext;
        private PersonRepository testee;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void SetUp()
        {
            this.mockery = new Mockery();
            this.workspace = this.mockery.NewMock<IWorkspace>();

            this.inMemoryQueryContext = new InMemoryQueryableContext<Person>(new InMemoryContext());
            Expect.Once.On(this.workspace).Method("CreateQuery", typeof(Person)).Will(Return.Value(this.inMemoryQueryContext));

            this.testee = new PersonRepository(this.workspace);
        }

        [TestCleanup]
        public void TearDown()
        {
            this.mockery.Dispose();
        }

        [TestMethod]
        public void Add()
        {
            var person = new Person();

            Expect.Once.On(this.workspace).Method("Add").With(person);

            this.testee.Add(person);

            this.mockery.VerifyAllExpectationsHaveBeenMet();
        }

        [TestMethod]
        public void Delete()
        {
            var person = new Person();

            Expect.Once.On(this.workspace).Method("Delete").With(person);

            this.testee.Delete(person);

            this.mockery.VerifyAllExpectationsHaveBeenMet();
        }

        [TestMethod]
        public void Update()
        {
            var person = new Person();

            Expect.Once.On(this.workspace).Method("Update").With(person);

            this.testee.Update(person);

            this.mockery.VerifyAllExpectationsHaveBeenMet();
        }

        [TestMethod]
        public void Specification()
        {
            var name = "Testname";
            var beruf = "Testberuf";

            var nameEqualsTestname = new Specification<Person>(p => p.Name == name);
            var berufEqualsTestberuf = new Specification<Person>(p => p.Beruf == beruf);
            var nameEqualsTestnameAndBerufEqualsTestberuf = nameEqualsTestname.And(berufEqualsTestberuf);

            var person = new Person { Name = name, Beruf = beruf };
            this.inMemoryQueryContext.Insert(person);

            var fetchedPerson = this.testee.FindBySpecification(nameEqualsTestnameAndBerufEqualsTestberuf).First();

            Assert.AreEqual(person, fetchedPerson);

            this.mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }

    public class PersonRepository : Repository<Person>
    {
        public PersonRepository(IWorkspace workspace) : base(workspace)
        {
        }
    }
}