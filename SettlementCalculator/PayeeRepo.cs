using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SettlementEvaluator
{
    public class Payee
    { 
        public string id { get; set; }
        public string freeze_pay { get; set; }
        public drsPayee drspayee { get; set; }
    }
    public class drsPayee
    { 
        public decimal taxable_owed { get; set; }
    }
    public interface IPayeeRepo
    {
        Task<Payee> Get(string id);
        Task Save(Payee payee);
    }
    public class PayeeRepo : IPayeeRepo
    {
        IHttpClientFactory _httpClientFactory;
        string baseUrl = $"https://tms.christensonloadtracking.com:5790/";
        public PayeeRepo(IHttpClientFactory httpClientFacotry)
        {
            _httpClientFactory = httpClientFacotry;
        }
        public async Task<Payee> Get(string id)
        {

            var queryUrl = $"ws/payees/{id}";

            var httpClientHelper = new utils.HTTPClientHelper(_httpClientFactory, "DeductionsService", baseUrl);

            return await httpClientHelper.GetAsync<Payee>(baseUrl + queryUrl);
        }

        public async Task Save(Payee payee)
        {
            throw new NotImplementedException();
        }
    }
}
