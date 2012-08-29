using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using CloudNotes.Domain.Entities;
using CloudNotes.Domain.Services.Contracts;
using CloudNotes.WebRole.Helpers;

namespace CloudNotes.WebRole.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        #region Fields

        private readonly ITaskListsService _taskListsService;
        private readonly INotesService _notesService;
        private readonly IUsersService _usersService;

        #endregion Fields

        #region Constructors

        public NotesController(ITaskListsService taskListsService, INotesService notesService, IUsersService usersService)
        {
            _taskListsService = taskListsService;
            _notesService = notesService;
            _usersService = usersService;
        }

        #endregion Constructors

        #region Actions

        // GET: /Notes/TaskList/{taskListOwnerId}/{taskListId}/Index
        public ActionResult Index(string taskListOwnerId, string taskListId, string sortOrder)
        {
            #region Sorting

            ViewBag.TitleSortParam = (sortOrder == "Title") ? "Title desc" : "Title";
            ViewBag.CreatedSortParam = (sortOrder == "Timestamp") ? "Timestamp desc" : "Timestamp";
            ViewBag.CurrentSortOrder = sortOrder;

            #endregion Sorting

            var currentUser = (User) Session["CurrentUser"];
            var container = _taskListsService.Get(t => t.PartitionKey == taskListOwnerId && t.RowKey == taskListId);
            _notesService.LoadNotes(container);

            // Only a user in the note's containing tasklist share list can list all the notes.
            if (!_taskListsService.HasPermissionToEdit(currentUser, container))
            {
                return View("Info", "_Layout", string.Format("The user '{0}' doesn't have permission to list the notes in the tasklist '{1}'.", currentUser.Name, container.Title));
            }

            ViewBag.ContainerOwnerId = taskListOwnerId;
            ViewBag.ContainerId = taskListId;
            var notes = container.Notes.AsQueryable();

            if (!String.IsNullOrEmpty(sortOrder))
            {
                notes = notes.OrderBy(sortOrder);
            }

            return View(notes.AsEnumerable());
        }

        [HttpPost]
        public ActionResult SaveNotesOrder(string[] orderingIndexes, string[] partitionKeys, string[] rowKeys, string taskListOwnerId, string taskListId, string sortOrder)
        {
            // if the TaskList has any Notes.
            if (orderingIndexes != null)
            {
                for (int i = 0; i < orderingIndexes.Length; i++)
                {
                    var parsedIndex = int.Parse(orderingIndexes[i]);

                    // If a note has a diferent position in the table then its ordering index it was moved.
                    if (parsedIndex != i)
                    {
                        var noteToUpdate = _notesService.Get(partitionKeys[i], rowKeys[i]);
                        noteToUpdate.OrderingIndex = i;
                        _notesService.Update(noteToUpdate);
                    }
                }
            }

            return RedirectToAction("Index", new { taskListOwnerId, taskListId });
        }

        // GET: /Notes/TaskList/{taskListOwnerId}/{taskListId}/Create
        public ActionResult Create(string taskListOwnerId, string taskListId)
        {
            ViewBag.ContainerOwnerId = taskListOwnerId;
            ViewBag.ContainerId = taskListId;
            return View(new Note());
        }

        // POST: /Notes/TaskList/{taskListOwnerId}/{taskListId}/Create
        [HttpPost, ActionName("Create")]
        public ActionResult CreatePost(string taskListOwnerId, string taskListId)
        {
            var currentUser = (User) Session["CurrentUser"];
            var container = _taskListsService.Get(t => t.PartitionKey == taskListOwnerId && t.RowKey == taskListId);
            var note = new Note { Owner = currentUser, Container = container };

            // Only a user in the note's containing tasklist share list can create a new note.
            if (!_taskListsService.HasPermissionToEdit(currentUser, container))
            {
                return View("Info", "_Layout", string.Format("The user '{0}' doesn't have permission to create a note in the tasklist '{1}'.", currentUser.Name, container.Title));
            }

            try
            {
                TryUpdateModel(note);

                // Check if a note with the same title already exists.
                var noteWithSameTitleExists = _notesService.NoteWithTitleExists(note.Title, container);

                if (noteWithSameTitleExists)
                {
                    ViewBag.ValidationErrors = new List<ValidationResult> { new ValidationResult(string.Format("The note with the title '{0}' already exists.", note.Title)) };
                    return View(note);
                }

                if (note.IsValid())
                {
                    _notesService.Create(note);
                    return RedirectToAction("Index", new { taskListOwnerId = container.PartitionKey, taskListId = container.RowKey });
                }
                else
                {
                    ViewBag.ValidationErrors = note.GetValidationErrors();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ValidationErrors = new List<ValidationResult> { new ValidationResult(string.Format("Create Note exception: {0}", ex.Message)) };
            }

            return View(note);
        }

        // GET: /Notes/{noteOwnerId}/{noteId}/Details
        public ActionResult Details(string noteOwnerId, string noteId)
        {
            var note = _notesService.Get(t => t.PartitionKey == noteOwnerId && t.RowKey == noteId);
            _usersService.LoadShare(note);
            ViewBag.ContainerOwnerId = note.Container.PartitionKey;
            ViewBag.ContainerId = note.Container.RowKey;

            return note == null ? View("Info", "_Layout", string.Format("The note with the title '{0}' doesn’t exist or was deleted.", note.Title)) : View(note);
        }

        // GET: /Notes/{noteOwnerId}/{noteId}/Delete
        public ActionResult Delete(string noteOwnerId, string noteId)
        {
            var note = _notesService.Get(t => t.PartitionKey == noteOwnerId && t.RowKey == noteId);
            ViewBag.ContainerOwnerId = note.Container.PartitionKey;
            ViewBag.ContainerId = note.Container.RowKey;

            return note == null ? View("Info", "_Layout", string.Format("The note with the title '{0}' doesn’t exist or was deleted.", note.Title)) : View(note);
        }

        // POST: /Notes/{noteOwnerId}/{noteId}/Delete
        [HttpPost, ActionName("Delete")]
        public ActionResult DeletePost(string noteOwnerId, string noteId)
        {
            var currentUser = (User) Session["CurrentUser"];
            var note = _notesService.Get(t => t.PartitionKey == noteOwnerId && t.RowKey == noteId);
            ViewBag.ContainerOwnerId = note.Container.PartitionKey;
            ViewBag.ContainerId = note.Container.RowKey;

            // Only a user in the note share list can delete it.
            if (!_notesService.HasPermissionToEdit(currentUser, note))
            {
                return View("Info", "_Layout", string.Format("The user '{0}' doesn't have permission to delete the note '{1}'.", currentUser.Name, note.Title));
            }

            _notesService.Delete(note);

            return RedirectToAction("Index", new { taskListOwnerId = note.Container.PartitionKey, taskListId = note.Container.RowKey });
        }

        // GET: /Notes/{noteOwnerId}/{noteId}/Edit
        public ActionResult Edit(string noteOwnerId, string noteId)
        {
            var note = _notesService.Get(t => t.PartitionKey == noteOwnerId && t.RowKey == noteId);
            ViewBag.ContainerOwnerId = note.Container.PartitionKey;
            ViewBag.ContainerId = note.Container.RowKey;

            return View(note);
        }

        // POST: /Notes/{noteOwnerId}/{noteId}/Edit
        [HttpPost, ActionName("Edit")]
        public ActionResult EditPost(string noteOwnerId, string noteId)
        {
            var currentUser = (User) Session["CurrentUser"];
            var note = _notesService.Get(t => t.PartitionKey == noteOwnerId && t.RowKey == noteId);
            _usersService.LoadOwner(note);

            // Only a user in the note share list can edit it.
            if (!_notesService.HasPermissionToEdit(currentUser, note))
            {
                return View("Info", "_Layout", string.Format("The user '{0}' doesn't have permission to edit the note '{1}'.", currentUser.Name, note.Title));
            }

            try
            {
                TryUpdateModel(note);

                if (note.IsValid())
                {
                    _notesService.Update(note);
                    return RedirectToAction("Index", new { taskListOwnerId = note.Container.PartitionKey, taskListId = note.Container.RowKey });
                }
                else
                {
                    ViewBag.ValidationErrors = note.GetValidationErrors();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ValidationErrors = new List<ValidationResult> { new ValidationResult(string.Format("Note editing exception: {0}", ex.Message)) };
            }

            return View(note);
        }
        // GET: /Notes/{noteOwnerId}/{noteId}/Share
        public ActionResult Share(string noteOwnerId, string noteId)
        {
            var allUsers = _usersService.Load();
            var note = _notesService.Get(t => t.PartitionKey == noteOwnerId && t.RowKey == noteId);
            _usersService.LoadShare(note);
            ViewBag.ContainerOwnerId = note.Container.PartitionKey;
            ViewBag.ContainerId = note.Container.RowKey;
            ViewBag.NoteOwnerId = noteOwnerId;
            ViewBag.NoteId = noteId;

            return View(allUsers.Except(note.Share));
        }

        // POST: /Notes/{noteOwnerId}/{noteId}/Share
        [HttpPost]
        public ActionResult Share(string noteOwnerId, string noteId, string[] userCheck)
        {
            if (userCheck != null)
            {
                var currentUser = (User) Session["CurrentUser"];
                var note = _notesService.Get(t => t.PartitionKey == noteOwnerId && t.RowKey == noteId);

                // Only a user in the note share list can share it.
                if (!_notesService.HasPermissionToEdit(currentUser, note))
                {
                    return View("Info", "_Layout", string.Format("The user '{0}' doesn't have permission to share the note '{1}'.", currentUser.Name, note.Title));
                }

                foreach (var userRowKey in userCheck)
                {
                    _notesService.AddShare(note, userRowKey);
                }

                return RedirectToAction("Index", "Notes", new { ownerId = note.Container.PartitionKey, entityId = note.Container.RowKey });
            }
            else
            {
                TempData["AlertMessage"] = "Please select at least one user to share.";
                return RedirectToAction("Share", new { noteOwnerId, noteId });
            }
        }

        // GET: /Notes/{noteOwnerId}/{noteId}/Copy
        public ActionResult Copy(string noteOwnerId, string noteId)
        {
            var currentUser = (User) Session["CurrentUser"];
            var note = _notesService.Get(t => t.PartitionKey == noteOwnerId && t.RowKey == noteId);
            var sharedTaskLists = _taskListsService.GetShared(currentUser);
            ViewBag.ContainerOwnerId = note.Container.PartitionKey;
            ViewBag.ContainerId = note.Container.RowKey;
            ViewBag.NoteOwnerId = noteOwnerId;
            ViewBag.NoteId = noteId;

            return View(sharedTaskLists.Where(t => t.RowKey != note.Container.RowKey));
        }

        // POST: /Notes/{noteOwnerId}/{noteId}/Copy
        [HttpPost]
        public ActionResult CopyNote(string noteOwnerId, string noteId, string[] taskListCheck)
        {
            // If a tasklist was selected from the list.
            if (taskListCheck != null)
            {
                var currentUser = (User) Session["CurrentUser"];
                var note = _notesService.Get(t => t.PartitionKey == noteOwnerId && t.RowKey == noteId);
                _usersService.LoadOwner(note);
                _usersService.LoadShare(note);
                _taskListsService.LoadContainer(note);

                // Only a user in the note share can copy it.
                if (!_notesService.HasPermissionToEdit(currentUser, note))
                {
                    return View("Info", "_Layout", string.Format("The user '{0}' doesn't have permission to copy the note '{1}'.", currentUser.Name, note.Title));
                }

                var destinationTaskList = _taskListsService.Get(taskListCheck[0]);

                // Check if a note with the same title already exists at the destination tasklist.
                var noteWithSameTitleExists = _notesService.NoteWithTitleExists(note.Title, destinationTaskList);

                if (noteWithSameTitleExists)
                {
                    return View("Info", "_Layout", string.Format("The note with the title '{0}' already exists in the destination tasklist '{1}'.", note.Title, destinationTaskList.Title));
                }

                _notesService.CopyNote(note, destinationTaskList);

                return RedirectToAction("Index", new { taskListOwnerId = note.Container.PartitionKey, taskListId = note.Container.RowKey });
            }
            else
            {
                TempData["AlertMessage"] = "Please select a destination taskList.";
                return RedirectToAction("Copy", new { noteOwnerId, noteId });
            }
        }

        // GET: /Notes/{noteOwnerId}/{noteId}/Move
        public ActionResult Move(string noteOwnerId, string noteId)
        {
            var currentUser = (User) Session["CurrentUser"];
            var note = _notesService.Get(t => t.PartitionKey == noteOwnerId && t.RowKey == noteId);
            var sharedTaskLists = _taskListsService.GetShared(currentUser);
            ViewBag.ContainerOwnerId = note.Container.PartitionKey;
            ViewBag.ContainerId = note.Container.RowKey;
            ViewBag.NoteOwnerId = noteOwnerId;
            ViewBag.NoteId = noteId;

            return View(sharedTaskLists.Where(t => t.RowKey != note.Container.RowKey));
        }

        // POST: /Notes/{noteOwnerId}/{noteId}/Move
        [HttpPost]
        public ActionResult MoveNote(string noteOwnerId, string noteId, string[] taskListCheck)
        {
            // If a tasklist was selected from the list.
            if (taskListCheck != null)
            {
                var currentUser = (User) Session["CurrentUser"];
                var note = _notesService.Get(t => t.PartitionKey == noteOwnerId && t.RowKey == noteId);
                _usersService.LoadOwner(note);
                _usersService.LoadShare(note);

                // Only the user that created the note can move it.
                if (currentUser.RowKey != note.Owner.RowKey)
                {
                    return View("Info", "_Layout", string.Format("Only the onwer of the note '{0}' can move it.", note.Title));
                }

                var destinationTaskList = _taskListsService.Get(taskListCheck[0]);
                _notesService.MoveNote(note, destinationTaskList);

                return RedirectToAction("Index", new { taskListOwnerId = note.Container.PartitionKey, taskListId = note.Container.RowKey });
            }
            else
            {
                TempData["AlertMessage"] = "Please select a destination taskList.";
                return RedirectToAction("Move", new { noteOwnerId, noteId });
            }
        }

        #endregion Actions
    }
}