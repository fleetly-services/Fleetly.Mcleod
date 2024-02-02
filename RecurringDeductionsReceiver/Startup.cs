
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Microsoft.Azure.Cosmos;

[assembly: FunctionsStartup(typeof(RecurringDeductionsReceiver.Startup))]

namespace RecurringDeductionsReceiver
{
    
    public class Startup : FunctionsStartup
    {   public IConfiguration config { get; }
        public Startup() { }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = builder.GetContext().Configuration;

            builder.Services.AddHttpClient();
            builder.Services.AddSingleton(new CosmosClient(config.GetConnectionStringOrSetting("Cosmos")));
            builder.Services.AddScoped<IRecurringDeductionRepo, RecurringDeductionRepo>();
        }
    }
}
