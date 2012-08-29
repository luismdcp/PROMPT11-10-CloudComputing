using System.Collections.Generic;
using System.Linq;
using CloudNotes.Domain.Entities;
using CloudNotes.Repositories.Entities;
using CloudNotes.Repositories.Entities.Relation;

namespace CloudNotes.Repositories.Extensions
{
    public static class Mapping
    {
        public static Note MapToNote(this NoteEntity entry)
        {
            Note note = null;

            if (entry != null)
            {
                note = new Note
                {
                    PartitionKey = entry.PartitionKey,
                    RowKey = entry.RowKey,
                    Timestamp = entry.Timestamp,
                    Title = entry.Title,
                    Content = entry.Content,
                    IsClosed = entry.IsClosed,
                    OrderingIndex = entry.OrderingIndex,
                    ContainerKeys = entry.ContainerKeys
                };   
            }

            return note;
        }

        public static NoteEntity MapToNoteEntity(this Note note)
        {
            NoteEntity noteEntity = null;

            if (note != null)
            {
                noteEntity = new NoteEntity(note.PartitionKey, note.RowKey)
                                                {
                                                    Title = note.Title,
                                                    Content = note.Content,
                                                    IsClosed = note.IsClosed,
                                                    OrderingIndex = note.OrderingIndex,
                                                    ContainerKeys = note.ContainerKeys
                                                };
            }

            return noteEntity;
        }

        public static ICollection<NoteShareEntity> MapToNoteShares(this ICollection<User> share, Note note)
        {
            return share.Select(user => new NoteShareEntity(note.RowKey, user.RowKey)).ToList();
        }

        public static TaskList MapToTaskList(this TaskListEntity entry)
        {
            return entry != null ? new TaskList { PartitionKey = entry.PartitionKey, RowKey = entry.RowKey, Timestamp = entry.Timestamp, Title = entry.Title } : null;
        }

        public static TaskListEntity MapToTaskListEntity(this TaskList taskList)
        {
            return taskList != null ? new TaskListEntity(taskList.PartitionKey, taskList.RowKey) { Title = taskList.Title } : null;
        }

        public static User MapToUser(this UserEntity entry)
        {
            return entry != null ? new User(entry.RowKey, entry.Name, entry.Email) { PartitionKey = entry.PartitionKey, RowKey = entry.RowKey, UniqueIdentifier = entry.UniqueIdentifier } : null;
        }

        public static UserEntity MapToUserEntity(this User user)
        {
            return user != null ? new UserEntity(user.PartitionKey, user.RowKey) { Email = user.Email, Name = user.Name, UniqueIdentifier = user.UniqueIdentifier } : null;
        }
    }
}