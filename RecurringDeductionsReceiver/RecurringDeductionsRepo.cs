using Microsoft.Azure.Cosmos;
using Microsoft.VisualBasic;
using RecurringDeductionsReceiver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SettlementEvaluator
{
    public interface IRecurringDeductionRepo
    {
        Task<RecurringDeduction> Get(string id);
        Task Save(RecurringDeduction trace);
    }
    public class RecurringDeductionRepo : IRecurringDeductionRepo
    {
        private string _conn;
        CosmosClient _client;
        Container container;
        public RecurringDeductionRepo(string connectionString, CosmosClient client)
        {
            _conn = connectionString;
            //client = new CosmosClient(_conn);
            _client = client;
            container = _client.GetContainer("Fleetly", "RecurringDeductions");
        }

        public async Task<RecurringDeduction> Get(string id)
        {
            ItemResponse<RecurringDeduction> ret = await container.ReadItemAsync<RecurringDeduction>(id, new PartitionKey("id"));//make sure this partition key is right if this doesn't work 

            return ret.Resource;
        }

        public async Task<List<RecurringDeduction>> GetByPayeeId(string id)
        {

            var q = new QueryDefinition(query: "select * from RecurringDeductions r where r.payee_id = @payee_id").WithParameter("@payee_id", id);

            using FeedIterator<RecurringDeduction> feed = container.GetItemQueryIterator<RecurringDeduction>(
                queryDefinition: q
            );

            List<RecurringDeduction> items = new();
            double requestCharge = 0d;
            while (feed.HasMoreResults)
            {
                FeedResponse<RecurringDeduction> response = await feed.ReadNextAsync();
                foreach (RecurringDeduction item in response)
                {
                    items.Add(item);
                }
                requestCharge += response.RequestCharge;
            }

            return items;
        }

        public Task Save(RecurringDeduction deduction)
        {
            container.UpsertItemAsync(deduction);
            return Task.CompletedTask;
        }
    }

}
