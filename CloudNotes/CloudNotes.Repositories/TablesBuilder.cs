using System.Configuration;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace CloudNotes.Repositories
{
    /// <summary>
    /// Class to initialize all the Azure Tables.
    /// </summary>
    public static class TablesBuilder
    {
        public static void InitializeTables()
        {
            var tableStorage = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            var tableClient = tableStorage.CreateCloudTableClient();
            tableClient.CreateTableIfNotExist("Users"); // Table to store the Users information.
            tableClient.CreateTableIfNotExist("Notes"); // Table to store all the Notes information.
            tableClient.CreateTableIfNotExist("TaskLists"); // Table to store all the TaskLists information.
            tableClient.CreateTableIfNotExist("TaskListShares"); // Table to store all the Shares related to each TaskList.
            tableClient.CreateTableIfNotExist("NoteShares");    // Table to store all the Shares related to each Note.
            tableClient.CreateTableIfNotExist("TaskListNotes"); // Table to store the information about each TaskList notes.
            tableClient.CreateTableIfNotExist("OAuthRegisteredApps");   // Table to store the OAuth registration applications.
            tableClient.CreateTableIfNotExist("OAuthTokens");   // Table to store the information about the OAuth tokens.
        }
    }
}