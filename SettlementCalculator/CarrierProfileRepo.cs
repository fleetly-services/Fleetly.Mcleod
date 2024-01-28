
using Microsoft.Azure.Cosmos;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SettlementEvaluator
{
    public interface ICarrierProfileRepo
    {
        Task<CarrierProfile> Get(string id);
        Task Save(CarrierProfile carrier);
    }
    public class CarrierProfileRepo : ICarrierProfileRepo
    {
        private string _conn;
        CosmosClient _client;
        Container container;
        public CarrierProfileRepo(CosmosClient client)
        {
            _client = client;
            container = _client.GetContainer("Fleetly", "CarrierProfiles");
        }

        public async Task<CarrierProfile> Get(string id)
        {
            try
            {
                ItemResponse<CarrierProfile> ret = await container.ReadItemAsync<CarrierProfile>(id, new PartitionKey(id));//make sure this partition key is right if this doesn't work 

                return ret.Resource;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public Task Save(CarrierProfile carrier)
        {
            container.UpsertItemAsync(carrier);
            return Task.CompletedTask;
        }
    }

}
