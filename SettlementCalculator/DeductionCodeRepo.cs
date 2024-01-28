using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SettlementEvaluator
{

    public class DeductionCode
    {
        public DeductionCode() { }
        public string company_id { get; set; }
        public string id { get; set; }
        public string code_type { get; set; }
    }
    public interface IDeductionCodesRepo
    {
        Task<DeductionCode> Get(string id);
        Task Save(DeductionCode trace);
        Task<List<DeductionCode>> GetAll();
    }
    public class DeductionCodesRepo : IDeductionCodesRepo
    {
        IHttpClientFactory _httpClientFactory;
        string baseUrl = $"https://tms.christensonloadtracking.com:5790/";
        public DeductionCodesRepo(IHttpClientFactory httpClientFacotry)
        {
            _httpClientFactory = httpClientFacotry;
        }
        public async Task<DeductionCode> Get(string id)
        {

            var queryUrl = $"ws/payroll/deductCodes?id={id}";

            var httpClientHelper = new utils.HTTPClientHelper(_httpClientFactory, "DeductionCodeService", baseUrl);

            return await httpClientHelper.GetAsync<DeductionCode>(baseUrl + queryUrl);
        }

        public async Task<List<DeductionCode>> GetAll()
        {

            var queryUrl = $"ws/payroll/deductCodes";

            var httpClientHelper = new utils.HTTPClientHelper(_httpClientFactory, "DeductionCodeService", baseUrl);

            return await httpClientHelper.GetAsync<List<DeductionCode>>(baseUrl + queryUrl);
        }

        public async Task Save(DeductionCode trace)
        {
            throw new NotImplementedException();
        }
    }
}
