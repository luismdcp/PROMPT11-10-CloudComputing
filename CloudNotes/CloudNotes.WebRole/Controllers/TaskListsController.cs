using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using CloudNotes.Domain.Entities;
using CloudNotes.Domain.Services.Contracts;
using CloudNotes.WebRole.Config;
using CloudNotes.WebRole.Helpers;
using PagedList;

namespace CloudNotes.WebRole.Controllers
{
    [Authorize]
    public class TaskListsController : Controller
    {
        #region Fields

        private readonly ITaskListsService _taskListsService;
        private readonly IUsersService _usersService;

        #endregion Fields

        #region Constructors

        public TaskListsController(ITaskListsService taskListsService, IUsersService usersService)
        {
            _taskListsService = taskListsService;
            _usersService = usersService;
        }

        #endregion Constructors

        #region Actions

        // GET: /TaskLists/Index
        public ActionResult Index(string sortOrder, int page = 1)
        {
            #region Sorting

            ViewBag.TitleSortParam = (sortOrder == "Title") ? "Title desc" : "Title";
            ViewBag.CreatedSortParam = (sortOrder == "Timestamp") ? "Timestamp desc" : "Timestamp";

            if (String.IsNullOrEmpty(sortOrder))
            {
                sortOrder = "Title";
            }

            ViewBag.CurrentSortOrder = sortOrder;

            #endregion Sorting

            var currentUser = (User) Session["CurrentUser"];
            var sharedTaskLists = _taskListsService.GetShared(currentUser).AsQueryable().OrderBy(sortOrder);
            var pageSize = AppConfiguration.PageSize;

            return View(sharedTaskLists.ToPagedList(page, pageSize));
        }

        // GET: /TaskLists/Create
        public ActionResult Create()
        {
            return View(new TaskList());
        }

