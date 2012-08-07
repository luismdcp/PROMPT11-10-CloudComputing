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

        #endregion Fields

        #region Constructors

        public TaskListsController(ITaskListsService taskListsService)
        {
            _taskListsService = taskListsService;
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
            var taskListsUserIsAssociated = _taskListsService.GetTaskListsUserIsAssociated(currentUser).AsQueryable().OrderBy(sortOrder);
            var pageSize = AppConfiguration.PageSize;

            return View(taskListsUserIsAssociated.ToPagedList(page, pageSize));
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

                // Check if a tasklist already exists with the same title
                var existingTaskList = _taskListsService.GetByTitleAndOwner(taskList.Title, currentUser);

                if (existingTaskList != null)
                {
                    ViewBag.ValidationErrors = new List<ValidationResult> { new ValidationResult(string.Format("The TaskList with the title '{0}' already exists.", taskList.Title)) };
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
                ViewBag.ValidationErrors = new List<ValidationResult> { new ValidationResult(string.Format("Create TaskList exception: {0}", ex.Message)) };
            }

            return View(taskList);
        }

        // GET: /TaskLists/Details/TaskList-Title
        public ActionResult Details(string taskListTitle)
        {
            var currentUser = (User) Session["CurrentUser"];
            var taskList = _taskListsService.GetTaskListEagerLoaded(taskListTitle, currentUser);
                        
            return taskList == null ? View("Info", "_Layout", string.Format("The TaskList with the title '{0}' doesn’t exist or was deleted.", taskListTitle.Replace('-', ' '))) : View(taskList);
        }

        // GET: /TaskLists/Edit/TaskList-Title
        public ActionResult Edit(string taskListTitle)
        {
            var currentUser = (User) Session["CurrentUser"];
            var taskList = _taskListsService.GetTaskListEagerLoaded(taskListTitle, currentUser);

            return View(taskList);
        }

        // POST: /TaskLists/Edit/TaskList-Title
        [HttpPost, ActionName("Edit")]
        public ActionResult EditPost(string taskListTitle)
        {
            var currentUser = (User) Session["CurrentUser"];
            var taskList = _taskListsService.GetTaskListEagerLoaded(taskListTitle, currentUser);

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

        // GET: /TaskLists/Delete/TaskList-Title
        public ActionResult Delete(string taskListTitle)
        {
            var currentUser = (User) Session["CurrentUser"];
            var taskList = _taskListsService.GetTaskListEagerLoaded(taskListTitle, currentUser);

            return taskList == null ? View("Info", "_Layout", string.Format("The TaskList with the title '{0}' doesn’t exist or was deleted.", taskListTitle.Replace('-', ' '))) : View(taskList);
        }

        // POST: /TaskLists/Delete/TaskList-Title
        [HttpPost, ActionName("Delete")]
        public ActionResult DeletePost(string taskListTitle)
        {
            var currentUser = (User) Session["CurrentUser"];
            var taskList = _taskListsService.GetByTitleAndOwner(taskListTitle.Replace('-', ' '), currentUser);

            // Check if the TaskList is being deleted by the user that created it.
            if (taskList.PartitionKey != currentUser.RowKey)
            {
                ViewBag.ValidationErrors = new List<ValidationResult> { new ValidationResult(string.Format("The TaskList with the title '{0}' can only be deleted by its owner.", taskList.Title)) };
                return View(taskList);
            }

            _taskListsService.Delete(taskList);
            return RedirectToAction("Index");
        }

        #endregion Actions
    }
}