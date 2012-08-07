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

        #endregion Fields

        #region Constructors

        public NotesController(ITaskListsService taskListsService, INotesService notesService)
        {
            _taskListsService = taskListsService;
            _notesService = notesService;
        }

        #endregion Constructors

        #region Actions

        // GET: /TaskList/TaskList-Title/Notes/Index
        public ActionResult Index(string taskListTitle, string sortOrder)
        {
            ViewBag.TitleSortParam = (sortOrder == "Title") ? "Title desc" : "Title";
            ViewBag.CreatedSortParam = (sortOrder == "Timestamp") ? "Timestamp desc" : "Timestamp";
            ViewBag.CurrentSortOrder = sortOrder;

            var currentUser = (User) Session["CurrentUser"];
            var taskList = _taskListsService.GetByTitleAndOwner(taskListTitle.Replace('-', ' '), currentUser);
            ViewBag.TaskListTitle = taskList.Title.Replace(' ', '-');

            _notesService.LoadNotes(taskList);
            var notes = taskList.Notes.AsQueryable();

            if (!String.IsNullOrEmpty(sortOrder))
            {
                notes = notes.OrderBy(sortOrder);
            }

            return View(notes.AsEnumerable());
        }

        [HttpPost]
        public ActionResult SaveNotesOrder(string[] orderingIndexes, string[] partitionKeys, string[] rowKeys, string[] titles, string taskListTitle, string sortOrder)
        {
            for (int i = 0; i < orderingIndexes.Length; i++)
            {
                var parsedIndex = int.Parse(orderingIndexes[i]);

                if (parsedIndex != i)
                {
                    var noteToUpdate = _notesService.Get(partitionKeys[i], rowKeys[i]);
                    noteToUpdate.OrderingIndex = i;
                    _notesService.Update(noteToUpdate);
                }
            }

            return RedirectToAction("Index", new { taskListTitle, sortOrder });
        }

        // GET: /TaskList/TaskList-Title/Notes/Create
        public ActionResult Create(string taskListTitle)
        {
            ViewBag.TaskListTitle = taskListTitle.Replace(' ', '-');
            return View(new Note());
        }

        // POST: /TaskList/TaskList-Title/Notes/Create
        [HttpPost, ActionName("Create")]
        public ActionResult CreatePost(string taskListTitle)
        {
            var currentUser = (User) Session["CurrentUser"];
            var taskList = _taskListsService.GetByTitleAndOwner(taskListTitle.Replace('-', ' '), currentUser);
            var note = new Note { Owner = currentUser, ContainerList = taskList };

            try
            {
                TryUpdateModel(note);

                // Check if a Note with the same title already exists.
                var existingNote = _notesService.GetByTitle(note.Title, taskList);

                if (existingNote != null)
                {
                    ViewBag.ValidationErrors = new List<ValidationResult> { new ValidationResult(string.Format("The Note with the title '{0}' already exists.", note.Title)) };
                    return View(note);
                }

                if (note.IsValid())
                {
                    _notesService.Create(note);
                    return RedirectToAction("Index", "Notes", new { taskListTitle });
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

        // GET: /TaskList/TaskList-Title/Notes/Details/Note-Title
        public ActionResult Details(string taskListTitle, string noteTitle)
        {
            var currentUser = (User) Session["CurrentUser"];
            var note = _notesService.GetNoteEagerLoaded(taskListTitle, noteTitle, currentUser);
            return View(note);
        }

        // GET: /TaskList/TaskList-Title/Notes/Delete/Note-Title
        public ActionResult Delete(string taskListTitle, string noteTitle)
        {
            var currentUser = (User) Session["CurrentUser"];
            var note = _notesService.GetNoteEagerLoaded(taskListTitle, noteTitle, currentUser);
            ViewBag.TaskListTitle = taskListTitle;
            return note == null ? View("Info", "_Layout", string.Format("The note with the title '{0}' doesn’t exist or was deleted.", noteTitle)) : View(note);
        }

        // POST: /TaskList/TaskList-Title/Notes/Delete/Note-Title
        [HttpPost, ActionName("Delete")]
        public ActionResult DeletePost(string taskListTitle, string noteTitle)
        {
            var currentUser = (User) Session["CurrentUser"];
            var note = _notesService.GetNoteEagerLoaded(taskListTitle, noteTitle, currentUser);
            _notesService.Delete(note);
            return RedirectToAction("Index", new { taskListTitle });
        }

        // GET: /TaskList/TaskList-Title/Notes/Edit/Note-Title
        public ActionResult Edit(string taskListTitle, string noteTitle)
        {
            var currentUser = (User) Session["CurrentUser"];
            var note = _notesService.GetNoteEagerLoaded(taskListTitle, noteTitle, currentUser);
            ViewBag.TaskListTitle = taskListTitle;
            return View(note);
        }

        // POST: /TaskList/TaskList-Title/Notes/Edit/Note-Title
        [HttpPost, ActionName("Edit")]
        public ActionResult EditPost(string taskListTitle, string noteTitle)
        {
            var currentUser = (User) Session["CurrentUser"];
            var note = _notesService.GetNoteEagerLoaded(taskListTitle, noteTitle, currentUser);

            try
            {
                TryUpdateModel(note);

                if (note.IsValid())
                {
                    _notesService.Update(note);
                    return RedirectToAction("Index");
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

        // GET: /TaskList/TaskList-1/Notes/Copy/Note-1
        public ActionResult Copy(string taskListTitle, string noteTitle)
        {
            var currentUser = (User) Session["CurrentUser"];
            var taskListsOwnedByUser = _taskListsService.GetTaskListsOwnedByUser(currentUser);
            ViewBag.NoteToCopy = noteTitle;
            return View(taskListsOwnedByUser);
        }

        // GET: /TaskList/TaskList-1/Notes/Move/Note-1
        public ActionResult Move(string taskListTitle, string noteTitle)
        {
            var currentUser = (User) Session["CurrentUser"];
            var taskListsOwnedByUser = _taskListsService.GetTaskListsOwnedByUser(currentUser);
            ViewBag.NoteToMove = noteTitle;
            return View(taskListsOwnedByUser);
        }

        [HttpPost]
        public ActionResult CopyNote(string taskListTitle, string noteTitle, string[] noteCheck)
        {
            if (noteCheck != null)
            {
                var currentUser = (User) Session["CurrentUser"];
                var note = _notesService.GetNoteEagerLoaded(taskListTitle, noteTitle, currentUser);
                var destinationTaskList = _taskListsService.Get(currentUser.RowKey, noteCheck[0]);
                _notesService.CopyNote(note, destinationTaskList);

                return RedirectToAction("Index");
            }
            else
            {
                TempData["AlertMessage"] = "Please select a destination TaskList.";
                return RedirectToAction("Copy", new { taskListTitle, noteTitle });   
            }
        }

        [HttpPost]
        public ActionResult MoveNote(string taskListTitle, string noteTitle, string[] noteCheck)
        {
            if (noteCheck != null)
            {
                var currentUser = (User) Session["CurrentUser"];
                var note = _notesService.GetNoteEagerLoaded(taskListTitle, noteTitle, currentUser);
                var destinationTaskList = _taskListsService.Get(currentUser.RowKey, noteCheck[0]);
                _notesService.MoveNote(note, destinationTaskList);

                return RedirectToAction("Index");
            }
            else
            {
                TempData["AlertMessage"] = "Please select a destination TaskList.";

                return RedirectToAction("Move", new { taskListTitle, noteTitle });
            }
        }

        #endregion Actions  
    }
}