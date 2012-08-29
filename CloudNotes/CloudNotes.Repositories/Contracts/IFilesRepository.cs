using System.Collections.Generic;
using System.IO;
using CloudNotes.Repositories.Models;

namespace CloudNotes.Repositories.Contracts
{
    public interface IFilesRepository
    {
        IEnumerable<FileInfoItem> Get(string noteOwnerId, string noteId);
        void Create(string noteOwnerId, string noteId, string fileName, Stream fileContent, string contentType);
        void Delete(string noteOwnerId, string noteId, string fileName);
        void Download(string noteOwnerId, string noteId, string fileName, Stream responseStream);
    }
}