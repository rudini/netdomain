//-------------------------------------------------------------------------------
// <copyright file="LinqToObjectsWorkspaceFixture.cs" company="bbv Software Services AG">
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

namespace netdomain.LinqToObjects.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using Abstract;
    using BusinessObjects;
    using Contrib.Validation;
    using FluentValidation.Results;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using NMock2;

    /// <summary>
    /// Summary description for InMemoryDataSourceUnitTest
    /// </summary>
    [TestClass]
    public class LinqToObjectsWorkspaceFixture
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        private Mockery mockery;

        protected IWorkspace Testee { get; set; }

        protected Mock<IWorkspaceBuilder> WorkspaceBuilderMock { get; set; }

        protected Mock<IWorkspaceExtension> ExtensionMock { get; set; }

        protected Mock<IWorkspaceFactory> WorkspaceFactoryMock { get; set; }

        [TestInitialize]
        public void SetUp()
        {
            this.mockery = new Mockery();
            this.RegisterExtensions();
            this.Testee = new InMemoryWorkspace(new ValidationHelper()); // TODO: move the test to contrib
        }

        [TestCleanup]
        public void TearDown()
        {
            WorkspaceBuilder.Current = null;
            this.mockery.Dispose();
            this.Testee.Dispose();
        }

        #region derived tests

        [TestMethod]
        public void AddEntity()
        {
            var name = "Testname";
            var profession = "TestProfession";
            var address = "TestAddress";

            var newPerson = this.CreateANewEntity(name, profession, address);
            this.Testee.SubmitChanges();

            var fetchedPerson = this.Testee.CreateQuery<Person>().Where(p => p.Name == name).First();

            Assert.AreEqual(newPerson, fetchedPerson, "The fetched person must be the same than the created.");
            Assert.AreEqual(newPerson.Adressliste, fetchedPerson.Adressliste, "The Addresses must be the same references.");

            this.Testee.Delete(newPerson);
            this.Testee.SubmitChanges();
        }

        [TestMethod]
        public void DeleteEntity()
        {
            var name = "Testname";
            var profession = "TestProfession";
            var address = "TestAddress";

            var newPerson = this.CreateANewEntity(name, profession, address);
            this.Testee.SubmitChanges();

            var fetchedPerson = this.Testee.CreateQuery<Person>().Where(p => p.Name == name).First();

            Assert.AreEqual(newPerson, fetchedPerson, "The fetched person must be the same than the created.");
            Assert.AreEqual(newPerson.Adressliste, fetchedPerson.Adressliste, "The Addresses must be the same references.");

            this.Testee.Delete(newPerson);
            this.Testee.SubmitChanges();

            fetchedPerson = this.Testee.CreateQuery<Person>().Where(p => p.Name == name).FirstOrDefault();

            Assert.IsNull(fetchedPerson, "The fetched person must be null.");
        }

        [TestMethod]
        public void UpdateEntity()
        {
            var name = "Testname";
            var profession = "TestProfession";
            var address = "TestAddress";
            var updatedProfession = "Updated";
            var updatedAddress = "Updated";

            var newPerson = this.CreateANewEntity(name, profession, address);
            this.Testee.SubmitChanges();

            newPerson.Beruf = updatedProfession;
            newPerson.Adressliste.First().Name = updatedAddress;

            this.Testee.SubmitChanges();

            var fetchedPerson = this.Testee.CreateQuery<Person>().Where(p => p.Name == name).First();

            Assert.AreEqual(name, fetchedPerson.Name, string.Format("The updated name must be {0}.", name));
            Assert.AreEqual(updatedProfession, fetchedPerson.Beruf, string.Format("The updated beruf must be {0}.", updatedProfession));
            Assert.AreEqual(updatedAddress, fetchedPerson.Adressliste.First().Name, string.Format("The updated address must be {0}.", updatedAddress));

            this.Testee.Delete(newPerson);
            this.Testee.SubmitChanges();
        }

        [TestMethod]
        public void DetachEntity()
        {
            // Not supported by InMemoryContext
        }

        [TestMethod]
        public void AttachEntity()
        {
            var name = "Testname";
            var profession = "TestProfession";
            var address = "TestAddress";
            var updatedProfession = "Updated";

            var newPerson = this.CreateANewEntity(name, profession, address);
            this.Testee.SubmitChanges();

            this.Testee.Detach(newPerson);

            this.Testee.Attach(newPerson);
            newPerson.Beruf = updatedProfession;
            this.Testee.SubmitChanges();

            var fetchedPerson = this.Testee.CreateQuery<Person>().Where(p => p.Name == name).First();

            Assert.AreEqual(name, fetchedPerson.Name, string.Format("The name name must be {0}.", name));
            Assert.AreEqual(updatedProfession, fetchedPerson.Beruf, string.Format("The beruf must be {0}.", updatedProfession));

            this.Testee.Delete(fetchedPerson);
            this.Testee.SubmitChanges();
        }

        [TestMethod]
        public void UpdateDetachedEntity()
        {
            var name = "Testname";
            var profession = "TestProfession";
            var address = "TestAddress";
            var updatedProfession = "Updated";

            var newPerson = this.CreateANewEntity(name, profession, address);
            this.Testee.SubmitChanges();

            this.Testee.Detach(newPerson);
            newPerson.Beruf = updatedProfession;

            this.Testee.Update(newPerson);
            this.Testee.SubmitChanges();

            var fetchedPerson = this.Testee.CreateQuery<Person>().Where(p => p.Name == name).First();

            Assert.AreEqual(name, fetchedPerson.Name, string.Format("The name name must be {0}.", name));
            Assert.AreEqual(updatedProfession, fetchedPerson.Beruf, string.Format("The updated beruf must be {0}.", updatedProfession));

            this.Testee.Delete(fetchedPerson);
            this.Testee.SubmitChanges();
        }

        [TestMethod]
        public void AttachEntityToANewWorkspace()
        {
            // Not supported by InMemoryContext
        }

        [TestMethod]
        public void CreateQuery()
        {
            var name = "Testname";
            var profession = "TestProfession";
            var address = "TestAddress";

            var newPerson = this.CreateANewEntity(name, profession, address);
            this.Testee.SubmitChanges();

            var count = this.Testee.CreateQuery<Person>().Where(p => p.Name == name).Count();

            Assert.AreEqual(1, count, string.Format("The count of persons must be {0}.", 1));

            this.Testee.Delete(newPerson);
            this.Testee.SubmitChanges();
        }

        [TestMethod]
        public void GetByKey()
        {
            // Not supported by InMemoryContext
        }

        [TestMethod]
        public void GetByKeyFromDb()
        {
            // Not supported by InMemoryContext
        }

        [TestMethod]
        public void RefreshEntity()
        {
            // Not supported by InMemoryContext
        }

        [TestMethod]
        public void CleanCache()
        {
            var name = "Testname";
            var profession = "TestProfession";
            var address = "TestAddress";
            var updatedProfession = "Updated";

            var newPerson = this.CreateANewEntity(name, profession, address);
            this.Testee.SubmitChanges();

            this.Testee.Clean();

            newPerson.Beruf = updatedProfession;

            this.Testee.Update(newPerson);
            this.Testee.SubmitChanges();

            var fetchedPerson = this.Testee.CreateQuery<Person>().Where(p => p.Name == name).First();

            Assert.AreEqual(name, fetchedPerson.Name, string.Format("The name name must be {0}.", name));
            Assert.AreEqual(updatedProfession, fetchedPerson.Beruf, string.Format("The updated beruf must be {0}.", updatedProfession));

            this.Testee.Delete(fetchedPerson);
            this.Testee.SubmitChanges();
        }

        [TestMethod]
        public void Include()
        {
            var name = "Testname";
            var profession = "TestProfession";
            var address = "TestAddress";

            this.CreateANewEntity(name, profession, address);
            this.Testee.SubmitChanges();

            this.Testee.Clean();

            var fetchedPerson2 = this.Testee.CreateQuery<Person>().Include("Adresse").Where(p => p.Name == name).First<Person>();
            Assert.IsFalse(fetchedPerson2.Adressliste.Count == 0, "The Adressliste count shall not be 0");
            Assert.IsNotNull(fetchedPerson2.Adressliste.ElementAt(0), "The index 0 of Adressliste count shall not be null");

            this.Testee.Delete(fetchedPerson2);
            this.Testee.SubmitChanges();
        }

        [TestMethod]
        [ExpectedException(typeof(OptimisticOfflineLockException))]
        public void ConcurencyException()
        {
            // Not supported by InMemoryContext
            throw new OptimisticOfflineLockException("Not supported by InMemoryContext", null);
        }

        [TestMethod]
        public void DisconnectReconnect()
        {
            // Not supported by InMemoryContext
        }

        public void Transaction()
        {
            // Not supported by InMemoryContext
        }

        [TestMethod]
        public void TransactionWithExplicitConnectionHandling()
        {
            // Not supported by InMemoryContext
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException<ValidationResult>))]
        public void Validation()
        {
            var name = "A too long string for the field name in db";
            var profession = "A too long string for the field profession";
            var address = "A too long string for the field name";

            try
            {
                this.CreateANewEntity(name, profession, address);
                this.Testee.SubmitChanges();
            }
            catch (ValidationException<ValidationResult> e)
            {
                Assert.AreEqual(3, e.ValidationResults.Errors.Count);
                Assert.AreEqual("Name", e.ValidationResults.Errors.ElementAt(0).PropertyName);
                Assert.AreEqual("Beruf", e.ValidationResults.Errors.ElementAt(1).PropertyName);
                Assert.AreEqual("Name", e.ValidationResults.Errors.ElementAt(2).PropertyName);
                throw;
            }
        }

        [TestMethod]
        public void IsDirty()
        {
            var name1 = "Testname1";
            var profession1 = "TestProfession1";
            var address1 = "TestAddress1";

            var person1 = this.CreateANewEntity(name1, profession1, address1);
            Assert.IsTrue(this.Testee.IsDirty());

            var name2 = "Testname2";
            var profession2 = "TestProfession2";
            var address2 = "TestAddress2";

            var person2 = this.CreateANewEntity(name2, profession2, address2);

            this.Testee.SubmitChanges();
            Assert.IsFalse(this.Testee.IsDirty());

            var name3 = "Testname3";
            var profession3 = "TestProfession3";
            var address3 = "TestAddress3";

            var person3 = this.CreateANewEntity(name3, profession3, address3);

            person1.Beruf = "NewProfession1";

            // mark an entity as deleted in the cache
            this.Testee.Delete(person2);
            Assert.IsTrue(this.Testee.IsDirty());

            this.Testee.SubmitChanges();
            Assert.IsFalse(this.Testee.IsDirty());

            // load an entity to the cache
            var fetchedPerson1 = this.Testee.CreateQuery<Person>().Where(p => p.Name == name1).First();
            var fetchedPerson3 = this.Testee.CreateQuery<Person>().Where(p => p.Name == name3).First();

            this.Testee.Delete(fetchedPerson1);
            this.Testee.Delete(fetchedPerson3);
            Assert.IsTrue(this.Testee.IsDirty());

            this.Testee.SubmitChanges();
            Assert.IsFalse(this.Testee.IsDirty());
        }

        [TestMethod]
        public void NestedWorkspaceScope()
        {
            // Arrange
            this.WorkspaceFactoryMock.Setup(factory => factory.GetWorkspaceInstance()).Returns(this.Testee);

            var name = "Testname";
            var profession = "TestProfession";
            var address = "TestAddress";

            using (var tx = new TransactionScope())
            using (var scope = new WorkspaceScope(this.Testee))
            {
                var newPerson = this.CreateANewEntity(name, profession, address);
                scope.SubmitChanges();

                var fetchedPerson = scope.CurrentWorkspace.CreateQuery<Person>().Where(p => p.Name == name).First();

                Assert.AreEqual(newPerson, fetchedPerson, "The fetched person must be the same than the created.");
                Assert.AreEqual(newPerson.Adressliste, fetchedPerson.Adressliste, "The Addresses must be the same references.");

                var newName = "newName";
                this.SubMethodCallUsingWorkspaceScopeChangesName(name, newName);

                var changedPerson = scope.CurrentWorkspace.CreateQuery<Person>().Where(p => p.Id == newPerson.Id).First();

                Assert.AreEqual(newName, changedPerson.Name, "The fetched person must be the same than the created.");
                Assert.AreEqual(newPerson.Id, changedPerson.Id, "The Addresses must be the same references.");
            }

            this.WorkspaceFactoryMock.Verify(factory => factory.ReleaseWorkspace(It.IsAny<IWorkspace>()), Times.Once());
        }

        [TestMethod]
        public void CallOnEntityAddedOnWorkspaceExtension()
        {
            // Arrange
            var person = new Person();

            // Act
            this.Testee.Add(person);

            // Assert
            this.ExtensionMock.Verify(e => e.OnEntityAdded(person));
            Assert.AreEqual(this.Testee, this.ExtensionMock.Object.Workspace);
        }

        [TestMethod]
        public void CallOnEntityDeletedOnWorkspaceExtension()
        {
            // Arrange
            var person = new Person();

            // Act
            this.Testee.Add(person);
            this.Testee.Delete(person);

            // Assert
            this.ExtensionMock.Verify(e => e.OnEntityDeleted(person));
            Assert.AreEqual(this.Testee, this.ExtensionMock.Object.Workspace);
        }

        [TestMethod]
        public void CallOnEntityUpdatedOnWorkspaceExtension()
        {
            // Arrange
            var person = new Person();

            // Act
            this.Testee.Update(person);

            // Assert
            this.ExtensionMock.Verify(e => e.OnEntityUpdated(person));
            Assert.AreEqual(this.Testee, this.ExtensionMock.Object.Workspace);
        }

        [TestMethod]
        public void CallOnSubmittingChangesOnWorkspaceExtension()
        {
            // Arrange
            var name1 = "Testname1";
            var profession1 = "TestProfession1";
            var person1 = new Person { Name = name1, Beruf = profession1 };
            var person2 = new Person { Name = name1, Beruf = profession1 };
            var person3 = new Person { Name = name1, Beruf = profession1 };

            // Act
            using (var tx = new TransactionScope()) // when using database identifier, add creates a row in the database 
            {
                this.Testee.Add(person1);
                this.Testee.Add(person2);
                this.Testee.Add(person3);
                person2.Name = "changed";
                this.Testee.Delete(person3);
                this.Testee.SubmitChanges();
            }

            // Assert
            this.ExtensionMock.Verify(e => e.OnEntityAdded(person1));
            this.ExtensionMock.Verify(e => e.OnEntityAdded(person2));
            this.ExtensionMock.Verify(e => e.OnEntityAdded(person3));
            this.ExtensionMock.Verify(e => e.OnEntityDeleted(person3));
            this.ExtensionMock.Verify(e => e.OnSubmittingChanges(new List<object>(), new[] { person1, person2 }, new List<object>()));
            Assert.AreEqual(this.Testee, this.ExtensionMock.Object.Workspace);
        }

        [TestMethod]
        public void CallOnCacheCleanedOnWorkspaceExtension()
        {
            // Act
            this.Testee.Clean();

            // Assert
            this.ExtensionMock.Verify(e => e.OnCacheCleaned());
        }

        [TestMethod]
        public void CallOnEntityRefreshedOnWorkspaceExtension()
        {
            // Not supported on InMemoryWorkspace
        }

        [TestMethod]
        public void CallOnEntityAttachedOnWorkspaceExtension()
        {
            // Arrange
            var id = 1;
            var name1 = "Testname1";
            var profession1 = "TestProfession1";
            var person1 = new Person { Id = id, Name = name1, Beruf = profession1 };

            // Act
            using (var tx = new TransactionScope())
            {
                this.Testee.Attach(person1);
            }

            // Assert
            this.ExtensionMock.Verify(e => e.OnEntityAttached(person1));
        }

        [TestMethod]
        public void CallOnEntityDetachedOnWorkspaceExtension()
        {
            // not supported by the InMemoryUnitOfWork
        }

        public void CallOnOptimisticOfflineLockExceptionThrownOnWorkspaceExtension()
        {
            // Not supported by InMemoryContext
        }

        #endregion

        #region test helper methods

        private void SubMethodCallUsingWorkspaceScopeChangesName(string oldName, string newName)
        {
            using (var scope = new WorkspaceScope(WorkspaceScopeOption.RequiresNew))
            {
                var fetchedPerson = scope.CurrentWorkspace.CreateQuery<Person>().Where(p => p.Name == oldName).First();
                fetchedPerson.Name = newName;

                scope.SubmitChanges();
            }
        }

        private Person CreateANewEntity(string name, string profession, string address)
        {
            var newPerson = new Person
            {
                Name = name,
                Beruf = profession
            };

            var newAddress = new Adresse
            {
                Name = address
            };

            newPerson.Adressliste.Add(newAddress);
            this.Testee.Add(newPerson);
            return newPerson;
        }

        #endregion

        private void RegisterExtensions()
        {
            this.WorkspaceBuilderMock = new Mock<IWorkspaceBuilder>();
            WorkspaceBuilder.Current = this.WorkspaceBuilderMock.Object;
            this.ExtensionMock = new Mock<IWorkspaceExtension>();
            this.WorkspaceFactoryMock = new Mock<IWorkspaceFactory>();
            this.WorkspaceBuilderMock.Setup(b => b.GetExtensionInstances()).Returns(new[] { this.ExtensionMock.Object });
            this.WorkspaceBuilderMock.Setup(b => b.GetWorkspaceFactory()).Returns(this.WorkspaceFactoryMock.Object);
            this.ExtensionMock.SetupAllProperties();
        }
    }
}
