namespace CloudNotes.Repositories.Entities.Relation
{
    internal class UserAssociatedNoteTableEntry : BaseEntity
    {
        #region Constructors

        public UserAssociatedNoteTableEntry(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
            
        }

        #endregion Constructors
    }
}