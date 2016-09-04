using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using SensorActor.Interfaces;
using System.Runtime.Serialization;
using System.ComponentModel;
using Common;

namespace SensorActor
{
    /// <remarks>
    /// This class represents a Sensor
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    public class SensorActor : TestableActor, ISensorActor
    {

        private readonly string STATE_NAME = "MyState";
        [DataContract]
        public sealed class ActorState
        {
            [DataMember]
            public double Temperature
            {
                get; set;
            }
            [DataMember]
            public int Index { get; set; }
        }

        [ReadOnly(true)]
        public async Task<int> GetIndexAsync()
        {
            var state = await this.StateManager.TryGetStateAsync<ActorState>(STATE_NAME);
            return state.Value.Index;
        }

        public async Task SetIndexAsync(int index)
        {
            var conditionalState = await this.StateManager.TryGetStateAsync<ActorState>(STATE_NAME);
            var state = conditionalState.Value;
            state.Index = index;
            await this.StateManager.SetStateAsync<ActorState>(STATE_NAME, state);
        }

        public async Task InitializeActorState()
        {
            ActorEventSource.Current.ActorMessage(this, "SensorActor activated.");
            var state = await this.StateManager.TryGetStateAsync<ActorState>("MyState");
            if (!state.HasValue)
            {
                var actorState = new ActorState() { Temperature = 0 };
                await this.StateManager.SetStateAsync<ActorState>("MyState", actorState);
            }
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            await InitializeActorState();
        }

        /// <summary>
        /// TODO: Replace with your own actor method.
        /// </summary>
        /// <returns></returns>
        Task<int> ISensorActor.GetCountAsync()
        {
            return this.StateManager.GetStateAsync<int>("count");
        }

        /// <summary>
        /// TODO: Replace with your own actor method.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        Task ISensorActor.SetCountAsync(int count)
        {
            // Requests are not guaranteed to be processed in order nor at most once.
            // The update function here verifies that the incoming count is greater than the current count to preserve order.
            return this.StateManager.AddOrUpdateStateAsync("count", count, (key, value) => count > value ? count : value);
        }
    }
}
