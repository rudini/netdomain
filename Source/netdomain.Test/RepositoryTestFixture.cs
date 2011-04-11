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
    using System.Collections.Generic;
    using System.Linq;

    using Moq;

    using netdomain.Abstract;
    using netdomain.LinqToObjects;
    using netdomain.LinqToObjects.Test.BusinessObjects;
    using netdomain.Specification;

    using NUnit.Framework;

    /// <summary>
    /// Summary description for RepositoryTestFixture
    /// </summary>
    [TestFixture]
    public class RepositoryTestFixture
    {
        private Mock<IWorkspace> workspaceMock;
        private InMemoryQueryableContext<Person> inMemoryQueryContext;
        private IPersonRepository testee;

        [SetUp]
        public void SetUp()
        {
            this.workspaceMock = new Mock<IWorkspace>();

            this.inMemoryQueryContext = new InMemoryQueryableContext<Person>(new InMemoryContext());
            this.workspaceMock.Setup(w => w.CreateQuery<Person>()).Returns(this.inMemoryQueryContext);
 
            this.testee = new PersonRepository(this.workspaceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void Add_MustCallAddOnWorkspace()
        {
            // Arrange
            var person = new Person();

            // Act
            this.testee.AddPerson(person);

            // Assert
            this.workspaceMock.Verify(w => w.Add(person));
        }

        [Test]
        public void Delete_MustCallDeleteOnWorkspace()
        {
            // Arrange
            var person = new Person();

            // Act
            this.testee.DeletePerson(person);

            // Assert
            this.workspaceMock.Verify(w => w.Delete(person));
        }

        [Test]
        public void Update_MustCallUpdateOnWorkspace()
        {
            // Arrange
            var person = new Person();

            // Act
            this.testee.UpdatePerson(person);

            // Assert
            this.workspaceMock.Verify(w => w.Update(person));
        }

        [Test]
        public void Specification_MustReturnTheRelevantEntity()
        {
            // Arrange
            var name = "Testname";
            var beruf = "Testberuf";

            var nameEqualsTestname = new Specification<Person>(p => p.Name == name);
            var berufEqualsTestberuf = new Specification<Person>(p => p.Beruf == beruf);
            var nameEqualsTestnameAndBerufEqualsTestberuf = nameEqualsTestname.And(berufEqualsTestberuf);

            var person = new Person { Name = name, Beruf = beruf };
            this.inMemoryQueryContext.Insert(person);

            // Act
            var fetchedPerson = this.testee.FindBySpecification(nameEqualsTestnameAndBerufEqualsTestberuf).First();

            // Assert
            Assert.AreEqual(person, fetchedPerson);
        }
    }

    public interface IPersonRepository
    {
        void AddPerson(Person person);

        void DeletePerson(Person item);

        void UpdatePerson(Person item);

        new IEnumerable<Person> FindBySpecification(ISpecification<Person> spec);
    }

    public class PersonRepository : Repository<Person>, IPersonRepository
    {
        public PersonRepository(IWorkspace workspace) : base(workspace)
        {
        }

        public void AddPerson(Person person)
        {
            this.Add(person);
        }

        public void DeletePerson(Person item)
        {
            this.Delete(item);
        }

        public void UpdatePerson(Person item)
        {
            this.Update(item);
        }

        public new IEnumerable<Person> FindBySpecification(ISpecification<Person> spec)
        {
            return base.FindBySpecification(spec);
        }
    }
}