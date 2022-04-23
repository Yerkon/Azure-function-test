﻿using FunctionApp2.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FunctionApp2
{
    public class HttpExample
    {
        private readonly IDataverseService dvService;
        public HttpExample(IDataverseService dataverseService)
        {
            dvService = dataverseService;
        }

        [FunctionName("HttpExample")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");
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
