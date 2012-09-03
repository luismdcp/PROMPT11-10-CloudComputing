using System.Configuration;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace CloudNotes.Repositories
{
    public static class ContainerBuilder
    {
        /// <summary>
        /// Class to initialize the Blob Container to store all the uploaded files.
        /// </summary>
        public static void InitializeContainer()
        {
            var blobStorage = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            var blobClient = blobStorage.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("cloudnotesfiles");
            container.CreateIfNotExist();
        }
    }
}