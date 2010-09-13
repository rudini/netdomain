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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using netdomain.Abstract;

    /// <summary>
    /// Summary description for WorkspaceScopeFixture
    /// </summary>
    [TestClass]
    public class WorkspaceBuilderFixture
    {
        public static Mock<IWorkspace> WorkspaceMock = new Mock<IWorkspace> { DefaultValue = DefaultValue.Mock };

        /// <summary>
        /// Gets or sets the test context which provides 
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

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

        [TestMethod]
        public void CanRegisterWorkspaceFactory()
        {
            var workspaceFactoryMock = new Mock<IWorkspaceFactory> { DefaultValue = DefaultValue.Mock };
            WorkspaceBuilder.Current.RegisterDefaultWorkspaceFactory(workspaceFactoryMock.Object);

            Assert.AreEqual(workspaceFactoryMock.Object, WorkspaceBuilder.Current.GetWorkspaceFactory());

            WorkspaceBuilder.Current.RemoveDefaultWorkspaceFactory();
        }

        [TestMethod]
        public void CanRegisterWorkspaceFactoryByType()
        {
            WorkspaceBuilder.Current.RegisterDefaultWorkspaceFactory<WorkspaceFactory>();

            Assert.AreEqual(WorkspaceMock.Object, WorkspaceBuilder.Current.GetWorkspaceFactory().GetWorkspaceInstance());

            WorkspaceBuilder.Current.RemoveDefaultWorkspaceFactory();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterWorkspaceBuilderToNull()
        {
            WorkspaceBuilder.Current.RegisterDefaultWorkspaceFactory(null);
            Assert.Fail("The RegisterWorkspaceFactory must fail.");
        }

        [TestMethod]
        public void RegisterExtension()
        {
            WorkspaceBuilder.Current.AddExtension<TestExtension>();
            WorkspaceBuilder.Current.RemoveExtension<TestExtension>();
        }

        [TestMethod]
        public void RemoveARegisteredExtension()
        {
            WorkspaceBuilder.Current.AddExtension<TestExtension>();
            WorkspaceBuilder.Current.RemoveExtension<TestExtension>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RemoveANotRegisteredExtension()
        {
            WorkspaceBuilder.Current.RemoveExtension<TestExtension>();
            Assert.Fail("The test must fail here.");
        }

        [TestMethod]
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
