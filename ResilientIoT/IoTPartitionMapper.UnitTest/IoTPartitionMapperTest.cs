using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using IotPartitionMapService;

namespace IotPartitionMapService.Tests
{

    [TestClass]
    public class IoTPartitionMapperTest
    {

        [TestInitialize]
    public void TestInitialize()
    {

            
    }
        [TestMethod]
        public void TestMethod0()
        {
            Assert.AreEqual(1, 1);

        }
        [TestMethod]
        public void TestMethod1()
        {
            var contextMock = new Mock<System.Fabric.StatefulServiceContext>();
            var stateManagerMock = new Mock<Microsoft.ServiceFabric.Data.ReliableStateManager>();
            var mock = new Mock<IoTPartitionMapperService>(contextMock, stateManagerMock);

            Assert.AreEqual(1, 1);


        }
    }
}
