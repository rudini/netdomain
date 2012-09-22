namespace netdomain
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Transactions;

    using NUnit.Framework;

    using netdomain.Abstract;

    [TestFixture]
    public class DbConnectionExtensionFixture
    {
        [Test]
        public void ExecuteOnConnectionManager_WhenAPersonHasBeenInserted_ShouldReturnThatPersonMappedToATestPersonObject()
        {
            using (var tx = new TransactionScope())
            {
                // Arrange
                IConnectionManager connectionManager =
                    new ConnectionManagerStub(
                        new SqlConnection(
                            @"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\App_Data\LinqTest.mdf;" 
                            + @"Integrated Security=True;User Instance=True"));

                connectionManager.ExecuteNonQuery(
                    "INSERT INTO person (name, beruf) values ('Peter', 'Tester' )");

                // Act
                var person = connectionManager.Execute<TestPerson>(
                    "SELECT Id, Version, Name, Beruf FROM person WHERE Name = @p1", 
                    new Parameter { Name = "@p1", Value = "Peter" }).First();

                // Assert
                Assert.AreEqual("Peter", person.Name);
            }
        }
    }

    public class TestPerson
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The object id.</value>
        public virtual int Id { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version field.</value>
        public virtual Byte[] Version { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The persons name.</value>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the beruf.
        /// </summary>
        /// <value>The persons beruf.</value>
        public virtual string Beruf { get; set; }
    }

    public class ConnectionManagerStub : IConnectionManager
    {
        private readonly IDbConnection connection;

        public ConnectionManagerStub(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IDbConnection Connection
        {
            get { return this.connection; }
        }

        public bool IsConnected
        {
            get { return this.connection.State == ConnectionState.Open; }
        }

        public IDbConnection Disconnect()
        {
            this.connection.Close();
            return this.connection;
        }

        public IDbConnection Reconnect()
        {
            this.connection.Open();
            return this.connection;
        }
    }
}