using FunctionApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;

namespace FunctionApp
{
    public class AddTimeEntry
    {
        private readonly IDataverseService dvService;
        public AddTimeEntry(IDataverseService dataverseService)
        {
            dvService = dataverseService;
        }

        [FunctionName("AddTimeEntry")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");
                if (dvService.IsReady == null || !dvService.IsReady.Value)
                    return new InternalServerErrorResult();

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<ReqData>(requestBody);
                string[] validationResult = data.GetValidationResult();
                if (validationResult.Length > 0)
                {
                    return new OkObjectResult(string.Join(", ", validationResult));
                }                               
                // dataverseService.CleanRecords();
               
                if (dvService.HasDublicate(data.StartOn.Value, data.EndOn.Value))
                {
                    Console.WriteLine($"The row already exists with the same start: {data.StartOn} and end: {data.EndOn}");
                    return new OkObjectResult($"The row already exists with the same start: {data.StartOn} and end: {data.EndOn}");
                } else
                {
                    dvService.AddEntity(data.StartOn.Value, data.EndOn.Value);
                }
                //dvService.CheckDublicates();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return new OkObjectResult("Entity created!");
        }      
    }
}
