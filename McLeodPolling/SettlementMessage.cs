using SettlementPolling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettlementPolling
{
    public class SettlementMessage
    {
        public SettlementMessage() { }
        public string CarrierID { get; set; }
        public string PayeeId { get; set; }
        public List<Settlement> Settlements { get; set; }


        public static List<SettlementMessage> Create(List<Settlement> settlements)
        {
            var settlementMessages = new List<SettlementMessage>();
            var payeeIds = settlements.Select(x => x.payee_id).ToList();
            payeeIds.ForEach(p =>
            {
                var message = new SettlementMessage();
                message.CarrierID = "CHNS"; //TODO: temporary until we decide on carrier setup
                message.PayeeId = p;
                message.Settlements = settlements.Where(s => s.payee_id == p).ToList();
                settlementMessages.Add(message);
            });

            return settlementMessages;
        }
    }
}
