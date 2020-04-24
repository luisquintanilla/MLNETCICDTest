using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.ML;
using static Domain.Schema;

namespace AzFnSentimentAnalysisService
{
    public class AnalyzeSentiment
    {
        private readonly PredictionEnginePool<ModelInput, ModelOutput> _predictionEnginePool;

        public AnalyzeSentiment(PredictionEnginePool<ModelInput, ModelOutput> predictionEnginePool)
        {
            _predictionEnginePool = predictionEnginePool;
        }

        [FunctionName("AnalyzeSentiment")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            //Parse HTTP Request Body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            ModelInput data = JsonConvert.DeserializeObject<ModelInput>(requestBody);
            log.LogInformation(requestBody);
            //Make Prediction
            ModelOutput prediction = _predictionEnginePool.Predict(data);

            //Convert prediction to string
            string sentiment = Convert.ToBoolean(prediction.PredictedSentiment) ? "Positive" : "Negative";

            //Return Prediction
            return (ActionResult)new OkObjectResult(sentiment);
        }
    }
}
