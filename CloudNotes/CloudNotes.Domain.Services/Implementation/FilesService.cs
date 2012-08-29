using System.Collections.Generic;
using System.IO;
using CloudNotes.Domain.Services.Contracts;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Repositories.Models;
using System.Configuration;

namespace CloudNotes.Domain.Services.Implementation
{
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

        public bool IsValidName(string fileName)
        {
            bool result = false;

            if (fileName.Contains("-") || fileName.Contains("+"))
            {
                result = false;
            }

            return result;
        }

        public IEnumerable<FileInfoItem> Get(string noteOwnerId, string noteId)
        {
            return _filesRepository.Get(noteOwnerId, noteId);
        }

        public void Create(string noteOwnerId, string noteId, string fileName, Stream fileContent, string contentType)
        {
            _filesRepository.Create(noteOwnerId, noteId, fileName, fileContent, contentType);
        }

        public void Delete(string noteOwnerId, string noteId, string fileName)
        {
            _filesRepository.Delete(noteOwnerId, noteId, fileName);
        }

        public void Download(string noteOwnerId, string noteId, string fileName, Stream responseStream)
        {
            _filesRepository.Download(noteOwnerId, noteId, fileName, responseStream);
        }

        #endregion Public methods
    }
}