using System.Configuration;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace CloudNotes.Repositories
{
    public static class TablesBuilder
    {
        public static void InitializeTables()
        {
            var tableStorage = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            var tableClient = tableStorage.CreateCloudTableClient();
            tableClient.CreateTableIfNotExist("Users");
            tableClient.CreateTableIfNotExist("Notes");
            tableClient.CreateTableIfNotExist("TaskLists");
            tableClient.CreateTableIfNotExist("TaskListShares");

            tableClient.CreateTableIfNotExist("NoteShares");
            tableClient.CreateTableIfNotExist("TaskListNotes");
        }
    }
}