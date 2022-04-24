using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;
using Microsoft.Xrm.Sdk.Messages;
using System.Collections.Generic;

namespace FunctionApp.Services
{
    public class DataverseService : IDataverseService, IDisposable
    {
        private ServiceClient Service;
        
        private readonly string ConnectionString;
        private static Guid ruleId;
        private readonly string TimeEntityName = "msdyn_timeentry";
        private readonly HashSet<(DateTime start, DateTime end)> timesSet;
        public DataverseService()
        {
            timesSet = new HashSet<(DateTime start, DateTime end)>();
            ConnectionString = Environment.GetEnvironmentVariable("MyConnectionString");
            Console.WriteLine("Init DataverseService...");
            Configure();          
        }

        public bool? IsReady => Service?.IsReady;

        public void Configure()
        {
            try
            {
                Service = new ServiceClient(ConnectionString);
                if (Service.IsReady)
                {
                    WhoAmI();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void CleanRecords()
        {
            var querybyattribute = new QueryByAttribute(TimeEntityName);
            querybyattribute.ColumnSet = new ColumnSet("msdyn_type");

            //  Attribute to query.
            querybyattribute.Attributes.AddRange("msdyn_type");

            //  Value of queried attribute to return.
            querybyattribute.Values.AddRange(192350000); // type work (default)

            //  Query passed to service proxy.
            EntityCollection retrieved = Service.RetrieveMultiple(querybyattribute);
            Console.WriteLine("Entities count: " + retrieved.Entities.Count);
            foreach (var item in retrieved.Entities)
            {
                Console.WriteLine("Deleteting " + item["msdyn_timeentryid"]);
                Service.Delete(TimeEntityName, Guid.Parse(item["msdyn_timeentryid"].ToString()));
            }
        }

        public bool HasDublicate(DateTime start, DateTime end)
        {
            if (timesSet.Contains((start, end))) return true;

            var querybyattribute = new QueryByAttribute(TimeEntityName);
            querybyattribute.ColumnSet = new ColumnSet("msdyn_start", "msdyn_end");

            //  Attribute to query.
            querybyattribute.Attributes.AddRange("msdyn_start", "msdyn_end");

            //  Value of queried attribute to return.
            querybyattribute.Values.AddRange(start, end);

            //  Query passed to service proxy.
            EntityCollection retrieved = Service.RetrieveMultiple(querybyattribute);
            return retrieved.Entities.Count > 0;
        }
        public void AddEntity(DateTime start, DateTime end)
        {
            var entity = new Entity(TimeEntityName);
            entity["msdyn_start"] = start;
            entity["msdyn_end"] = end;

            var createReq = new CreateRequest() { Target = entity };
            createReq.Parameters.Add("SuppressDuplicateDetection", false);

            var response = (CreateResponse)Service.Execute(createReq);
            Guid id = response.id;
            Console.WriteLine(id + " created");
            timesSet.Add((start, end));
        }

        public void ConfigureDublicateDetection()
        {
            var duplicateRule = new DuplicateRule
            {
                Name = $"DuplicateRule: for entiry {TimeEntityName}",
                BaseEntityName = TimeEntityName,
                MatchingEntityName = TimeEntityName
            };
            ruleId = Service.Create(duplicateRule);

            // Create a duplicate detection rule condition
            DuplicateRuleCondition dupCondition = new DuplicateRuleCondition
            {
                BaseAttributeName = "msdyn_start",
                MatchingAttributeName = "msdyn_start",
                OperatorCode = new OptionSetValue(3), // Same date.
                RegardingObjectId = new EntityReference(DuplicateRule.EntityLogicalName, ruleId),
            };
            DuplicateRuleCondition dupCondition2 = new DuplicateRuleCondition
            {
                BaseAttributeName = "msdyn_end",
                MatchingAttributeName = "msdyn_end",
                OperatorCode = new OptionSetValue(3), // Same date.
                RegardingObjectId = new EntityReference(DuplicateRule.EntityLogicalName, ruleId),


            };
            Guid conditionId = Service.Create(dupCondition);
            Service.Create(dupCondition2);

            Console.Write("'{0}' created, ", duplicateRule.Name);

            // Execute the publish request.
            var response = (PublishDuplicateRuleResponse)Service.Execute(new PublishDuplicateRuleRequest() { DuplicateRuleId = ruleId });

            // When the publishDuplicateRule request returns, the state of the rule will still be "Publishing" (StatusCode = 1).
            // we need to wait for the publishing operation to complete, so we keep polling the state of the
            // rule until it becomes "Published" (StatusCode = 2).
            int i = 0;
            var retrievedRule = (DuplicateRule)Service.Retrieve(DuplicateRule.EntityLogicalName, ruleId, new ColumnSet(new String[] { "statuscode" }));
            while (retrievedRule.StatusCode.Value == 1 && i < 20)
            {
                i++;
                System.Threading.Thread.Sleep(1000);
                retrievedRule = 
                    (DuplicateRule)Service.Retrieve(DuplicateRule.EntityLogicalName, ruleId, new ColumnSet(new String[] { "statuscode" }));
            }

            Console.Write("published.\n");
        }

        public void WhoAmI()
        {
            WhoAmIResponse whoAmIResponse = (WhoAmIResponse)Service.Execute(new WhoAmIRequest());
            Console.WriteLine($"Connected with UserId: {whoAmIResponse.UserId}");
        }

        public void CheckDublicates(DateTime start, DateTime end) {
            var entity = new Entity(TimeEntityName);
            entity["msdyn_start"] = start;
            entity["msdyn_end"] = end;

            var request = new RetrieveDuplicatesRequest()
            {
                BusinessEntity = entity,
                MatchingEntityName = entity.LogicalName,
                PagingInfo = new PagingInfo() { PageNumber = 1, Count = 50 }
            };

            var response = (RetrieveDuplicatesResponse)Service.Execute(request);
            if (response.DuplicateCollection.Entities.Count >= 1)
            {
                Console.WriteLine("{0} Duplicate rows found.", response.DuplicateCollection.Entities.Count);
            }
        }            

        public void Dispose()
        {
            Service?.Dispose();
        }
    }
}
