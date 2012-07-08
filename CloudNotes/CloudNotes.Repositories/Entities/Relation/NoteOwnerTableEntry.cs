namespace CloudNotes.Repositories.Entities.Relation
{
    public class NoteOwnerTableEntry : BaseEntity
    {
        #region Constructors

        public NoteOwnerTableEntry(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
            
        }

        #endregion Constructors
    }
}