using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SensorActor;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Mocks;
using System.Reflection;
using System.Threading.Tasks;
using IoTPartitionMapper.Tests;


namespace SensorActor.Test
{

    [TestClass]
    public class SensorActorTest
    {

        [TestMethod]
        public async Task TestMethod1Async()
        {
            var actor = await CreateSensorActor();
            await actor.SetIndexAsync(1);
            var actorState = await actor.StateManager.GetStateAsync<SensorActor.ActorState>("MyState");
            Assert.AreEqual(1, actorState.Index);
        }

        private static async Task<SensorActor> CreateSensorActor()
        {
            var target = new SensorActor();

            PropertyInfo idProperty = typeof(ActorBase).GetProperty("Id");
            idProperty.SetValue(target, new ActorId(Guid.NewGuid()));

            PropertyInfo stateManagerProperty = typeof(SensorActor).GetProperty("StateManager");
            stateManagerProperty.SetValue(target, new MockActorStateManager());
            await target.InitializeActorState();

            return target;
        }
    }


}
