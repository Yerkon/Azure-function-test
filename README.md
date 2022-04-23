# Sample: Insert records to Dataverse table

This sample shows how to connect Dataverse and some data manipulation 

## How to run this sample

Download or clone the repo so that you have a local copy.
(Optional) Edit the local.settings.json file to define a connection string specifying the Dataverse instance you want to connect to.

## What this sample does

This sample creates local Azure function that accepts json payload to create records in Datavserse. 

## How this sample works

### Setup

1. DataverseService - Singleton service to initialize connection to Dataverse and contains the logic for validation duplicates
2. HttpExample - Azure function, which handles requests.
3. https://github.com/microsoft/PowerPlatform-DataverseServiceClient - for connection with with Dataverse

### Demonstrate

After configuration, pass json payload to insert unique rows by StartOn, EndOn:
```
{
  "StartOn":"2022-04-23 10:56",
  "EndOn": "2022-04-23 14:57"
}
```

### Links
https://github.com/microsoft/PowerApps-Samples/tree/master/cds/orgsvc/C%23/RetrieveMultipleQueryByAttribute
