using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters.Internal;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SettlementEvaluator
{
    public class SettlementMessage
    {
        public SettlementMessage() { }
        public string CarrierId { get; set; }
        public string PayeeId { get; set; }
        public List<Settlement> Settlements { get; set; }
        public decimal TotalPay()
        {
            return this.Settlements.Sum(x => x.total_pay);
        }
    }
    public class Settlement
    {
        public Settlement() { }
        public string company_id { get; set; }
        public string movement_id { get; set; }
        public string order_id { get; set; }
        public string payee_id { get; set; }
        public decimal total_pay { get; set; }
        public DateTime delivery_date { get; set; }
    }

    public class SettlementReceiver
    {
        ISettlementEvaluator _eval;
        public SettlementReceiver(ISettlementEvaluator eval) 
        {
            _eval = eval;
        }
        [FunctionName("SettlementReceiver")]
        public async Task Run([ServiceBusTrigger("settlements", Connection = "Settlements")]string message, ILogger log, MessageReceiver messageReceiver)
        {
            var settlement = JsonConvert.DeserializeObject<SettlementMessage>(message);
            var eval = await _eval.Evaluate(settlement);
        }
    }
}
