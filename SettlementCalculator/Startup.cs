
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Azure.Messaging.ServiceBus.Administration;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Cosmos;

[assembly: FunctionsStartup(typeof(SettlementEvaluator.Startup))]

namespace SettlementEvaluator
{
    
    public class Startup : FunctionsStartup
    {   public IConfiguration config { get; }
        public Startup() { }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = builder.GetContext().Configuration;
            //builder.Services.AddSingleton<IMyService>((s) => {
            //    return new MyService();
            //});
            //builder.Services.AddSingleton<ILoggerProvider, MyLoggerProvider>();

            builder.Services.AddHttpClient();
            builder.Services.AddSingleton(new ServiceBusClient(config.GetConnectionStringOrSetting("Settlements")));
            builder.Services.AddSingleton(new CosmosClient(config.GetConnectionStringOrSetting("Cosmos")));
            builder.Services.AddScoped<IPendingDeductionsRepo, PendingDeductionsRepo>();
            builder.Services.AddScoped<IRecurringDeductionRepo, RecurringDeductionRepo>();
            builder.Services.AddScoped<IDeductionCodesRepo, DeductionCodesRepo>();
            builder.Services.AddScoped<ICarrierProfileRepo, CarrierProfileRepo>();
            builder.Services.AddScoped<IPayeeRepo, PayeeRepo>();
            builder.Services.AddScoped<ISettlementEvaluator, SettlementEvaluator>();
        }
    }
}
