using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.ML;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using static Domain.Schema;
using AzFnSentimentAnalysisService;
using System;

[assembly: FunctionsStartup(typeof(Startup))]
namespace AzFnSentimentAnalysisService
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddPredictionEnginePool<ModelInput, ModelOutput>()
                .FromUri(Environment.GetEnvironmentVariable("MODEL_URI"));
        }
    }
}
