namespace CloudNotes.Repositories.Entities.Relation
{
    internal class NoteSubscriberTableEntry : BaseEntity
    {
        #region Constructors

        public NoteSubscriberTableEntry(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
            
        }

        #endregion Constructors
    }
}