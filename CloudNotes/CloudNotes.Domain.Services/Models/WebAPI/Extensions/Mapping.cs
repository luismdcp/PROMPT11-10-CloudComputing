using System.Collections.Generic;
using System.Linq;
using CloudNotes.Domain.Entities;

namespace CloudNotes.Domain.Services.Models.WebAPI.Extensions
{
    public static class Mapping
    {
        public static List MapToList(this TaskList taskList)
        {
            return taskList != null ? new List(taskList.PartitionKey, taskList.RowKey, taskList.Title) : null;
        }

        public static TaskList MapToTaskList(this List list)
        {
            return list != null ? new TaskList(list.PartitionKey, list.RowKey, list.Title) : null;
        }

        public static IEnumerable<List> MapToLists(this IEnumerable<TaskList> taskLists)
        {
            return taskLists.Select(t => new List(t.PartitionKey, t.RowKey, t.Title));
        }

        public static Task MapToTask(this Note note)
        {
            return note != null ? new Task(note.PartitionKey, note.RowKey, note.Title, note.Content, note.IsClosed) : null;
        }

        public static Note MapToNote(this Task task)
        {
            return task != null ? new Note(task.PartitionKey, task.RowKey, task.Title, task.Content, task.IsClosed) : null;
        }

        public static IEnumerable<Task> MapToTasks(this IEnumerable<Note> notes)
        {
            return notes.Select(t => new Task(t.PartitionKey, t.RowKey, t.Title, t.Content, t.IsClosed));
        }
    }
}