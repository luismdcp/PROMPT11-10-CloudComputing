using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using CloudNotes.Domain.Entities;
using CloudNotes.Domain.Services.Contracts;

namespace CloudNotes.WebRole.Controllers
{
    public class UsersController : Controller
    {
        #region Fields

        private readonly IUsersService _usersService;
        private readonly ITaskListsService _taskListsService;
        private readonly INotesService _notesService;

        #endregion Fields

        #region Constructors

        public UsersController(IUsersService usersService, ITaskListsService taskListsService, INotesService notesService)
        {
            _usersService = usersService;
            _taskListsService = taskListsService;
            _notesService = notesService;
        }

        #endregion Constructors

        #region Actions

        // GET: /Users/Home
        public ActionResult Home(IPrincipal principal)
        {
            string name;
            string email;
            string uniqueIdentifier;
            var userExists = _usersService.UserExists(principal);

            _usersService.GetUserAuthenticationInfo(principal, out name, out email, out uniqueIdentifier);

            if (!userExists)
            {
                ViewBag.UserName = name;
                ViewBag.UserEmail = email;
                return View();
            }
            else
            {
                var user = _usersService.GetByUniqueIdentifier(uniqueIdentifier);
                Session["CurrentUser"] = user;
                return RedirectToAction("Index", "TaskLists");
            }
        }

        // POST: /Users/Home
        [HttpPost, ActionName("Home")]
        public ActionResult HomePost(IPrincipal principal)
        {
            var user = new User();
            
            try
            {
                TryUpdateModel(user);

                if (user.IsValid())
                {
                    _usersService.FillUniqueIdentifier(user, principal);
                    _usersService.Create(user);
                    Session["CurrentUser"] = user;
                    return RedirectToAction("Index", "TaskLists");
                }
                else
                {
                    ViewBag.ValidationErrors = user.GetValidationErrors();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ValidationErrors = new List<ValidationResult> { new ValidationResult(string.Format("User creation exception: {0}", ex.Message)) };
            }

            return View(user);
        }

        // GET: /Users/Associate/TaskList/TaskList-Title
        public ActionResult AssociateUsersToTaskList(string taskListTitle)
        {
            var allUsers = _usersService.Load();
            var currentUser = (User) Session["CurrentUser"];
            var taskList = _taskListsService.GetByTitleAndOwner(taskListTitle.Replace('-', ' '), currentUser);

            _usersService.LoadTaskListAssociatedUsers(taskList);
            ViewBag.TaskListToAssociate = taskList;

            return View(allUsers.Except(taskList.AssociatedUsers));
        }

        // GET: /Users/Associate/Note/Note-Title
        public ActionResult AssociateUsersToNote(string noteTitle, string taskListTitle)
        {
            var allUsers = _usersService.Load();
            var currentUser = (User) Session["CurrentUser"];
            var taskList = _taskListsService.GetByTitleAndOwner(taskListTitle.Replace('-', ' '), currentUser);
            var note = _notesService.GetByTitle(noteTitle, taskList);

            _usersService.LoadNoteAssociatedUsers(note);
            ViewBag.NoteToAssociate = note;

            return View(allUsers.Except(note.AssociatedUsers));
        }

        [HttpPost]
        public ActionResult AssociateUsersToTaskListPost(string taskListTitle, string partitionKey, string rowKey, string[] userCheck)
        {
            if (userCheck != null)
            {
                var taskList = _taskListsService.Get(partitionKey, rowKey);

                foreach (var userRowKey in userCheck)
                {
                    var user = _usersService.Get("Users", userRowKey);
                    _taskListsService.AddAssociatedUser(taskList, user);
                }

                return RedirectToAction("Index", "TaskLists");
            }
            else
            {
                TempData["AlertMessage"] = "Please select at least one User to associate.";
                return RedirectToAction("AssociateUsersToTaskList", new { taskListTitle });
            }
        }

        [HttpPost]
        public ActionResult AssociateUsersToNotePost(string taskListTitle, string noteTitle, string partitionKey, string rowKey, string[] userCheck)
        {
            if (userCheck != null)
            {
                var note = _notesService.Get(partitionKey, rowKey);


                foreach (var userRowKey in userCheck)
                {
                    var user = _usersService.Get("Users", userRowKey);
                    _notesService.AddAssociatedUser(note, user);
                }

                return RedirectToAction("Index", "Notes", new { taskListTitle });
            }
            else
            {
                TempData["AlertMessage"] = "Please select at least one User to associate.";
                return RedirectToAction("AssociateUsersToNote", new { taskListTitle });
            }
        }

        #endregion Actions
    }
}