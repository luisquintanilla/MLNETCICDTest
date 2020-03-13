using System;
using Xunit;
using Xunit.Abstractions;
using Microsoft.ML;
using static Domain.Schema;
using System.Runtime.CompilerServices;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestModel
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _output;
        private readonly MLContext _mlContext;
        private readonly string _modelUri;
        private readonly HttpClient _client;

        public UnitTest1(ITestOutputHelper output)
        {
            _output = output;
            _client = new HttpClient();
            _mlContext = new MLContext();
            _modelUri = Environment.GetEnvironmentVariable("MODEL", EnvironmentVariableTarget.Machine);
        }
        
        [Fact]
        public async Task ModelLoads()
        {
            Stream modelStream = await _client.GetStreamAsync(_modelUri);
            ITransformer model = _mlContext.Model.Load(modelStream, out DataViewSchema inputSchema);
            _output.WriteLine(model.GetType().ToString());
            Assert.True(typeof(ITransformer).IsInstanceOfType(model));
        }

        [Fact]
        public async Task AccuracyAtOrAbove60Percent()
        {

            var data = new ModelInput[]
            {
                new ModelInput("This is nice", true),
                new ModelInput("This is terrible", false),
                new ModelInput("This is cool", true),
                new ModelInput("This is bad", false),
                new ModelInput("This is awesome", true)
            };

            IDataView testDataView = _mlContext.Data.LoadFromEnumerable(data);

            Stream modelStream = await _client.GetStreamAsync(_modelUri);
            ITransformer model = _mlContext.Model.Load(modelStream, out DataViewSchema inputSchema);

            IDataView predictionDataView = model.Transform(testDataView);

            var modelMetrics = _mlContext.BinaryClassification.Evaluate(predictionDataView);

            _output.WriteLine($"Accuracy: {modelMetrics.Accuracy}");
            Assert.True(modelMetrics.Accuracy > 0.5);
        }
    }
}
