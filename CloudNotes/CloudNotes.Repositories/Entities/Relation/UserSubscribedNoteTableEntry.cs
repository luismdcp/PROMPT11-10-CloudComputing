namespace CloudNotes.Repositories.Entities.Relation
{
    internal class UserSubscribedNoteTableEntry : BaseEntity
    {
        #region Constructors

        public UserSubscribedNoteTableEntry(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
            
        }

        #endregion Constructors
    }
}