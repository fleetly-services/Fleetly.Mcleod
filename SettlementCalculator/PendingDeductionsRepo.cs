using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SettlementEvaluator
{

    public class PendingDeduction
    {
        public PendingDeduction() { }
        public string company_id { get; set; }
        public decimal amount { get; set; }
        public string payee_id { get; set; }
        public string status { get; set; }
        public DateTime loadpay_process_date { get; set; }
        public string ready_to_pay_flag { get; set; }
        public string deduction_type { get; set; }
        public string deduct_code_id { get; set; }
        public string recur_deduct_id { get; set; }
        public DateTime transaction_date { get; set; }
    }
    public interface IPendingDeductionsRepo
    {
        Task<PendingDeduction> Get(string id);
        Task Save(PendingDeduction trace);
        Task<List<PendingDeduction>> GetForSettlementCalc(string payeeId);
    }
    public class PendingDeductionsRepo : IPendingDeductionsRepo
    {
        IHttpClientFactory _httpClientFactory;
        string baseUrl = $"https://tms.christensonloadtracking.com:5790/";
        public PendingDeductionsRepo(IHttpClientFactory httpClientFacotry) 
        {
            _httpClientFactory= httpClientFacotry; 
        }
        public async  Task<PendingDeduction> Get(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<PendingDeduction>> GetForSettlementCalc(string payeeId)
        {

            var queryUrl = $"ws/deductions/search?drs_pending_deduct.payee_id={payeeId}&ready_to_pay_flag=Y";

            var httpClientHelper = new utils.HTTPClientHelper(_httpClientFactory, "DeductionsService", baseUrl);

            return await httpClientHelper.GetAsync<List<PendingDeduction>>(baseUrl + queryUrl);
        }

        public async Task Save(PendingDeduction trace)
        {
            throw new NotImplementedException();
        }
    }
}
