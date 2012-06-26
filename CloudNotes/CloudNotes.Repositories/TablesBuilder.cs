using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace CloudNotes.Repositories
{
    public static class TablesBuilder
    {
        public const string StorageAccountConnectionString = "DefaultEndpointsProtocol=http;AccountName=luis.prompt;AccountKey=EGzUtKSo0RvBnMX4gqde8TQx0LErE9PQ5KFbo++WeURl8zPidAtt3tzQnzCSp7IH1rW8Ecct3OmNdB3gLwhA1Q==";

        public static void InitializeTables()
        {
            var tableStorage = CloudStorageAccount.Parse(StorageAccountConnectionString);
            var tableClient = tableStorage.CreateCloudTableClient();
            tableClient.CreateTableIfNotExist("Users");
            tableClient.CreateTableIfNotExist("Notes");
            tableClient.CreateTableIfNotExist("TaskLists");
            tableClient.CreateTableIfNotExist("NoteOwner");
            tableClient.CreateTableIfNotExist("NoteAssociatedUsers");
            tableClient.CreateTableIfNotExist("TaskListAssociatedUsers");
        }
    }
}