namespace CloudNotes.Repositories.Entities
{
    public class NoteTableEntry : BaseEntity
    {
        #region Properties

        public string Content { get; set; }
        public bool IsClosed { get; set; }
        public int OrderingIndex { get; set; }

        #endregion Properties

        #region Constructors

        public NoteTableEntry(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
            
        }

        #endregion Constructors
    }
}