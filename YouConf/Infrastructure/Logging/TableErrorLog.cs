using Elmah;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YouConf.Infrastructure.Logging
{
    /// <summary>
    /// Based on http://www.wadewegner.com/2011/08/using-elmah-in-windows-azure-with-table-storage/
    /// Updated for Azure Storage v2 SDK
    /// </summary>
    public class TableErrorLog : ErrorLog
    {
        private string connectionString;
        public const string TableName = "Errors";

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

        public override ErrorLogEntry GetError(string id)
        {
            var table = GetTable(TableName);
            TableQuery<ErrorEntity> query = new TableQuery<ErrorEntity>();
            TableOperation retrieveOperation = TableOperation.Retrieve<ErrorEntity>("", id);

            TableResult retrievedResult = table.Execute(retrieveOperation);
            if (retrievedResult.Result == null)
            {
                return null;
            }

            return new ErrorLogEntry(this, id, ErrorXml.DecodeString(((ErrorEntity)retrievedResult.Result).SerializedError));
        }

        public override int GetErrors(int pageIndex, int pageSize, IList errorEntryList)
        {
            var count = 0;
            var table = GetTable(TableName);
            TableQuery<ErrorEntity> query = new TableQuery<ErrorEntity>()
            .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, TableName))
            .Take((pageIndex + 1) * pageSize);

            //NOTE: Ideally we'd use a continuation token for paging, as currently we're retrieving all errors back  
            //then paging in-memory. Running out of time though so have to leave it as-is for now (which is how it was originally)
            var errors = table.ExecuteQuery(query)
                .Skip(pageIndex * pageSize);
            foreach (var error in errors)
            {
                errorEntryList.Add(new ErrorLogEntry(this, error.RowKey,
                    ErrorXml.DecodeString(error.SerializedError)));
                count += 1;
            }
            return count;
        }

        public override string Log(Error error)
        {
            var entity = new ErrorEntity(error);
            var table = GetTable(TableName);
            TableOperation upsertOperation = TableOperation.InsertOrReplace(entity);
            table.Execute(upsertOperation);
            return entity.RowKey;
        }

        public TableErrorLog(IDictionary config)
        {
            Initialize();
        }

        public TableErrorLog(string connectionString)
        {
            this.connectionString = connectionString;
            Initialize();
        }

        void Initialize()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
               CloudConfigurationManager.GetSetting("StorageConnectionString"));

            var tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("Errors");
            table.CreateIfNotExists();
        }
    }
    public class ErrorEntity : TableEntity
    {
        public string SerializedError { get; set; }

        public ErrorEntity() { }
        public ErrorEntity(Error error)
            : base()
        {
            PartitionKey = TableErrorLog.TableName;
            RowKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19");
            this.SerializedError = ErrorXml.EncodeString(error);
        }
    }
}