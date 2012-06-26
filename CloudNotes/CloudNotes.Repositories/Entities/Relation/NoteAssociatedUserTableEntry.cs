namespace CloudNotes.Repositories.Entities.Relation
{
    internal class NoteAssociatedUserTableEntry : BaseEntity
    {
        #region Constructors

        public NoteAssociatedUserTableEntry (string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
            
        }

        #endregion Constructors
    }
}