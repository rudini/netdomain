//-------------------------------------------------------------------------------
// <copyright file="DataReaderWrapperFixture.cs" company="bbv Software Services AG">
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
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Transactions;

    using NUnit.Framework;

    [TestFixture]
    public class DataReaderWrapperFixture
    {
        private const string Id = "Id";

        private const string Message = "Message";

        private IEnumerable<LogMessage> testdata;

        [SetUp]
        public void Setup()
        {
            var logMessage = new LogMessage { Id = Id, Message = Message };
            this.testdata = new List<LogMessage> { logMessage };
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExceptionCallAMethodBeforeACallToRead()
        {
            var dataReader = new DataReaderWrapper(this.testdata);

            dataReader.GetValue(0);
            Assert.Fail("The test must fail because the enumeration has not been started.");
        }

        [Test]
        public void CorrectSequenceOfGetValueMethod()
        {
            var dataReader = new DataReaderWrapper(this.testdata);

            dataReader.Read();
            Assert.AreEqual(Id, dataReader.GetValue(0), "The value at index {0} must be {1}", 0, Id);
            Assert.AreEqual(Message, dataReader.GetValue(1), "The value at index {0} must be {1}", 1, Message);
        }

        [Test]
        public void GetValueByGetOrdinal()
        {
            var dataReader = new DataReaderWrapper(this.testdata);

            dataReader.Read();
            Assert.AreEqual(Id, dataReader.GetValue(dataReader.GetOrdinal(Id)), "The value with name {0} must be {1}", Id, Id);
            Assert.AreEqual(Message, dataReader.GetValue(dataReader.GetOrdinal(Message)), "The value at index {0} must be {1}", Message, Message);
        }

        [Test]
        public void WriteToServer()
        {
            var logMessageList = new List<LogMessage>();

            for (int i = 0; i < 100000; i++)
            {
                logMessageList.Add(new LogMessage { Id = i.ToString(), Message = "Test" + i.ToString() });
            }

            var sqlBulkCopy = new SqlBulkCopy(@"Data Source=(local);Initial Catalog=LINQTEST;Integrated Security=True") { BatchSize = 10000000, DestinationTableName = "Messages" };
            sqlBulkCopy.ColumnMappings.Add("Message", "Message");
            sqlBulkCopy.ColumnMappings.Add("Id", "Id");

            using (var tx = new TransactionScope())
            {
                var watch = new Stopwatch();
                watch.Reset();
                watch.Start();

                sqlBulkCopy.WriteToServer(logMessageList);
                watch.Stop();

                Console.WriteLine(@"Time elapsed = " + watch.Elapsed.Seconds);
            }
        }

        /// <summary>
        /// Test stub class.
        /// </summary>
        public class LogMessage
        {
            /// <summary>
            /// Gets or sets the id of the message.
            /// </summary>
            /// <value>The message id.</value>
            public string Id { get; set; }

            /// <summary>
            /// Gets or sets the message.
            /// </summary>
            /// <value>The message text.</value>
            public string Message { get; set; }
        }
    }
}
