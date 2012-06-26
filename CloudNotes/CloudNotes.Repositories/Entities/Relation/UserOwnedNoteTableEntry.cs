namespace CloudNotes.Repositories.Entities.Relation
{
    internal class UserOwnedNoteTableEntry : BaseEntity
    {
        #region Constructors

        public UserOwnedNoteTableEntry(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
            
        }

        #endregion Constructors
    }
}