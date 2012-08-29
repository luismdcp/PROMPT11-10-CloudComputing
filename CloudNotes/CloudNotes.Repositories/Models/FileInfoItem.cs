using System;

namespace CloudNotes.Repositories.Models
{
    public class FileInfoItem
    {
        public Uri FileUri { get; set; }
        public long Size { get; set; }
        public string ContentType { get; set; }
        public string NoteOwnerId { get; set; }
        public string NoteId { get; set; }
    }
}