        // POST: /TaskLists/Create
        [HttpPost, ActionName("Create")]
        public ActionResult CreatePost()
        {
            var currentUser = (User) Session["CurrentUser"];
            var taskList = new TaskList { Owner = currentUser };

            try
            {
                TryUpdateModel(taskList);

                // Check if a tasklist already exists with the same title.
                var existingTaskList = _taskListsService.Get(t => t.PartitionKey == currentUser.RowKey && t.Title == taskList.Title);

                if (existingTaskList != null)
                {
                    ViewBag.ValidationErrors = new List<ValidationResult> { new ValidationResult(string.Format("The taskList with the title '{0}' already exists.", taskList.Title)) };
                    return View(taskList);
                }

                if (taskList.IsValid())
                {
                    _taskListsService.Create(taskList);
                    
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ValidationErrors = taskList.GetValidationErrors();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ValidationErrors = new List<ValidationResult> { new ValidationResult(string.Format("Create taskList exception: {0}", ex.Message)) };
            }

            return View(taskList);
        }

        // GET: /TaskLists/{taskListOwnerId}/{taskListId}/Details
        public ActionResult Details(string taskListOwnerId, string taskListId)
        {
            var taskList = _taskListsService.Get(t => t.PartitionKey == taskListOwnerId && t.RowKey == taskListId);
            _usersService.LoadShare(taskList);
            return taskList == null ? View("Info", "_Layout", string.Format("The taskList with the title '{0}' doesn’t exist or was deleted.", taskList.Title)) : View(taskList);
        }

        // GET: /TaskLists/{taskListOwnerId}/{taskListId}/Edit
        public ActionResult Edit(string taskListOwnerId, string taskListId)
        {
            var taskList = _taskListsService.Get(t => t.PartitionKey == taskListOwnerId && t.RowKey == taskListId);
            return View(taskList);
        }

        // POST: /TaskLists/{taskListOwnerId}/{taskListId}/Edit
        [HttpPost, ActionName("Edit")]
        public ActionResult EditPost(string taskListOwnerId, string taskListId)
        {
            var currentUser = (User) Session["CurrentUser"];
            var taskList = _taskListsService.Get(t => t.PartitionKey == taskListOwnerId && t.RowKey == taskListId);
            _usersService.LoadOwner(taskList);

            // Only a user in the taskList share can edit it.
            if (!_taskListsService.HasPermissionToEdit(currentUser, taskList))
            {
                return View("Info", "_Layout", string.Format("The user '{0}' doesn't have permission to edit the tasklist '{1}'.", currentUser.Name, taskList.Title));
            }

            try
            {
                TryUpdateModel(taskList);

                if (taskList.IsValid())
                {
                    _taskListsService.Update(taskList);
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ValidationErrors = taskList.GetValidationErrors();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ValidationErrors = new List<ValidationResult> { new ValidationResult(string.Format("TaskList editing exception: {0}", ex.Message)) };
            }

            return View(taskList);
        }

        // GET: /TaskLists/{taskListOwnerId}/{taskListId}/Delete
        public ActionResult Delete(string taskListOwnerId, string taskListId)
        {
            var taskList = _taskListsService.Get(t => t.PartitionKey == taskListOwnerId && t.RowKey == taskListId);
            return taskList == null ? View("Info", "_Layout", string.Format("The taskList with the title '{0}' doesn’t exist or was deleted.", taskList.Title)) : View(taskList);
        }

        // POST: /TaskLists/{taskListOwnerId}/{taskListId}/Delete
        [HttpPost, ActionName("Delete")]
        public ActionResult DeletePost(string taskListOwnerId, string taskListId)
        {
            var currentUser = (User) Session["CurrentUser"];
            var taskList = _taskListsService.Get(t => t.PartitionKey == taskListOwnerId && t.RowKey == taskListId);

            // Only a user in the taskList share list can edit it.
            if (!_taskListsService.HasPermissionToEdit(currentUser, taskList))
            {
                return View("Info", "_Layout", string.Format("The user {0} doesn't have permission to edit the tasklist {1}.", currentUser.Name, taskList.Title));
            }

            // Check if the TaskList is being deleted by the user that created it.
            if (taskList.PartitionKey != currentUser.RowKey)
            {
                ViewBag.ValidationErrors = new List<ValidationResult> { new ValidationResult(string.Format("The taskList with the title '{0}' can only be deleted by its owner.", taskList.Title)) };
                return View(taskList);
            }

            _taskListsService.Delete(taskList);
            return RedirectToAction("Index");
        }

        // GET: /TaskLists/{taskListOwnerId}/{taskListId}/Share
        public ActionResult Share(string taskListOwnerId, string taskListId)
        {
            var allUsers = _usersService.Load();
            var taskList = _taskListsService.Get(t => t.PartitionKey == taskListOwnerId && t.RowKey == taskListId);
            _usersService.LoadShare(taskList);

            return View(allUsers.Except(taskList.Share));
        }

        // POST: /TaskLists/{taskListOwnerId}/{taskListId}/Share
        [HttpPost]
        public ActionResult Share(string taskListOwnerId, string taskListId, string[] userCheck)
        {
            if (userCheck != null)
            {
                var currentUser = (User) Session["CurrentUser"];
                var taskList = _taskListsService.Get(t => t.PartitionKey == taskListOwnerId && t.RowKey == taskListId);
                _usersService.LoadShare(taskList);

                // Only a user in the taskList share list can share it.
                if (!_taskListsService.HasPermissionToEdit(currentUser, taskList))
                {
                    return View("Info", "_Layout", string.Format("The user {0} doesn't have permission to share the tasklist {1}.", currentUser.Name, taskList.Title));
                }

                foreach (var userRowKey in userCheck)
                {
                    _taskListsService.AddShare(taskList, userRowKey);
                }

                return RedirectToAction("Index", "TaskLists");
            }
            else
            {
                TempData["AlertMessage"] = "Please select at least one user to share.";
                return RedirectToAction("Share");
            }
        }

        #endregion Actions
    }
}