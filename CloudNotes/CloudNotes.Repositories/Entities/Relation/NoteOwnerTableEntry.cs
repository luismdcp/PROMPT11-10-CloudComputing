namespace CloudNotes.Repositories.Entities.Relation
{
    internal class NoteOwnerTableEntry : BaseEntity
    {
        #region Constructors

        public NoteOwnerTableEntry(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
            
        }

        #endregion Constructors
    }
}