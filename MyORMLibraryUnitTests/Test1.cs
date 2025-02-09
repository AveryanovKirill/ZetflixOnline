using Moq;
using MyORMLibrary;
using MyORMLibraryUnitTests.Models;
using System.Data;
using System.Data.SqlClient;

namespace MyORMLibraryUnitTests
{
    [TestClass]
    public sealed class Test1Context
    {
        [TestMethod]
        public void TestMethod1()
        {
            var dbConnection = new Mock<IDbConnection>();
            var dbCommand = new Mock<IDbCommand>();
            var dbDataReader = new Mock<IDataReader>();
            var person = new Person()
            { 
                Id = 1,
                Email = "test@mail.ru",
                Name = "Test",
            };
            var context = new TestContext<Person>(dbConnection.Object);

            dbDataReader.SetupSequence(x => x.Read())
                .Returns(true)
                .Returns(false);

            dbDataReader.Setup(x => x.GetBoolean(It.Is<int>(p => p == 1))).Returns(true);

            dbDataReader.Setup(r => r["Id"]).Returns(person.Id);
            dbDataReader.Setup(r => r["Email"]).Returns(person.Email);
            dbDataReader.Setup(r => r["Name"]).Returns(person.Name);

            dbConnection.Setup(c => c.CreateCommand()).Returns(dbCommand.Object);

            var result = context.GetById(person.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(person.Id, result.Id);
            Assert.AreEqual(person.Name, result.Name);
            Assert.AreEqual(person.Email, result.Email);
        }
    }
}
