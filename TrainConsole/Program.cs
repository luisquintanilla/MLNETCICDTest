using Microsoft.ML;
using System;
using static Domain.Schema;

namespace TrainConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //Initialize MLContext
            MLContext mlContext = new MLContext();

            // Create in-memory data
            var data = new ModelInput[]
            {
                new ModelInput("This is nice", true),
                new ModelInput("This is terrible", false),
                new ModelInput("This is cool", true),
                new ModelInput("This is bad", false),
                new ModelInput("This is awesome", true)
            };

            // Create DataView
            IDataView dv = mlContext.Data.LoadFromEnumerable(data);

            Console.WriteLine("Data Loaded");

            // Define training pipeline
            IEstimator<ITransformer> pipeline =
                mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName:"Comment")
                .Append(mlContext.BinaryClassification.Trainers.LbfgsLogisticRegression());

            // Train model
            ITransformer model = pipeline.Fit(dv);

            Console.WriteLine("Model Trained");

            // Save the model
            mlContext.Model.Save(model, dv.Schema, "model.zip");

            Console.WriteLine("Model Saved");
        }
    }
}
