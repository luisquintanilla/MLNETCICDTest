using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzFnUploadService
{
    public static class Function2
    {
        [FunctionName("UploadValidation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [Blob("validation/model.zip", FileAccess.ReadWrite, Connection = "AzModelStorageConnectionString")] CloudBlockBlob savedFile,
            ILogger log)
        {
            var file = req.Form.Files[0];

            await savedFile.UploadFromStreamAsync(file.OpenReadStream());
            
            return new OkObjectResult("Uploaded successfully");
        }
    }
}
