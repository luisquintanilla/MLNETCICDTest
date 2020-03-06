using System;
using Xunit;
using Xunit.Abstractions;
using Microsoft.ML;
using static Domain.Schema;
using System.Runtime.CompilerServices;

namespace TestModel
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _output;

        public UnitTest1(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public void AccuracyAbove70Percent()
        {

            var data = new ModelInput[]
            {
                new ModelInput("This is nice", true),
                new ModelInput("This is terrible", false),
                new ModelInput("This is cool", true),
                new ModelInput("This is bad", false),
                new ModelInput("This is awesome", true)
            };

            MLContext mlContext = new MLContext();

            IDataView testDataView = mlContext.Data.LoadFromEnumerable(data);

            ITransformer model = mlContext.Model.Load("model.zip", out DataViewSchema inputSchema);

            IDataView predictionDataView = model.Transform(testDataView);

            var modelMetrics = mlContext.BinaryClassification.Evaluate(predictionDataView);

            _output.WriteLine($"Accuracy: {modelMetrics.Accuracy}");
            Assert.True(modelMetrics.Accuracy > 0.5);
        }
    }
}
