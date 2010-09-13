//-------------------------------------------------------------------------------
// <copyright file="AuditLogExtensionFixture.cs" company="bbv Software Services AG">
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

namespace netdomain.Contrib
{
    using System;
    using System.Security.Principal;
    using System.Threading;
    using Extensions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using netdomain.Abstract;

    /// <summary>
    /// Summary description for WorkspaceScopeFixture
    /// </summary>
    [TestClass]
    public class AuditLogExtensionFixture
    {
        /// <summary>
        /// Gets or sets the test context which provides 
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        public AuditLogExtension Testee { get; set; }

        protected Mock<IWorkspaceFactory> WorkspaceFactoryMock { get; set; }

        protected Mock<IWorkspaceBuilder> WorkspaceBuilderMock { get; set; }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestInitialize]
        public void SetUp()
        {
            this.Testee = new MyAuditLog();
            this.WorkspaceBuilderMock = new Mock<IWorkspaceBuilder>();
            WorkspaceBuilder.Current = this.WorkspaceBuilderMock.Object;
            this.WorkspaceFactoryMock = new Mock<IWorkspaceFactory>();
            this.WorkspaceBuilderMock.Setup(b => b.GetWorkspaceFactory()).Returns(this.WorkspaceFactoryMock.Object);
        }

        [TestMethod]
        public void LogAuditBeforeSubmittChanges()
        {
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("Testuser"), new[] { "Admin" });

            var workspaceMock1 = new Mock<IWorkspace>();
            var workspaceMock2 = new Mock<IWorkspace>();
            this.Testee.Workspace = workspaceMock1.Object;
            this.WorkspaceFactoryMock.Setup(f => f.GetWorkspaceInstance()).Returns(workspaceMock2.Object);

            var deletedEntities = new[]
                                      {
                                          new AuditableItem { Id = 1, Name = "Name1" },
                                          new AuditableItem { Id = 2, Name = "Name2" }
                                      };

            var addedEntities = new[]
                                      {
                                          new AuditableItem { Id = 3, Name = "Name3" },
                                          new AuditableItem { Id = 4, Name = "Name4" }
                                      };

            var o1 = new[] { new object() };

            var item1 = new NestedAuditableItem {Id = 1};
            var item2 = new NestedAuditableItem {Id = 2};

            var originalEntity1 = new AuditableItem { Id = 5, Name = "Name5", Objects = o1, Price = 44, Item = item1 };
            var originalEntity2 = new AuditableItem { Id = 6, Name = "Name6", Objects = new[] { new object() }, Price = 55, Item = item2 };

            var modifiedEntity1 = new AuditableItem { Id = 5, Name = "Name7", Objects = o1, Price = 7777, Item = new NestedAuditableItem {Id = 3} };
            var modifiedEntity2 = new AuditableItem { Id = 6, Name = null, Objects = new[] { new object() }, Price = 222, Item = item2 };
            var modifiedEntities = new[] { modifiedEntity1, modifiedEntity2 };

            workspaceMock1.Setup(w => w.Add(It.IsAny<AuditTrail>()));
            workspaceMock2.Setup(w => w.Get(It.Is<object>(o => ((AuditableItem)o).Id == 5))).Returns(originalEntity1);
            workspaceMock2.Setup(w => w.Get(It.Is<object>(o => ((AuditableItem)o).Id == 6))).Returns(originalEntity2);
            workspaceMock2.Setup(w => w.Get(It.Is<object>(o => ((AuditableItem)o).Id == 100))).Returns(new AuditableItem { Id = 100 });
            
            this.Testee.OnSubmittingChanges(deletedEntities, addedEntities, modifiedEntities);
        }
    }

    public class AuditableItem : IAuditable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public object[] Objects { get; set; }
        public NestedAuditableItem Item { get; set; }
        public string GetId()
        {
            return Id.ToString();
        }
    }

    public class NestedAuditableItem : IAuditable
    {
        public int Id { get; set; }
        public string GetId()
        {
            return Id.ToString();
        }
    }

    public class MyAuditLog : AuditLogExtension
    {
        public override void LogAudit(IAuditable entity, AuditAction auditAction, string property, string propertyNewValue, string propertyOldValue)
        {
            var audit = new AuditTrail
                            {
                                Id = Guid.NewGuid(),
                                Actor = Thread.CurrentPrincipal.Identity.Name,
                                EntityName = entity.GetType().Name,
                                EntityId = entity.GetId(),
                                OperationType = auditAction.ToString(),
                                Property = property,
                                PropertyNewValue = propertyNewValue,
                                PropertyOldValue = propertyOldValue,
                                RelevanceTime = DateTime.UtcNow
                            };

            this.Workspace.Add(audit);

            Console.WriteLine(
                string.Format(
                "Id={0};Actor={1};EntityName={2};EntityId={3};OperationType={4};Property={5};NewValue={6};OldValue={7};",
                audit.Id,
                audit.Actor,
                audit.EntityName,
                audit.EntityId,
                audit.OperationType,
                audit.Property,
                audit.PropertyNewValue,
                audit.PropertyOldValue));
        }
    }
}