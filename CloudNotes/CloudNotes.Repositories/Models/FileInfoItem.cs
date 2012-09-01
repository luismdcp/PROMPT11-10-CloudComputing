using System;

namespace CloudNotes.Repositories.Models
{
    public class FileInfoItem
    {
        public Uri FileUri { get; set; }    // Uri for the upload file in the Azure Blobs Container.
        public long Size { get; set; }      // The uploaded file's bytes size.
        public string ContentType { get; set; } // The file ContentType.
        public string NoteOwnerId { get; set; } // RowKey from the User that created the Note where the file was uploaded.
        public string NoteId { get; set; }  // RowKey from the Note where the file was uploaded.
    }
}