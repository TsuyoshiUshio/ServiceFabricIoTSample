using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using IotPartitionMapService;
using Microsoft.ServiceFabric.Data;
using System.Fabric;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using System.Numerics;
using Mocks;
using System.Collections.Generic;

namespace IoTPartitionMapper.Tests
{
    public class MockIoTPartitionMapper : IoTPartitionMapperService
    {
        internal static ICodePackageActivationContext activationontext = new MockCodePackageActivationContext(
            "fabric:/someapp",
            "SomeAppType",
            "Code",
            "1.0.0.0",
            Guid.NewGuid().ToString(),
            @"C:\Log",
            @"C:\Temp",
            @"C:\Work",
            "ServiceManifest",
            "1.0.0.0"
            );

        public MockIoTPartitionMapper(StatefulServiceContext context) : base(context)
        {

        }
        public MockIoTPartitionMapper(StatefulServiceContext context, IReliableStateManagerReplica replica) : base(context, replica)
        {

        }
        protected override Task<string[]> GetPartitionListAsync()
        {
            return Task.FromResult<string[]>(new string[] { "0", "1", "2", "3" });
        }
        internal IReliableStateManager MockStateManager()
        {
            return this.StateManager;
        }


    }
    

    [TestClass]
    public class UnitTest1
    {
        private MockIoTPartitionMapper service;

        [TestInitialize]
        public void Initialize()
        {
            var context = new StatefulServiceContext(new NodeContext("192.168.11.10", new NodeId(new BigInteger(3), new BigInteger(1)), new BigInteger(1), "Node1", "some"), new Mock<ICodePackageActivationContext>().Object, "aaaType", new Uri("fabric:/aaa"), null, new Guid(), 1);
            IReliableStateManagerReplica replica = new MockReliableStateManager();
            service = new MockIoTPartitionMapper(context, replica);
        }
        [TestMethod]
        public async Task TestSetupStateManagerAsync()
        {
            await service.SetUpIoTPartitionsAsync();
            var state = service.MockStateManager();
            var dictionary = await state.GetOrAddAsync<IReliableDictionary<string, string[]>>("MyDictionary");
            var result = await dictionary.TryGetValueAsync(new MockTransaction(),  "partitionList");
            Console.WriteLine(result);
            var expected = new string[] { "0", "1", "2", "3" };
            for (var i = 0; i > result.Value.Length; i++)
            {
                Assert.AreEqual(expected[i], result.Value[i]);
            }
            // Assert.AreEqual(1, 1);
        }

  //      [TestMethod]
  //      public async Task TestMethodAsync()
  //      {
            //var context = new StatefulServiceContext(new NodeContext("192.168.11.10", new NodeId(new BigInteger(3), new BigInteger(1)), new BigInteger(1), "Node1", "some") , new Mock<ICodePackageActivationContext>().Object, "aaaType", new Uri("fabric:/aaa"), null, new Guid(), 1);
            //var stateManager = new Mock<IReliableStateManagerReplica>();
            //var dictionary = new Mock<IReliableDictionary<string, long>>(MockBehavior.Strict);
            //var dictionaryObject = dictionary.Object;
            //stateManager.Setup(x => x.GetOrAddAsync<IReliableDictionary<string, long>>("MyDictionary")).Returns(Task.FromResult(dictionaryObject));
            //var serviceMock = new IoTPartitionMapperService(context, stateManager.Object);
            //await serviceMock.GetIoTHubPartitions();
            //Assert.AreEqual(1, 1);


            
    //    }
    }
}
