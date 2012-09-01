using System.Collections.Generic;
using System.Configuration;
using System.IO;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Repositories.Models;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace CloudNotes.Repositories.Implementation
{
    /// <summary>
    /// Repository to manage all the actions related to files uploads and deletes to the application Blobs Container.
    /// </summary>
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

        /// <summary>
        /// Gets all the information about the files upload to a virtual directories structure in the Blobs Container.
        /// </summary>
        /// <param name="noteOwnerId">RowKey from the User that created the Note where the files were uploaded</param>
        /// <param name="noteId">RowKey from the Note where the files were uploaded</param>
        /// <returns></returns>
        public IEnumerable<FileInfoItem> Get(string noteOwnerId, string noteId)
        {
            var infoList = new List<FileInfoItem>();

            // Get a reference to the virtual directories structure in the blobs container.
            // The virtual directories structures consists in a Directory with the name taken from RowKey from the User 
            // and a Sub-Directory with the name taken from the RowKey of the Note where the files were uploaded.
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

        /// <summary>
        /// Uploads a file to the Blobs Container virtual directory structure.
        /// </summary>
        /// <param name="noteOwnerId">RowKey from the User that created the Note where the files were uploaded</param>
        /// <param name="noteId">RowKey from the Note where the files were uploaded</param>
        /// <param name="fileName">The file name</param>
        /// <param name="fileContent">Stream with the file content</param>
        /// <param name="contentType">The file ContentType</param>
        public void Create(string noteOwnerId, string noteId, string fileName, Stream fileContent, string contentType)
        {
            // The virtual directories structures consists in a Directory with the name taken from RowKey from the User 
            // and a Sub-Directory with the name taken from the RowKey of the Note where the files were uploaded.
            var blobName = string.Format("{0}/{1}/{2}", noteOwnerId, noteId, fileName);
            var blob = _container.GetBlobReference(blobName);
            blob.UploadFromStream(fileContent);
            blob.Properties.ContentType = contentType;
            blob.SetProperties();
        }

        /// <summary>
        /// Deletes a file in the Blobs Container virtual directory structure.
        /// </summary>
        /// <param name="noteOwnerId">RowKey from the User that created the Note where the files were uploaded</param>
        /// <param name="noteId">RowKey from the Note where the files were uploaded</param>
        /// <param name="fileName">The file name</param>
        public void Delete(string noteOwnerId, string noteId, string fileName)
        {
            var blobName = string.Format("{0}/{1}/{2}", noteOwnerId, noteId, fileName);
            var blob = _container.GetBlobReference(blobName);
            blob.DeleteIfExists();
        }

        /// <summary>
        /// Downloads a file from the Blobs Container to a Stream.
        /// </summary>
        /// <param name="noteOwnerId">RowKey from the User that created the Note where the files were uploaded</param>
        /// <param name="noteId">RowKey from the Note where the files were uploaded</param>
        /// <param name="fileName">The file name</param>
        /// <param name="responseStream">The Steam to write the file content bytes</param>
        public void Download(string noteOwnerId, string noteId, string fileName, Stream responseStream)
        {
            var blobName = string.Format("{0}/{1}/{2}", noteOwnerId, noteId, fileName);
            var blob = _container.GetBlobReference(blobName);
            blob.DownloadToStream(responseStream);
        }

        #endregion Public methods
    }
}