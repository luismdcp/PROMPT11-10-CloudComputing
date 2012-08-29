using System.Collections.Generic;
using System.Configuration;
using System.IO;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Repositories.Models;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace CloudNotes.Repositories.Implementation
{
    public class FilesRepository : IFilesRepository
    {
        #region Fields

        private readonly CloudBlobContainer _container;
        private readonly CloudBlobClient _client;

        #endregion Fields

        #region Constructors

        public FilesRepository()
        {
            var blobStorage = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            _client = blobStorage.CreateCloudBlobClient();
            _container = _client.GetContainerReference("cloudnotesfiles");
        }

        #endregion Constructors

        #region Public methods

        public IEnumerable<FileInfoItem> Get(string noteOwnerId, string noteId)
        {
            var infoList = new List<FileInfoItem>();

            var directoryName = string.Format("{0}/{1}", noteOwnerId, noteId);
            var cloudBlobDirectory = _container.GetDirectoryReference(directoryName);

            foreach (var blobItem in cloudBlobDirectory.ListBlobs())
            {
                var blobReference = blobItem.Container.GetBlobReference(blobItem.Uri.ToString());
                blobReference.FetchAttributes();

                var infoItem = new FileInfoItem {
                                                   ContentType = blobReference.Properties.ContentType,
                                                   FileUri = blobItem.Uri,
                                                   NoteId = noteId,
                                                   Size = blobReference.Properties.Length,
                                                   NoteOwnerId = noteOwnerId
                                               };

                infoList.Add(infoItem);
            }

            return infoList;
        }

        public void Create(string noteOwnerId, string noteId, string fileName, Stream fileContent, string contentType)
        {
            var blobName = string.Format("{0}/{1}/{2}", noteOwnerId, noteId, fileName);
            var blob = _container.GetBlobReference(blobName);
            blob.UploadFromStream(fileContent);
            blob.Properties.ContentType = contentType;
            blob.SetProperties();
        }

        public void Delete(string noteOwnerId, string noteId, string fileName)
        {
            var blobName = string.Format("{0}/{1}/{2}", noteOwnerId, noteId, fileName);
            var blob = _container.GetBlobReference(blobName);
            blob.DeleteIfExists();
        }

        public void Download(string noteOwnerId, string noteId, string fileName, Stream responseStream)
        {
            var blobName = string.Format("{0}/{1}/{2}", noteOwnerId, noteId, fileName);
            var blob = _container.GetBlobReference(blobName);
            blob.DownloadToStream(responseStream);
        }

        #endregion Public methods
    }
}