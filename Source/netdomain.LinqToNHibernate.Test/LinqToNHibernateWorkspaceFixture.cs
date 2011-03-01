//-------------------------------------------------------------------------------
// <copyright file="LinqToNHibernateWorkspaceFixture.cs" company="bbv Software Services AG">
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

namespace netdomain.LinqToNHibernate.Test
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Transactions;

    using Moq;

    using netdomain.Abstract;
    using netdomain.LinqToNHibernate;
    using netdomain.LinqToNHibernate.Test.BusinessObjects;

    using NHibernate.Linq;

    using NUnit.Framework;

    /// <summary>
    /// Test class for LINQ To NHibernate
    /// </summary>
    [TestFixture]
    public class LinqToNHibernateWorkspaceFixture
    {
        protected IWorkspace Testee { get; set; }

        protected Mock<IWorkspaceBuilder> WorkspaceBuilderMock { get; set; }

        protected Mock<IWorkspaceExtension> ExtensionMock { get; set; }

        protected Mock<IWorkspaceFactory> WorkspaceFactoryMock { get; set; }

        [SetUp]
        public void SetUp()
        {
            this.RegisterExtensions();
            this.Testee = new LinqToNHibernateWorkspace(new LinqToNHibernateContext().GetSession());
        }

        [TearDown]
        public void TearDown()
        {
            WorkspaceBuilder.Current = null;
            this.Testee.Dispose();
            SqlConnection.ClearAllPools();
        }

        #region derived tests

        [Test]
        public void AddEntity()
        {
            var name = "Testname";
            var profession = "TestProfession";
            var address = "TestAddress";            

            using (new TransactionScope())
            {
                var newPerson = this.CreateANewEntity(name, profession, address);
                this.Testee.SubmitChanges();

                var fetchedPerson = this.Testee.CreateQuery<Person>().Where(p => p.Name == name).First();

                Assert.AreEqual(newPerson, fetchedPerson, "The fetched person must be the same than the created.");
                Assert.AreEqual(newPerson.Adressliste, fetchedPerson.Adressliste, "The Addresses must be the same references.");
            }
        }

        [Test]
        public void DeleteEntity()
        {
            var name = "Testname";
            var profession = "TestProfession";
            var address = "TestAddress";

            using (new TransactionScope())
            {
                var newPerson = this.CreateANewEntity(name, profession, address);
                this.Testee.SubmitChanges();

                var fetchedPerson = this.Testee.CreateQuery<Person>().Where(p => p.Name == name).First();

                Assert.AreEqual(newPerson, fetchedPerson, "The fetched person must be the same than the created.");
                Assert.AreEqual(newPerson.Adressliste, fetchedPerson.Adressliste, "The Addresses must be the same references.");

                this.Testee.Delete(fetchedPerson);
                this.Testee.SubmitChanges();

                fetchedPerson = this.Testee.CreateQuery<Person>().Where(p => p.Name == name).FirstOrDefault();
                var fetchedAddress = this.Testee.CreateQuery<Adresse>().Where(a => a.Name == address).FirstOrDefault();

                Assert.IsNull(fetchedPerson, "The fetched person must be null.");
                Assert.IsNull(fetchedAddress, "The fetched address must be null.");
            }
        }

        [Test]
        public void UpdateEntity()
        {
            var name = "Testname";
            var profession = "TestProfession";
            var address = "TestAddress";
            var updatedProfession = "Updated";
            var updatedAddress = "Updated";

            using (new TransactionScope())
            {
                var newPerson = this.CreateANewEntity(name, profession, address);
                this.Testee.SubmitChanges();

                newPerson.Beruf = updatedProfession;
                newPerson.Adressliste.First().Name = updatedAddress;

                this.Testee.SubmitChanges();

                var fetchedPerson = this.Testee.CreateQuery<Person>().Where(p => p.Name == name).First();

                Assert.AreEqual(name, fetchedPerson.Name, string.Format("The updated name must be {0}.", name));
                Assert.AreEqual(updatedProfession, fetchedPerson.Beruf, string.Format("The updated beruf must be {0}.", updatedProfession));
                Assert.AreEqual(updatedAddress, fetchedPerson.Adressliste.First().Name, string.Format("The updated address must be {0}.", updatedAddress));
            }
        }

        [Test]
        public void DetachEntity()
        {
            var name = "Testname";
            var profession = "TestProfession";
            var address = "TestAddress";
            var updatedProfession = "Updated";

            using (new TransactionScope())
            {
                var newPerson = this.CreateANewEntity(name, profession, address);
                this.Testee.SubmitChanges();

                this.Testee.Detach(newPerson);
                newPerson.Beruf = updatedProfession;
                this.Testee.SubmitChanges();

                var fetchedPerson = this.Testee.CreateQuery<Person>().Where(p => p.Name == name).First();

                Assert.AreEqual(name, fetchedPerson.Name, string.Format("The name name must be {0}.", name));
                Assert.AreEqual(profession, fetchedPerson.Beruf, string.Format("The beruf must be {0}.", profession));
            }
        }

        [Test]
        public void AttachEntity()
        {
            var name = "Testname";
            var profession = "TestProfession";
            var address = "TestAddress";
            var updatedProfession = "Updated";

            using (new TransactionScope())
            {
                var newPerson = this.CreateANewEntity(name, profession, address);
                this.Testee.SubmitChanges();

                this.Testee.Detach(newPerson);

                this.Testee.Attach(newPerson);
                newPerson.Beruf = updatedProfession;
                this.Testee.SubmitChanges();

                var fetchedPerson = this.Testee.CreateQuery<Person>().Where(p => p.Name == name).First();

                Assert.AreEqual(name, fetchedPerson.Name, string.Format("The name name must be {0}.", name));
                Assert.AreEqual(updatedProfession, fetchedPerson.Beruf, string.Format("The beruf must be {0}.", updatedProfession));
            }
        }

        [Test]
        public void UpdateDetachedEntity()
        {
            var name = "Testname";
            var profession = "TestProfession";
            var address = "TestAddress";
            var updatedProfession = "Updated";
            
            using (new TransactionScope())
            {
                var newPerson = this.CreateANewEntity(name, profession, address);
                this.Testee.SubmitChanges();

                this.Testee.Detach(newPerson);
                newPerson.Beruf = updatedProfession;

                this.Testee.Update(newPerson);
                this.Testee.SubmitChanges();

                var fetchedPerson = this.Testee.CreateQuery<Person>().Where(p => p.Name == name).First();

                Assert.AreEqual(name, fetchedPerson.Name, string.Format("The name name must be {0}.", name));
                Assert.AreEqual(updatedProfession, fetchedPerson.Beruf, string.Format("The updated beruf must be {0}.", updatedProfession));
            }
        }

        [Test]
        public void AttachEntityToANewWorkspace()
        {
            var name = "Testname";
            var profession = "TestProfession";
            var address = "TestAddress";
            var updatedProfession = "Updated";
            
            using (new TransactionScope())
            {
                var newPerson = this.CreateANewEntity(name, profession, address);
                this.Testee.SubmitChanges();

                this.Testee.Detach(newPerson);

                var ctx1 = new LinqToNHibernateContext().GetSession();
                var ws1 = new LinqToNHibernateWorkspace(ctx1);

                newPerson.Beruf = updatedProfession;

                ws1.Update(newPerson);
                ws1.SubmitChanges();

                var fetchedPerson = this.Testee.CreateQuery<Person>().Where(p => p.Name == name).First();

                Assert.AreEqual(name, fetchedPerson.Name, string.Format("The name name must be {0}.", name));
                Assert.AreEqual(updatedProfession, fetchedPerson.Beruf, string.Format("The updated beruf must be {0}.", updatedProfession));
            }
        }

        [Test]
        public void CreateQuery()
        {
            var name = "Testname";
            var profession = "TestProfession";
            var address = "TestAddress";

            using (new TransactionScope())
            {
                var newPerson = this.CreateANewEntity(name, profession, address);
                this.Testee.SubmitChanges();

                var count = this.Testee.CreateQuery<Person>().Where(p => p.Name == name).Count();

                Assert.AreEqual(1, count, string.Format("The count of persons must be {0}.", 1));
            }
        }

        [Test]
        public void GetByKey()
        {
            var name = "Testname";
            var profession = "TestProfession";
            var address = "TestAddress";
            
            using (new TransactionScope())
            {
                var newPerson = this.CreateANewEntity(name, profession, address);
                this.Testee.SubmitChanges();

                var fetchedPerson = this.Testee.Get(newPerson);

                Assert.AreEqual(newPerson, fetchedPerson, string.Format("The fetched person and the created person must be the same."));
            }
        }

        [Test]
        public void GetByKeyFromDb()
        {
            var name = "Testname";
            var profession = "TestProfession";
            var address = "TestAddress";

            using (new TransactionScope())
            {
                var newPerson = this.CreateANewEntity(name, profession, address);
                this.Testee.SubmitChanges();

                // create an entity with primary keys only
                var entityWithKey = new Person { Id = newPerson.Id };

                this.Testee.Clean();
                var fetchedPerson = this.Testee.Get(entityWithKey);

                // you have to compare the id because the Equals() and GetHashCode() method are not implemented in Linq to entities
                Assert.AreEqual(newPerson.Id, fetchedPerson.Id, string.Format("The fetched persons' id and the created persons' id must be the same."));
                Assert.AreNotEqual(newPerson, fetchedPerson, string.Format("The fetched person and the created person must not be the same."));
            }
        }

        [Test]
        public void RefreshEntity()
        {
            var name = "Testname";
            var profession = "TestProfession";
            var address = "TestAddress";
            var updatedName = "UpdatedN";
            var updatedProfession = "UpdatedP";
            var updatedAdress = "UpdatedA";

            using (new TransactionScope())
            {
                var newPerson = this.CreateANewEntity(name, profession, address);
                this.Testee.SubmitChanges();

                newPerson.Name = updatedName;
                newPerson.Beruf = updatedProfession;
                newPerson.Adressliste.First().Name = updatedAdress;

                this.Testee.Refresh(newPerson);
                this.Testee.Refresh(newPerson.Adressliste.First());

                Assert.AreEqual(name, newPerson.Name, string.Format("The name name must be {0}.", name));
                Assert.AreEqual(profession, newPerson.Beruf, string.Format("The beruf must be {0}.", profession));
                Assert.AreEqual(address, newPerson.Adressliste.First().Name, string.Format("The address must be {0}.", address));
            }
        }

        [Test]
        public void CleanCache()
        {
            var name = "Testname";
            var profession = "TestProfession";
            var address = "TestAddress";
            var updatedProfession = "Updated";
            
            using (new TransactionScope())
            {
                var newPerson = this.CreateANewEntity(name, profession, address);
                this.Testee.SubmitChanges();

                this.Testee.Clean();

                newPerson.Beruf = updatedProfession;

                this.Testee.Update(newPerson);
                this.Testee.SubmitChanges();

                var fetchedPerson = this.Testee.CreateQuery<Person>().Where(p => p.Name == name).First();

                Assert.AreEqual(name, fetchedPerson.Name, string.Format("The name name must be {0}.", name));
                Assert.AreEqual(updatedProfession, fetchedPerson.Beruf, string.Format("The updated beruf must be {0}.", updatedProfession));
            }
        }

        [Test]
        public void Include()
        {
            var name = "Testname";
            var profession = "TestProfession";
            var address = "TestAddress";

            using (new TransactionScope())
            {
                this.CreateANewEntity(name, profession, address);
                this.Testee.SubmitChanges();

                this.Testee.Clean();

                var fetchedPerson2 =
                    this.Testee.CreateQuery<Person>().Where(p => p.Name == name).FetchMany(p => p.Adressliste).ThenFetch(a => a.AdresseDetails).First();

                Assert.IsFalse(fetchedPerson2.Adressliste.Count == 0, "The Adressliste count shall not be 0");
                Assert.IsNotNull(fetchedPerson2.Adressliste.ElementAt(0), "The index 0 of Adressliste count shall not be null");
            }
        }

        [Test]
        [ExpectedException(typeof(OptimisticOfflineLockException))]
        public void ConcurencyException()
        {
            var name = "Testname";
            var profession = "TestProfession";

            using (new TransactionScope())
            {
                var person = new Person { Name = name, Beruf = profession };
                this.Testee.Add(person);
                this.Testee.SubmitChanges();

                var ctx1 = new LinqToNHibernateContext().GetSession();
                var ws1 = new LinqToNHibernateWorkspace(ctx1);

                var ctx2 = new LinqToNHibernateContext().GetSession();
                var ws2 = new LinqToNHibernateWorkspace(ctx2);

                var person1 = ws1.CreateQuery<Person>().Where(p => p.Name == name).First();
                var person2 = ws2.CreateQuery<Person>().Where(p => p.Name == name).First();

                person1.Name = "Updated1";
                person2.Name = "Updated2";

                try
                {
                    ws1.SubmitChanges();
                    ws2.SubmitChanges();
                }
                catch (OptimisticOfflineLockException ex)
                {
                    Assert.AreEqual(1, ex.ConfictedObjects.Count());
                    Assert.AreEqual(typeof(Person), ex.ConfictedObjects[0].Type);
                    var conflictedperson = (Person)ex.ConfictedObjects[0].Object;
                    Assert.AreEqual("Updated2", conflictedperson.Name);
                    throw;
                }
            }
        }

        [Test]
        public void DisconnectReconnect()
        {
            IConnectionManager manager = this.Testee.ConnectionManager;
            manager.Disconnect();
            Assert.IsFalse(manager.IsConnected);

            manager.Reconnect();
            Assert.IsTrue(manager.IsConnected);
        }

        [Test]
        [Ignore("MSDTC must be available")]
        public void TransactionWithExplicitConnectionHandling()
        {
            var name = "Testname";
            var profession = "TestProfession";
            var address = "TestAddress";

            IConnectionManager manager = this.Testee.ConnectionManager;

            using (var ts = new TransactionScope())
            {
                // NHibernate connection is already open when the session has been created. 
                manager.Reconnect();
                Assert.IsTrue(manager.IsConnected);

                this.CreateANewEntity(name, profession, address);
                this.Testee.SubmitChanges();
                Assert.IsTrue(manager.IsConnected);
                manager.Disconnect();

                Assert.IsFalse(manager.IsConnected);

                using (var innerTs = new TransactionScope())
                {
                    manager.Reconnect();
                    Assert.IsTrue(manager.IsConnected);

                    manager.ExecuteNonQuery("StoredProcedure1");
                }
            }

            Assert.IsNull(this.Testee.CreateQuery<Person>().Where(p => p.Name == name).FirstOrDefault());
            Assert.IsTrue(manager.IsConnected);
        }

        [Test]
        [Ignore("MSDTC must be available")]
        public void Transaction()
        {
            var name = "Testname";
            var profession = "TestProfession";
            var address = "TestAddress";

            IConnectionManager manager = this.Testee.ConnectionManager;

            using (var ts = new TransactionScope())
            {
                this.CreateANewEntity(name, profession, address);
                this.Testee.SubmitChanges();

                using (var innerTs = new TransactionScope())
                {
                    manager.ExecuteNonQuery("StoredProcedure1");
                }
            }

            Assert.IsNull(this.Testee.CreateQuery<Person>().Where(p => p.Name == name).FirstOrDefault());
        }

        [Test]
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

        [Test]
        public void NestedWorkspaceScope()
        {
            // arrange
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

        [Test]
        public void CallOnEntityAddedOnWorkspaceExtension()
        {
            // Arrange
            var person = new Person();

            // Act
            using (var tx = new TransactionScope()) // when using database identifier, add creates a row in the database 
            {
                this.Testee.Add(person);
            }

            // Assert
        }

        [Test]
        public void CallOnEntityDeletedOnWorkspaceExtension()
        {
            // Arrange
            var person = new Person();

            // Act
            using (var tx = new TransactionScope()) // when using database identifier, insert creates a row in the database 
            {
                this.Testee.Add(person);
                this.Testee.Delete(person);
            }

            // Assert
            this.ExtensionMock.Verify(e => e.OnEntityDeleted(person));
            Assert.AreEqual(this.Testee, this.ExtensionMock.Object.Workspace);
        }

        [Test]
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

        [Test]
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
            
            // when using database identifier, the on submitting changes does not include the already added entities.
            this.ExtensionMock.Verify(e => e.OnSubmittingChanges(new[] { person3 }, new List<object>(), new[] { person2 }));
            Assert.AreEqual(this.Testee, this.ExtensionMock.Object.Workspace);
        }

        [Test]
        public void CallOnCacheCleanedOnWorkspaceExtension()
        {
            // Act
            this.Testee.Clean();

            // Assert
            this.ExtensionMock.Verify(e => e.OnCacheCleaned());
        }

        [Test]
        public void CallOnEntityRefreshedOnWorkspaceExtension()
        {
            // Arrange
            var name1 = "Testname1";
            var profession1 = "TestProfession1";
            var person1 = new Person { Name = name1, Beruf = profession1 };

            // Act
            using (var tx = new TransactionScope())
            {
                this.Testee.Add(person1);
                this.Testee.SubmitChanges();
                this.Testee.Refresh(person1);
            }

            // Assert
            this.ExtensionMock.Verify(e => e.OnEntityRefreshed(person1));
        }

        [Test]
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

        [Test]
        public void CallOnEntityDetachedOnWorkspaceExtension()
        {
            // Arrange
            var id = 1;
            var name1 = "Testname1";
            var profession1 = "TestProfession1";
            var person1 = new Person { Id = id, Name = name1, Beruf = profession1 };

            // Act
            using (var tx = new TransactionScope())
            {
                this.Testee.Add(person1);
                this.Testee.SubmitChanges(); // create an entity key by saving the entity to store
                this.Testee.Detach(person1);
            }

            // Assert
            this.ExtensionMock.Verify(e => e.OnEntityDetached(person1));
        }

        [Test]
        public void CallOnOptimisticOfflineLockExceptionThrownOnWorkspaceExtension()
        {
            // Arrange
            OptimisticOfflineLockException exception = null;

            // Act
            try
            {
                this.ConcurencyException(); // call the dependent Test
            }
            catch (OptimisticOfflineLockException ex)
            {
                exception = ex;
            }

            // Assert
            this.ExtensionMock.Verify(e => e.OnOptimisticOfflineLockExceptionThrown(exception));
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