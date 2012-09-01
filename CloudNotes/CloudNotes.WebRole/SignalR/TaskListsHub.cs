using SignalR.Hubs;

namespace CloudNotes.WebRole.SignalR
{
    [HubName("tasklistsHub")]
    public class TaskListsHub : Hub
    {
        #region TaskList methods

        public void CreateTaskList()
        {
            Clients.BroadcastTaskListChanges();
        }

        public void DeleteTaskList(string taskListId)
        {
            Clients.BroadcastTaskListDelete(taskListId);
            Clients.BroadcastTaskListChanges(taskListId);
        }

        public void EditTaskList(string title, string taskListId)
        {
            Clients.BroadcastTaskListChanges(title, taskListId);
        }

        public void ShareTaskList(string taskListId)
        {
            Clients.BroadcastTaskListShare(taskListId);
        }

        #endregion TaskList methods

        #region Note methods

        public void CreateNote()
        {
            Clients.BroadcastNoteChanges();
        }

        public void DeleteNote(string noteId)
        {
            Clients.BroadcastNoteDelete(noteId);
            Clients.BroadcastNoteChanges(noteId);
        }

        public void EditNote(string title, string content, string isClosed, string noteId)
        {
            Clients.BroadcastNoteChanges(title, content, isClosed, noteId);
        }

        public void ShareNote(string noteId)
        {
            Clients.BroadcastNoteShare(noteId);
        }

        public void MoveOrCopyNote()
        {
            Clients.BroadcastNoteChanges();
        }

        public void Notify(string message)
        {
            Clients.Notify(message);
        }

        #endregion Note methods
    }
}