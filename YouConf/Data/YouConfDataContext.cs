using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YouConf.Data.Entities;

namespace YouConf.Data
{
    public class YouConfDataContext : YouConf.Data.IYouConfDataContext
    {

        public YouConfDataContext()
        {

            var tableClient = GetTableClient();
            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("Conferences");
            table.CreateIfNotExists();
        }

        private CloudTableClient GetTableClient()
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
               CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the table client.
            return storageAccount.CreateCloudTableClient();
        }

        private CloudTable GetTable(string tableName)
        {
            var tableClient = GetTableClient();
            return tableClient.GetTableReference(tableName);
        }

        public IEnumerable<Conference> GetAllConferences()
        {

            //TODO: Yes I know that this will result in an unbounded select, however, once we start getting
            //a decent number of conferences we'll change how this works so it does a filter of some sort (TBD)
            var table = GetTable("Conferences");
            TableQuery<AzureTableEntity> query = new TableQuery<AzureTableEntity>();
            var conferences = table.ExecuteQuery(query);
            return conferences.Select(x =>  JsonConvert.DeserializeObject<Conference>(x.Entity));
        }

        public Conference GetConference(string hashTag)
        {
            //TODO: Yes I know that this will result in an unbounded select, however, once we start getting
            //a decent number of conferences we'll change how this works so it does a filter of some sort (TBD)
            var table = GetTable("Conferences");
            TableQuery<AzureTableEntity> query = new TableQuery<AzureTableEntity>();
            TableOperation retrieveOperation = TableOperation.Retrieve<AzureTableEntity>("Conferences", hashTag);
            TableResult retrievedResult = table.Execute(retrieveOperation);
            if (retrievedResult.Result == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<Conference>(((AzureTableEntity)(retrievedResult.Result)).Entity);
        }

        public void UpsertConference(Conference conference)
        {
            //Wrap the conference in our custom AzureTableEntity
            var table = GetTable("Conferences");
            var entity = new AzureTableEntity()
            {
                PartitionKey = "Conferences",
                RowKey = conference.HashTag,
                Entity = JsonConvert.SerializeObject(conference)
            };

            TableOperation upsertOperation = TableOperation.InsertOrReplace(entity);

            // Insert or update the conference
            table.Execute(upsertOperation);
        }
    }
}