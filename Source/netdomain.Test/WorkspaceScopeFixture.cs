//-------------------------------------------------------------------------------
// <copyright file="WorkspaceScopeFixture.cs" company="bbv Software Services AG">
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
    using LinqToObjects.Test.BusinessObjects;
    using Moq;
    using netdomain.Abstract;

    using NUnit.Framework;

    /// <summary>
    /// Summary description for WorkspaceScopeFixture
    /// </summary>
    [TestFixture]
    public class WorkspaceScopeFixture
    {
        [Test]
        public void CanRegisterWorkspaceFactory()
        {
            var workspaceMock = new Mock<IWorkspace>();
            var workspaceFactoryMock = this.RegisterWorkspaceFactory();

            workspaceFactoryMock.Setup(factory => factory.GetWorkspaceInstance()).Returns(workspaceMock.Object);
           
            Assert.AreEqual(workspaceMock.Object, new WorkspaceScope().CurrentWorkspace);

            WorkspaceBuilder.Current.RemoveDefaultWorkspaceFactory();
        }

        [Test]
        public void ReleaseMethodCallAfterUsingBlock()
        {
            var workspaceMock = new Mock<IWorkspace>();
            var workspaceFactoryMock = this.RegisterWorkspaceFactory();
            workspaceFactoryMock.Setup(factory => factory.GetWorkspaceInstance()).Returns(workspaceMock.Object);

            using (new WorkspaceScope(workspaceMock.Object))
            {
            }

            workspaceFactoryMock.Verify(factory => factory.ReleaseWorkspace(workspaceMock.Object));

            WorkspaceBuilder.Current.RemoveDefaultWorkspaceFactory();
        }

        [Test]
        public void CreateInstanceByType()
        {
            var workspaceType = typeof(string);
            var workspaceMock = new Mock<IWorkspace>();
            var workspaceFactoryMock = this.RegisterWorkspaceFactory();
            workspaceFactoryMock.Setup(factory => factory.GetWorkspaceInstance(workspaceType)).Returns(
                workspaceMock.Object);

            using (new WorkspaceScope(workspaceType))
            {
            }

            workspaceFactoryMock.Verify(factory => factory.ReleaseWorkspace(workspaceMock.Object));

            WorkspaceBuilder.Current.RemoveDefaultWorkspaceFactory();
        }

        [Test]
        public void CreateANewRepositoryInAWorkspaceScope()
        {
            var person = new Person { Name = "TestName", Beruf = "TestBeruf" };
            var workspaceMock = new Mock<IWorkspace>();
            var workspaceFactoryMock = this.RegisterWorkspaceFactory();
            workspaceFactoryMock.Setup(factory => factory.GetWorkspaceInstance()).Returns(workspaceMock.Object);

            using (var scope = new WorkspaceScope(workspaceMock.Object))
            {
                IPersonRepository repository = new PersonRepository(scope.CurrentWorkspace);
                repository.AddPerson(person);
                scope.SubmitChanges();
            }

            workspaceMock.Verify(scope => scope.Add(person));
            workspaceMock.Verify(scope => scope.SubmitChanges());
            
            WorkspaceBuilder.Current.RemoveDefaultWorkspaceFactory();
        }

        [Test]
        public void NestedWorkspaceScopesWithRequiredWorkspaceOption()
        {
            var workspaceFactoryMock = this.RegisterWorkspaceFactory();
            var outerWorkspace = new Mock<IWorkspace>();
            var innerWorkspace = new Mock<IWorkspace>();
            workspaceFactoryMock.Setup(factory => factory.GetWorkspaceInstance()).ReturnsInOrder(outerWorkspace.Object, innerWorkspace.Object);

            using (var outerScope = new WorkspaceScope(WorkspaceScopeOption.RequiresNew))
            {
                using (var innerScope = new WorkspaceScope())
                {
                    Assert.AreEqual(
                        outerScope.CurrentWorkspace,
                        innerScope.CurrentWorkspace,
                        "The inner workspace must be the same instance like the outer workspace.");
                }
            }

            WorkspaceBuilder.Current.RemoveDefaultWorkspaceFactory();
        }

        [Test]
        public void ReleaseNestedWorkspaceScopesWithRequiredWorkspaceOption()
        {
            var workspaceFactoryMock = this.RegisterWorkspaceFactory();
            var outerWorkspace = new Mock<IWorkspace>();
            var innerWorkspace = new Mock<IWorkspace>();
            workspaceFactoryMock.Setup(factory => factory.GetWorkspaceInstance()).ReturnsInOrder(outerWorkspace.Object, innerWorkspace.Object);

            using (var outerScope = new WorkspaceScope(WorkspaceScopeOption.RequiresNew))
            {
                using (var innerScope = new WorkspaceScope())
                {
                }
            }

            workspaceFactoryMock.Verify(factory => factory.ReleaseWorkspace(outerWorkspace.Object), Times.Once());
            workspaceFactoryMock.Verify(factory => factory.ReleaseWorkspace(innerWorkspace.Object), Times.Never());

            WorkspaceBuilder.Current.RemoveDefaultWorkspaceFactory();
        }

        [Test]
        public void NestedWorkspaceScopesWithRequiresNewWorkspaceOption()
        {
            var workspaceFactoryMock = this.RegisterWorkspaceFactory();
            var outerWorkspace = new Mock<IWorkspace>();
            var innerWorkspace = new Mock<IWorkspace>();
            workspaceFactoryMock.Setup(factory => factory.GetWorkspaceInstance()).ReturnsInOrder(outerWorkspace.Object, innerWorkspace.Object);

            using (var outerScope = new WorkspaceScope(WorkspaceScopeOption.RequiresNew))
            {
                using (var innerScope = new WorkspaceScope(WorkspaceScopeOption.RequiresNew))
                {
                    Assert.AreNotEqual(
                        outerScope.CurrentWorkspace,
                        innerScope.CurrentWorkspace,
                        "The inner workspace must be the same instance like the outer workspace.");
                }
            }

            WorkspaceBuilder.Current.RemoveDefaultWorkspaceFactory();
        }

        [Test]
        public void ReleaseNestedWorkspaceScopesWithRequiresNewWorkspaceOption()
        {
            var workspaceFactoryMock = this.RegisterWorkspaceFactory();
            var outerWorkspace = new Mock<IWorkspace>();
            var innerWorkspace = new Mock<IWorkspace>();
            workspaceFactoryMock.Setup(factory => factory.GetWorkspaceInstance()).ReturnsInOrder(outerWorkspace.Object, innerWorkspace.Object);

            using (var outerScope = new WorkspaceScope(WorkspaceScopeOption.RequiresNew))
            {
                using (var innerScope = new WorkspaceScope(WorkspaceScopeOption.RequiresNew))
                {
                }
            }

            workspaceFactoryMock.Verify(factory => factory.ReleaseWorkspace(outerWorkspace.Object), Times.Once());
            workspaceFactoryMock.Verify(factory => factory.ReleaseWorkspace(innerWorkspace.Object), Times.Once());

            WorkspaceBuilder.Current.RemoveDefaultWorkspaceFactory();
        }

        [Test]
        public void NestedWorkspaceScopesWithRequiredWorkspaceOptionAsTypedWorkspace()
        {
            var workspaceFactoryMock = this.RegisterWorkspaceFactory();
            var outerWorkspace = new Mock<IWorkspace>();
            var innerWorkspace = new Mock<IWorkspace>();
            workspaceFactoryMock.Setup(factory => factory.GetWorkspaceInstance(typeof(MyWorkspace))).Returns(outerWorkspace.Object);
            workspaceFactoryMock.Setup(factory => factory.GetWorkspaceInstance(typeof(MyDifferentWorkspace))).Returns(innerWorkspace.Object);

            using (var outerScope = new WorkspaceScope(typeof(MyWorkspace), WorkspaceScopeOption.RequiresNew))
            {
                using (var innerScope = new WorkspaceScope(typeof(MyDifferentWorkspace)))
                {
                    Assert.AreNotEqual(
                        outerScope.CurrentWorkspace,
                        innerScope.CurrentWorkspace,
                        "The inner workspace must not be the same instance like the outer workspace.");
                }
            }

            WorkspaceBuilder.Current.RemoveDefaultWorkspaceFactory();
        }
        
        private Mock<IWorkspaceFactory> RegisterWorkspaceFactory()
        {
            var workspaceFactoryMock = new Mock<IWorkspaceFactory>() { DefaultValue = DefaultValue.Mock };
            WorkspaceBuilder.Current.RegisterDefaultWorkspaceFactory(workspaceFactoryMock.Object);
            return workspaceFactoryMock;
        }
    }

    internal class MyWorkspace { }
    internal class MyDifferentWorkspace { }
}
