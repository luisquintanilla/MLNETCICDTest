using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.ML;
using static Domain.Schema;

namespace TrainConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Initialize MLContext
            MLContext mlContext = new MLContext();

            // Create in-memory data
            //var data = new ModelInput[]
            //{
            //    new ModelInput("This is nice", true),
            //    new ModelInput("This is terrible", false),
            //    new ModelInput("This is cool", true),
            //    new ModelInput("This is bad", false),
            //    new ModelInput("This is awesome", true)
            //};

            var data = await GetDataAsync(args[0]);

            // Create DataView
            IDataView dv = mlContext.Data.LoadFromEnumerable(data);

            Console.WriteLine("Data Loaded");

            // Define training pipeline
            IEstimator<ITransformer> pipeline =
                mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: "Comment")
                .Append(mlContext.BinaryClassification.Trainers.LbfgsLogisticRegression());

            // Train model
            ITransformer model = pipeline.Fit(dv);

            Console.WriteLine("Model Trained");

            // Save the model
            mlContext.Model.Save(model, dv.Schema, "model.zip");

            Console.WriteLine("Model Saved");
        }

        private static async Task<IEnumerable<ModelInput>> GetDataAsync(string url)
        {
            IEnumerable<ModelInput> data;
            using(var client = new HttpClient())
            {
                var rawdata = await client.GetStringAsync(url);
                var lines = rawdata.Split('\n');
                data =
                    lines.Select(line =>
                    {
                        var columns = line.Split('\t');
                        if (columns[0] != "")
                        {
                            return new ModelInput
                            {
                                Comment = columns[0],
                                Sentiment = Convert.ToBoolean(Int16.Parse(columns[1]))
                            };
                        }
                        else
                        {
                            return new ModelInput();
                        }
                    })
                    .Where(input => input.Comment != default || input.Sentiment != default);
            }

            return data;
        }
    }
}
