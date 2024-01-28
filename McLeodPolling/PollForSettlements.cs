using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using SettlementPolling;
using SettlementPolling.utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SettlementPolling
{
    public class Settlement
    {
        public Settlement() { }
        public string company_id { get; set; }
        public string movement_id { get; set; }
        public string order_id { get; set; }
        public string payee_id { get; set; }
        public decimal total_pay { get; set; }
        
    }
   
    public class PollForSettlements
    {
        
        IHttpClientFactory _httpClientFactory;
        IConfiguration _config;
        ServiceBusClient _messageClient;
        public PollForSettlements(IHttpClientFactory httpClientFactory, IConfiguration config, ServiceBusClient messageClient) 
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _messageClient = messageClient;
        }
        [FunctionName("PollForSettlements")]
        public async Task Run([TimerTrigger("0 */1 * * * *", RunOnStartup = true)]TimerInfo myTimer, ILogger loh)
        {
            
            var baseUrl = $"https://tms.christensonloadtracking.com:5790/";
            var settlementsUrl = @"ws/settlements/search?settlement.transfer_date=>=1/7/2024";
            var httpClientHelper = new HTTPClientHelper(_httpClientFactory, "SettlementsService", baseUrl);

            var settlements = await httpClientHelper.GetAsync<List<Settlement>>(baseUrl + settlementsUrl);

            var messages = SettlementMessage.Create(settlements);
            
            await EnqueueSettlements(messages);
           

        }
        public async Task EnqueueSettlements(List<SettlementMessage> messages)
        {
            var sender = _messageClient.CreateSender("settlements");
            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();
            
            messages.ForEach(s => {
               messageBatch.TryAddMessage(new ServiceBusMessage(JsonConvert.SerializeObject(s)));
            });

            await sender.SendMessagesAsync(messageBatch);
        }
    }
}
