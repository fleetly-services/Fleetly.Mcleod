using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos;

namespace RecurringDeductionsReceiver
{
    public class RecurringDeductionReceiver
    {
        IRecurringDeductionRepo _repo;
        public RecurringDeductionReceiver(IRecurringDeductionRepo repo) 
        {
            _repo = repo;
        }
        [FunctionName("RecurringDeductionUpdate")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<RecurringDeduction>(requestBody);

                await _repo.Save(data);
                return new OkObjectResult("Succesful");

            }
            catch (Exception ex) 
            {
                return new UnprocessableEntityObjectResult("Invalid Recurring Deduction Format.");
            }
        }
    }
}
