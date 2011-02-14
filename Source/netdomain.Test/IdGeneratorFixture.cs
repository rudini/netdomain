//-------------------------------------------------------------------------------
// <copyright file="IdGeneratorFixture.cs" company="bbv Software Services AG">
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

    using netdomain.Abstract;
    using netdomain.IdGenerator;
    using netdomain.LinqToObjects;
    using netdomain.Workspace;

    using NUnit.Framework;

    [TestFixture]
    public class IdGeneratorFixture
    {
        [Test]
        public void CreateIdentifierForPoco()
        {
            // Arrange
            var test = new Test();
            IWorkspace ws = new InMemoryWorkspace();

            // Act 
            ws.GenerateId(test);

            // Assert
            Assert.AreNotEqual(default(Guid), test.Id);
        }

        [Test]
        public void DoNotChangeTheIdentifierIfAlreadyAssigned()
        {
            // Arrange
            var test = new Test();
            IWorkspace ws = new InMemoryWorkspace();

            // Act 
            ws.GenerateId(test);
            var generatedValue = test.Id;
            ws.GenerateId(test);

            // Assert
            Assert.AreEqual(generatedValue, test.Id);
        }
    }

    [IdentifierGenerator(typeof(GuidCombGenerator), "Id")]
    public class Test
    {
        public Guid Id { get; set; }
    }
}
