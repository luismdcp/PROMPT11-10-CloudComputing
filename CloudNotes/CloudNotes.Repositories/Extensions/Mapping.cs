using System.Collections.Generic;
using System.Linq;
using CloudNotes.Domain.Entities;
using CloudNotes.Repositories.Entities;
using CloudNotes.Repositories.Entities.Relation;

namespace CloudNotes.Repositories.Extensions
{
    public static class Mapping
    {
        public static Note MapToNote(this NoteTableEntry entry)
        {
            var note = new Note
                           {
                                PartitionKey = entry.PartitionKey,
                                RowKey = entry.RowKey,
                                Timestamp = entry.Timestamp,
                                Title = entry.Title,
                                Content = entry.Content,
                                IsClosed = entry.IsClosed,
                                OrderingIndex = entry.OrderingIndex
                            };

            return note;
        }

        public static NoteTableEntry MapToNoteTableEntry(this Note note)
        {
            var noteTableEntry = new NoteTableEntry(note.PartitionKey, note.RowKey)
            {
                Title = note.Title,
                Content = note.Content,
                IsClosed = note.IsClosed,
                OrderingIndex = note.OrderingIndex
            };

            return noteTableEntry;
        }

        public static ICollection<NoteAssociatedUserTableEntry> MapToNoteAssociatedUserTableEntries(this ICollection<User> associatedUsers, Note note)
        {
            return associatedUsers.Select(user => new NoteAssociatedUserTableEntry(note.RowKey, user.RowKey)).ToList();
        }

        public static TaskList MapToTaskList(this TaskListTableEntry entry)
        {
            return new TaskList { PartitionKey = entry.PartitionKey, RowKey = entry.RowKey, Timestamp = entry.Timestamp, Title = entry.Title };
        }

        public static TaskListTableEntry MapToTaskListTableEntry(this TaskList taskList)
        {
            return new TaskListTableEntry(taskList.PartitionKey, taskList.RowKey) { Title = taskList.Title };
        }

        public static User MapToUser(this UserTableEntry entry)
        {
            return new User(entry.RowKey, entry.Name, entry.Email) { PartitionKey = entry.PartitionKey, RowKey = entry.RowKey};
        }

        public static UserTableEntry MapToUserTableEntry(this User user)
        {
            return new UserTableEntry(user.PartitionKey, user.RowKey) { Email = user.Email, Name = user.Name};
        }
    }
}