using System.Collections.Generic;
using System.IO;
using CloudNotes.Domain.Services.Contracts;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Repositories.Models;

namespace CloudNotes.Domain.Services.Implementation
{
    /// <summary>
    /// Service to manage all the operations related to the Note's files.
    /// </summary>
    public class FilesService : IFilesService
    {
        #region Fields

        private readonly IFilesRepository _filesRepository;

        #endregion Fields

        #region Constructors

        public FilesService(IFilesRepository filesRepository)
        {
            _filesRepository = filesRepository;
        }

        #endregion Constructors

        #region Public methods

        /// <summary>
        /// Check if the filename is valid.
        /// </summary>
        /// <param name="fileName">Filename</param>
        /// <returns>True if the filename does not have the characters - or +</returns>
        public bool IsValidName(string fileName)
        {
            return !(fileName.Contains("-") || fileName.Contains("+"));
        }

        /// <summary>
        /// Get all the data abou the files attached to a Note.
        /// </summary>
        /// <param name="noteOwnerId">The Note owner row key</param>
        /// <param name="noteId">The Note row key</param>
        /// <returns></returns>
        public IEnumerable<FileInfoItem> Get(string noteOwnerId, string noteId)
        {
            return _filesRepository.Get(noteOwnerId, noteId);
        }

        /// <summary>
        /// Upload and attach a File to a Note.
        /// </summary>
        /// <param name="noteOwnerId">The Note owner row key</param>
        /// <param name="noteId">The Note row key</param>
        /// <param name="fileName">The file name</param>
        /// <param name="fileContent">Stream with the file content bytes</param>
        /// <param name="contentType">The file ContentType</param>
        public void Create(string noteOwnerId, string noteId, string fileName, Stream fileContent, string contentType)
        {
            _filesRepository.Create(noteOwnerId, noteId, fileName, fileContent, contentType);
        }

        /// <summary>
        /// Delete a file attached to a Note.
        /// </summary>
        /// <param name="noteOwnerId">The Note owner row key</param>
        /// <param name="noteId">The Note row key</param>
        /// <param name="fileName">The file name</param>
        public void Delete(string noteOwnerId, string noteId, string fileName)
        {
            _filesRepository.Delete(noteOwnerId, noteId, fileName);
        }

        /// <summary>
        /// Download the file attached to a Note.
        /// </summary>
        /// <param name="noteOwnerId">The Note owner row key</param>
        /// <param name="noteId">The Note row key</param>
        /// <param name="fileName">The file name</param>
        /// <param name="responseStream">Stream to write the file name content bytes</param>
        public void Download(string noteOwnerId, string noteId, string fileName, Stream responseStream)
        {
            _filesRepository.Download(noteOwnerId, noteId, fileName, responseStream);
        }

        #endregion Public methods
    }
}