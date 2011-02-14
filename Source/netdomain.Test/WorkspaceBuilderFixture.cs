//-------------------------------------------------------------------------------
// <copyright file="WorkspaceBuilderFixture.cs" company="bbv Software Services AG">
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Moq;
    using netdomain.Abstract;

    using NUnit.Framework;

    /// <summary>
    /// Summary description for WorkspaceScopeFixture
    /// </summary>
    [TestFixture]
    public class WorkspaceBuilderFixture
    {
        public static Mock<IWorkspace> WorkspaceMock = new Mock<IWorkspace> { DefaultValue = DefaultValue.Mock };

        [Test]
        public void CanRegisterWorkspaceFactory()
        {
            var workspaceFactoryMock = new Mock<IWorkspaceFactory> { DefaultValue = DefaultValue.Mock };
            WorkspaceBuilder.Current.RegisterDefaultWorkspaceFactory(workspaceFactoryMock.Object);

            Assert.AreEqual(workspaceFactoryMock.Object, WorkspaceBuilder.Current.GetWorkspaceFactory());

            WorkspaceBuilder.Current.RemoveDefaultWorkspaceFactory();
        }

        [Test]
        public void CanRegisterWorkspaceFactoryByType()
        {
            WorkspaceBuilder.Current.RegisterDefaultWorkspaceFactory<WorkspaceFactory>();

            Assert.AreEqual(WorkspaceMock.Object, WorkspaceBuilder.Current.GetWorkspaceFactory().GetWorkspaceInstance());

            WorkspaceBuilder.Current.RemoveDefaultWorkspaceFactory();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterWorkspaceBuilderToNull()
        {
            WorkspaceBuilder.Current.RegisterDefaultWorkspaceFactory(null);
            Assert.Fail("The RegisterWorkspaceFactory must fail.");
        }

        [Test]
        public void RegisterExtension()
        {
            WorkspaceBuilder.Current.AddExtension<TestExtension>();
            WorkspaceBuilder.Current.RemoveExtension<TestExtension>();
        }

        [Test]
        public void RemoveARegisteredExtension()
        {
            WorkspaceBuilder.Current.AddExtension<TestExtension>();
            WorkspaceBuilder.Current.RemoveExtension<TestExtension>();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RemoveANotRegisteredExtension()
        {
            WorkspaceBuilder.Current.RemoveExtension<TestExtension>();
            Assert.Fail("The test must fail here.");
        }

        [Test]
        public void GetAllRegisteredExtensionInstances()
        {
            WorkspaceBuilder.Current.AddExtension<TestExtension>();
            IEnumerable<IWorkspaceExtension> extension = WorkspaceBuilder.Current.GetExtensionInstances();

            Assert.AreEqual(1, extension.Count());
            Assert.AreEqual(typeof(TestExtension), extension.First().GetType());

            WorkspaceBuilder.Current.RemoveExtension<TestExtension>();
        }
    }

    public class TestExtension : WorkspaceExtension
    {
    }

    public class WorkspaceFactory : IWorkspaceFactory
    {
        public IWorkspace GetWorkspaceInstance()
        {
            return WorkspaceBuilderFixture.WorkspaceMock.Object;
        }

        public IWorkspace GetWorkspaceInstance(Type workspaceType)
        {
            throw new NotImplementedException();
        }

        public void ReleaseWorkspace(IWorkspace workspace)
        {
            throw new NotImplementedException();
        }
    }
}
