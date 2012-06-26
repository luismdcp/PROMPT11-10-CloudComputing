namespace CloudNotes.Repositories.Contracts
{
    public interface IUnitOfWork
    {
        void SubmitChanges();
        void SubmitChangesWithInsertOrReplace();
    }
}