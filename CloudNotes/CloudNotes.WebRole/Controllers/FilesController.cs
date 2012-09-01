using System.Web;
using System.Web.Mvc;
using CloudNotes.Domain.Entities;
using CloudNotes.Domain.Services.Contracts;

namespace CloudNotes.WebRole.Controllers
{
    [Authorize]
    public class FilesController : Controller
    {
        #region Fields

        private readonly IFilesService _filesService;
        private readonly INotesService _notesService;
        private readonly ITaskListsService _taskListsService;

        #endregion Fields

        #region Constructors

        public FilesController(IFilesService filesService, INotesService notesService, ITaskListsService taskListsService)
        {
            _filesService = filesService;
            _notesService = notesService;
            _taskListsService = taskListsService;
        }

        #endregion Constructors

        #region Actions

        // GET: /Files/Note/{noteOwnerId}/{noteId}/Index
        public ActionResult Index(string noteOwnerId, string noteId)
        {
            var currentUser = (User) Session["CurrentUser"];
            var note = _notesService.Get(t => t.PartitionKey == noteOwnerId && t.RowKey == noteId);
            _taskListsService.LoadContainer(note);
            ViewBag.ContainerOwnerId = note.Container.PartitionKey;
            ViewBag.ContainerId = note.Container.RowKey;

            // Only a user in the note share list can list the files attached to it.
            if (!_notesService.HasPermissionToEdit(currentUser, note))
            {
                return View("Info", "_Layout", string.Format("The user '{0}' doesn't have permission to download a file attached to the note '{1}'.", currentUser.Name, note.Title));
            }

            var noteFiles = _filesService.Get(noteOwnerId, noteId);
            ViewBag.NoteOwnerId = noteOwnerId;
            ViewBag.NoteId = noteId;
            return View(noteFiles);
        }

        // GET: /Files/Note/{noteOwnerId}/{noteId}/Download/{fileName}
        public ActionResult Download(string noteOwnerId, string noteId, string fileName)
        {
            var currentUser = (User) Session["CurrentUser"];
            var note = _notesService.Get(t => t.PartitionKey == noteOwnerId && t.RowKey == noteId);

            // Only a user in the note share list can download a file attached to it.
            if (!_notesService.HasPermissionToEdit(currentUser, note))
            {
                return View("Info", "_Layout", string.Format("The user '{0}' doesn't have permission to download a file attached to the note '{1}'.", currentUser.Name, note.Title));
            }

            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            _filesService.Download(noteOwnerId, noteId, fileName, Response.OutputStream);
            return new EmptyResult();
        }


        [HttpPost]
        public ActionResult Upload(string noteOwnerId, string noteId, HttpPostedFileBase file)
        {
            // check if the file name does not contain the characters - and +
            if (!_filesService.IsValidName(file.FileName))
            {
                return View("Info", "_Layout", string.Format("The file name '{0}' is not valid. It cannot contain the characters '-' or '+'", file.FileName));
            }

            var currentUser = (User) Session["CurrentUser"];
            var note = _notesService.Get(t => t.PartitionKey == noteOwnerId && t.RowKey == noteId);

            // Only a user in the note share list can attach a file to it.
            if (!_notesService.HasPermissionToEdit(currentUser, note))
            {
                return View("Info", "_Layout", string.Format("The user '{0}' doesn't have permission to download a file attached to the note '{1}'.", currentUser.Name, note.Title));
            }

            if (file != null && file.ContentLength > 0)
            { 
                _filesService.Create(noteOwnerId, noteId, file.FileName, file.InputStream, file.ContentType);
            }
         
            return RedirectToAction("Index");
        }

        // GET: /Files/Note/{noteOwnerId}/{noteId}/Delete/{fileName}
        public ActionResult Delete(string noteOwnerId, string noteId, string fileName)
        {
            ViewBag.NoteOwnerId = noteOwnerId;
            ViewBag.NoteId = noteId;
            ViewBag.FileName = fileName;
            return View();
        }

        // POST: /Files/Note/{noteOwnerId}/{noteId}/Delete/{fileName}
        [HttpPost, ActionName("Delete")]
        public ActionResult DeletePost(string noteOwnerId, string noteId, string fileName)
        {
            var currentUser = (User) Session["CurrentUser"];
            var note = _notesService.Get(t => t.PartitionKey == noteOwnerId && t.RowKey == noteId);

            // Only a user in the note share list can delete an attached file to it.
            if (!_notesService.HasPermissionToEdit(currentUser, note))
            {
                return View("Info", "_Layout", string.Format("The user '{0}' doesn't have permission to download a file attached to the note '{1}'.", currentUser.Name, note.Title));
            }

            _filesService.Delete(noteOwnerId, noteId, fileName);
            return RedirectToAction("Index");
        }

        #endregion Actions
    }
}