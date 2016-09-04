using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceFabric.Services.Remoting;

namespace IotPartitionMapService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    public class IoTPartitionMapperService : StatefulService, IService 
    {
        public IoTPartitionMapperService(StatefulServiceContext context)
            : base(context)
        { }
        public IoTPartitionMapperService(StatefulServiceContext context, IReliableStateManagerReplica replica) : base(context, replica)
        {

        }

        protected virtual Task<string[]> GetPartitionListAsync()
        {
            var eventHubClient = EventHubClient.CreateFromConnectionString("HostName = iote2e.azure - devices.net; SharedAccessKeyName = iothubowner; SharedAccessKey = Bsp4 + D5at3lTacsNaZPvx0FhVvrdDa8LGFzKS / B6zzQ = ", "messages / events");
            var partitions = eventHubClient.GetRuntimeInformation().PartitionIds;
            return Task.FromResult<string[]>(partitions);
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see http://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[0];
        }

        public async Task GetIoTHubPartitions()
        {
            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");
            using (var tx = this.StateManager.CreateTransaction()) {
                await myDictionary.TryAddAsync(tx, "1", 1);
                await tx.CommitAsync();
            }
        }

        public async Task SetUpIoTPartitionsAsync()
        {
            var partitions = await GetPartitionListAsync();
            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string[]>>("MyDictionary");
            using (var tx = this.StateManager.CreateTransaction())
            {
                await myDictionary.TryAddAsync(tx, "partitionList", partitions);
                await tx.CommitAsync();
            }
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(this, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
