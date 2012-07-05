using System.Collections.Generic;
using System.Linq;
using CloudNotes.Domain.Entities;
using CloudNotes.Repositories.Entities;
using CloudNotes.Repositories.Entities.Relation;

namespace CloudNotes.Repositories.Extensions
{
    internal static class Mapping
    {
        internal static Note MapToNote(this NoteTableEntry entry)
        {
            var note = new Note(entry.PartitionKey, entry.RowKey)
                            {
                                Timestamp = entry.Timestamp,
                                Title = entry.RowKey,
                                Content = entry.Content,
                                IsClosed = entry.IsClosed,
                                OrderingIndex = entry.OrderingIndex
                            };

            return note;
        }

        internal static NoteTableEntry MapToNoteTableEntry(this Note note)
        {
            var noteTableEntry = new NoteTableEntry(note.PartitionKey, note.RowKey)
                                     {
                                         Content = note.Content,
                                         IsClosed = note.IsClosed,
                                         OrderingIndex = note.OrderingIndex
                                     };

            return noteTableEntry;
        }

        internal static ICollection<NoteAssociatedUserTableEntry> MapToNoteAssociatedUserTableEntries(this ICollection<User> associatedUsers, Note note)
        {
            return associatedUsers.Select(user => new NoteAssociatedUserTableEntry(note.RowKey, user.RowKey)).ToList();
        }

        internal static ICollection<NoteSubscriberTableEntry> MapToNoteSubscribersTableEntries(this ICollection<User> subscribers, Note note)
        {
            return subscribers.Select(user => new NoteSubscriberTableEntry(note.RowKey, user.RowKey)).ToList();
        }

        internal static TaskList MapToTaskList(this TaskListTableEntry entry)
        {
            var taskList = new TaskList(entry.PartitionKey, entry.RowKey)
                               {Timestamp = entry.Timestamp, Title = entry.RowKey};

            return taskList;
        }

        internal static TaskListTableEntry MapToTaskListTableEntry(this TaskList taskList)
        {
            var taskListTableEntry = new TaskListTableEntry(taskList.PartitionKey, taskList.RowKey)
                                         {Title = taskList.Title};

            return taskListTableEntry;
        }

        internal static User MapToUser(this UserTableEntry entry)
        {
            var user = new User(entry.PartitionKey, entry.RowKey) {UserUniqueIdentifier = entry.RowKey, Email = entry.Email};
            return user;
        }

        internal static UserTableEntry MapToUserTableEntry(this User user)
        {
            var userTableEntry = new UserTableEntry(user.PartitionKey, user.RowKey) {Email = user.Email};
            return userTableEntry;
        }
    }
}