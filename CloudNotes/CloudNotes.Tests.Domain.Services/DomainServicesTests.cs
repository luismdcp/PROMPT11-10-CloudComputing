using System.Collections.Generic;
using System.Linq;
using CloudNotes.Domain.Entities;
using CloudNotes.Domain.Services.Implementation;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Tests.Domain.Services.Memory;
using Microsoft.IdentityModel.Claims;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CloudNotes.Tests.Domain.Services
{
    [TestClass]
    public class DomainServicesTests
    {
        #region NotesService tests

        [TestMethod]
        public void NotesServiceMethodLoadShouldReturnAllNotes()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var taskListsRepositoryMock = new Mock<ITaskListsRepository>();
            var notesRepositoryMock = new Mock<INotesRepository>();
            var taskListsService = new TaskListsService(unitOfWorkMock.Object, taskListsRepositoryMock.Object, notesRepositoryMock.Object);
            var repository = new NotesMemoryRepository();
            var service = new NotesService(unitOfWorkMock.Object, repository, taskListsService);

            // Act
            var result = service.Load();

            // Assert
            Assert.IsInstanceOfType(result, typeof(IQueryable<Note>));
            Assert.IsTrue(result.Count() == 2);
        }

        [TestMethod]
        public void NotesServiceMethodGetShouldReturnANote()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var taskListsRepositoryMock = new Mock<ITaskListsRepository>();
            var notesRepositoryMock = new Mock<INotesRepository>();
            var taskListsService = new TaskListsService(unitOfWorkMock.Object, taskListsRepositoryMock.Object, notesRepositoryMock.Object);
            var repository = new NotesMemoryRepository();
            var service = new NotesService(unitOfWorkMock.Object, repository, taskListsService);

            // Act
            var result = service.Get(NotesMemoryRepository.User1RowKey, repository.Note1RowKey);

            // Assert
            Assert.IsInstanceOfType(result, typeof(Note));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void NotesServiceMethodGetShouldReturnNullForANonExistingNote()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var taskListsRepositoryMock = new Mock<ITaskListsRepository>();
            var notesRepositoryMock = new Mock<INotesRepository>();
            var taskListsService = new TaskListsService(unitOfWorkMock.Object, taskListsRepositoryMock.Object, notesRepositoryMock.Object);
            var repository = new NotesMemoryRepository();
            var service = new NotesService(unitOfWorkMock.Object, repository, taskListsService);

            // Act
            var result = service.Get(NotesMemoryRepository.User3RowKey, repository.Note3RowKey);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void NotesServiceMethodGetByFilterShouldReturnANote()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var taskListsRepositoryMock = new Mock<ITaskListsRepository>();
            var notesRepositoryMock = new Mock<INotesRepository>();
            var taskListsService = new TaskListsService(unitOfWorkMock.Object, taskListsRepositoryMock.Object, notesRepositoryMock.Object);
            var repository = new NotesMemoryRepository();
            var service = new NotesService(unitOfWorkMock.Object, repository, taskListsService);

            // Act
            var result = service.Get(n => n.PartitionKey == NotesMemoryRepository.User1RowKey & n.RowKey == repository.Note1RowKey);
            
            // Assert
            Assert.IsInstanceOfType(result, typeof(Note));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void NotesServiceMethodCreateShouldCreateANote()
        {
            // Arrange
            var user = new User { PartitionKey = NotesMemoryRepository.IdentityProvider, RowKey = NotesMemoryRepository.User1RowKey };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var taskListsRepositoryMock = new Mock<ITaskListsRepository>();
            var notesRepositoryMock = new Mock<INotesRepository>();
            var taskListsService = new TaskListsService(unitOfWorkMock.Object, taskListsRepositoryMock.Object, notesRepositoryMock.Object);
            var repository = new NotesMemoryRepository();
            var note = new Note { PartitionKey = NotesMemoryRepository.User1RowKey, RowKey = repository.Note1RowKey, Owner = user };
            var service = new NotesService(unitOfWorkMock.Object, repository, taskListsService);

            // Act
            service.Create(note);
            var result = service.Get(note.PartitionKey, note.RowKey);

            // Assert
            Assert.IsInstanceOfType(result, typeof(Note));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void NotesServiceMethodNoteWithTitleExistsShouldReturnTrueForAnExistingTitle()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var taskListsRepositoryMock = new Mock<ITaskListsRepository>();
            var notesRepositoryMock = new Mock<INotesRepository>();
            var taskListsService = new TaskListsService(unitOfWorkMock.Object, taskListsRepositoryMock.Object, notesRepositoryMock.Object);
            var repository = new NotesMemoryRepository();
            var taskList = new TaskList { PartitionKey = NotesMemoryRepository.User1RowKey, RowKey = repository.TaskList1RowKey };
            var service = new NotesService(unitOfWorkMock.Object, repository, taskListsService);

            // Act
            var result = service.NoteWithTitleExists("Test title", taskList);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NotesServiceMethodNoteWithTitleExistsShouldReturnFalseForNonExistingTitle()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var taskListsRepositoryMock = new Mock<ITaskListsRepository>();
            var notesRepositoryMock = new Mock<INotesRepository>();
            var taskListsService = new TaskListsService(unitOfWorkMock.Object, taskListsRepositoryMock.Object, notesRepositoryMock.Object);
            var repository = new NotesMemoryRepository();
            var taskList = new TaskList { PartitionKey = NotesMemoryRepository.User1RowKey, RowKey = repository.TaskList1RowKey };
            var service = new NotesService(unitOfWorkMock.Object, repository, taskListsService);

            // Act
            var result = service.NoteWithTitleExists("Title", taskList);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void NotesServiceMethodUpdateShouldUpdateANote()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var taskListsRepositoryMock = new Mock<ITaskListsRepository>();
            var notesRepositoryMock = new Mock<INotesRepository>();
            var taskListsService = new TaskListsService(unitOfWorkMock.Object, taskListsRepositoryMock.Object, notesRepositoryMock.Object);
            var repository = new NotesMemoryRepository();
            var service = new NotesService(unitOfWorkMock.Object, repository, taskListsService);
            var result = service.Get(NotesMemoryRepository.User1RowKey, repository.Note1RowKey);

            // Act
            result.Content = "Test content";
            service.Update(result);
            var updatedResult = service.Get(NotesMemoryRepository.User1RowKey, repository.Note1RowKey);

            // Assert
            Assert.IsTrue(updatedResult.Content == "Test content");
        }

        [TestMethod]
        public void NotesServiceMethodDeleteShouldDeleteANote()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var taskListsRepositoryMock = new Mock<ITaskListsRepository>();
            var notesRepositoryMock = new Mock<INotesRepository>();
            var taskListsService = new TaskListsService(unitOfWorkMock.Object, taskListsRepositoryMock.Object, notesRepositoryMock.Object);
            var repository = new NotesMemoryRepository();
            var service = new NotesService(unitOfWorkMock.Object, repository, taskListsService);
            var result = service.Get(NotesMemoryRepository.User1RowKey, repository.Note1RowKey);

            // Act
            service.Delete(result);
            result = service.Get(NotesMemoryRepository.User1RowKey, repository.Note1RowKey);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void NotesServiceMethodAddShareShouldAddAUserToTheNoteShares()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var taskListsRepositoryMock = new Mock<ITaskListsRepository>();
            var notesRepositoryMock = new Mock<INotesRepository>();
            var taskListsService = new TaskListsService(unitOfWorkMock.Object, taskListsRepositoryMock.Object, notesRepositoryMock.Object);
            var repository = new NotesMemoryRepository();
            var note = new Note { PartitionKey = NotesMemoryRepository.User1RowKey, RowKey = repository.Note1RowKey };
            var service = new NotesService(unitOfWorkMock.Object, repository, taskListsService);
            var noteSharesCount = repository._noteShares.Count;

            // Act
            service.AddShare(note, NotesMemoryRepository.User2RowKey);

            // Assert
            Assert.IsTrue(repository._noteShares.Count == noteSharesCount + 1);
        }

        [TestMethod]
        public void NotesServiceMethodRemoveShareShouldRemoveAShareFromTheNoteShares()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var taskListsRepositoryMock = new Mock<ITaskListsRepository>();
            var notesRepositoryMock = new Mock<INotesRepository>();
            var taskListsService = new TaskListsService(unitOfWorkMock.Object, taskListsRepositoryMock.Object, notesRepositoryMock.Object);
            var repository = new NotesMemoryRepository();
            var note = new Note { PartitionKey = NotesMemoryRepository.User1RowKey, RowKey = repository.Note1RowKey };
            var service = new NotesService(unitOfWorkMock.Object, repository, taskListsService);
            var noteSharesCount = repository._noteShares.Count;

            // Act
            service.RemoveShare(note, NotesMemoryRepository.User1RowKey);

            // Assert
            Assert.IsTrue(repository._noteShares.Count == noteSharesCount - 1);
        }

        [TestMethod]
        public void NotesServiceMethodLoadNotesShouldLoadAllNotes()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var taskListsRepositoryMock = new Mock<ITaskListsRepository>();
            var notesRepositoryMock = new Mock<INotesRepository>();
            var taskListsService = new TaskListsService(unitOfWorkMock.Object, taskListsRepositoryMock.Object, notesRepositoryMock.Object);
            var repository = new NotesMemoryRepository();
            var taskList = new TaskList { PartitionKey = NotesMemoryRepository.User1RowKey, RowKey = repository.TaskList1RowKey };
            var service = new NotesService(unitOfWorkMock.Object, repository, taskListsService);

            // Act
            service.LoadNotes(taskList);

            // Assert
            Assert.IsTrue(taskList.Notes.Count == 2);
        }

        [TestMethod]
        public void NotesServiceMethodHasPermissionToEditShouldReturnTrueForAUserInTheShare()
        {
            // Arrange
            var user = new User { PartitionKey = NotesMemoryRepository.IdentityProvider, RowKey = NotesMemoryRepository.User1RowKey };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var taskListsRepositoryMock = new Mock<ITaskListsRepository>();
            var notesRepositoryMock = new Mock<INotesRepository>();
            var taskListsService = new TaskListsService(unitOfWorkMock.Object, taskListsRepositoryMock.Object, notesRepositoryMock.Object);
            var repository = new NotesMemoryRepository();
            var note = new Note { PartitionKey = NotesMemoryRepository.User1RowKey, RowKey = repository.Note1RowKey, Owner = user };
            var service = new NotesService(unitOfWorkMock.Object, repository, taskListsService);

            // Act
            var result = service.HasPermissionToEdit(user, note);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NotesServiceMethodHasPermissionToEditShouldReturnFalseForAUserNotInTheShare()
        {
            // Arrange
            var user = new User { PartitionKey = NotesMemoryRepository.IdentityProvider, RowKey = NotesMemoryRepository.User3RowKey };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var taskListsRepositoryMock = new Mock<ITaskListsRepository>();
            var notesRepositoryMock = new Mock<INotesRepository>();
            var taskListsService = new TaskListsService(unitOfWorkMock.Object, taskListsRepositoryMock.Object, notesRepositoryMock.Object);
            var repository = new NotesMemoryRepository();
            var note = new Note { PartitionKey = NotesMemoryRepository.User1RowKey, RowKey = repository.Note1RowKey };
            var service = new NotesService(unitOfWorkMock.Object, repository, taskListsService);

            // Act
            var result = service.HasPermissionToEdit(user, note);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void NotesServiceMethodCopyNoteShouldCopyANote()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var taskListsRepositoryMock = new Mock<ITaskListsRepository>();
            var notesRepositoryMock = new Mock<INotesRepository>();
            var taskListsService = new TaskListsService(unitOfWorkMock.Object, taskListsRepositoryMock.Object, notesRepositoryMock.Object);
            var repository = new NotesMemoryRepository();
            var user = new User { PartitionKey = NotesMemoryRepository.IdentityProvider, RowKey = NotesMemoryRepository.User1RowKey };
            var note = new Note { PartitionKey = NotesMemoryRepository.User1RowKey, RowKey = repository.Note1RowKey, Owner = user };
            var taskList = new TaskList { PartitionKey = NotesMemoryRepository.TaskList2PartitionKey, RowKey = repository.TaskList2RowKey };
            var service = new NotesService(unitOfWorkMock.Object, repository, taskListsService);

            // Act
            service.CopyNote(note, taskList);
            service.LoadNotes(taskList);

            // Assert
            //Assert.IsInstanceOfType(result, typeof(IQueryable<Note>));
            //Assert.IsTrue(result.Count() == 2);
        }

        #endregion NotesService tests

        #region TaskListsService tests

        [TestMethod]
        public void TaskListsServiceMethodLoadShouldReturnAllTaskLists()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new TaskListsMemoryRepository();
            var notesRepository = new NotesMemoryRepository();
            var service = new TaskListsService(unitOfWorkMock.Object, repository, notesRepository);

            // Act
            var result = service.Load();

            // Assert
            Assert.IsInstanceOfType(result, typeof(IQueryable<TaskList>));
            Assert.IsTrue(result.Count() == 2);
        }

        [TestMethod]
        public void TaskListsServiceMethodGetShouldReturnATaskList()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new TaskListsMemoryRepository();
            var notesRepository = new NotesMemoryRepository();
            var service = new TaskListsService(unitOfWorkMock.Object, repository, notesRepository);

            // Act
            var result = service.Get(TaskListsMemoryRepository.User1RowKey, repository.TaskList1RowKey);

            // Assert
            Assert.IsInstanceOfType(result, typeof(TaskList));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TaskListsServiceMethodGetByCombinedKeysShouldReturnATaskList()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new TaskListsMemoryRepository();
            var notesRepository = new NotesMemoryRepository();
            var service = new TaskListsService(unitOfWorkMock.Object, repository, notesRepository);

            // Act
            var result = service.Get(string.Format("{0}+{1}", TaskListsMemoryRepository.User1RowKey, repository.TaskList1RowKey));

            // Assert
            Assert.IsInstanceOfType(result, typeof(TaskList));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TaskListsServiceMethodGetShouldReturnNullForANonExistingTaskList()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new TaskListsMemoryRepository();
            var notesRepository = new NotesMemoryRepository();
            var service = new TaskListsService(unitOfWorkMock.Object, repository, notesRepository);

            // Act
            var result = service.Get(TaskListsMemoryRepository.User1RowKey, repository.TaskList3RowKey);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TaskListsServiceMethodGetByFilterShouldReturnATaskList()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new TaskListsMemoryRepository();
            var notesRepository = new NotesMemoryRepository();
            var service = new TaskListsService(unitOfWorkMock.Object, repository, notesRepository);

            // Act
            var result = service.Get(t => t.PartitionKey == TaskListsMemoryRepository.User1RowKey && t.RowKey == repository.TaskList1RowKey);

            // Assert
            Assert.IsInstanceOfType(result, typeof(TaskList));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TaskListsServiceMethodCreateShouldCreateATaskList()
        {
            // Arrange

            var user = new User { PartitionKey = TaskListsMemoryRepository.IdentityProvider, RowKey = TaskListsMemoryRepository.User1RowKey };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new TaskListsMemoryRepository();
            var notesRepository = new NotesMemoryRepository();
            var taskList = new TaskList { PartitionKey = TaskListsMemoryRepository.User1RowKey, RowKey = repository.TaskList3RowKey, Owner = user };
            var service = new TaskListsService(unitOfWorkMock.Object, repository, notesRepository);

            // Act
            service.Create(taskList);
            var result = service.Get(NotesMemoryRepository.User1RowKey, repository.TaskList3RowKey);

            // Assert
            Assert.IsInstanceOfType(result, typeof(TaskList));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TaskListsServiceMethodUpdateShouldUpdateATaskList()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new TaskListsMemoryRepository();
            var notesRepository = new NotesMemoryRepository();
            var service = new TaskListsService(unitOfWorkMock.Object, repository, notesRepository);
            var result = service.Get(TaskListsMemoryRepository.User1RowKey, repository.TaskList1RowKey);

            // Act
            result.Title = "Test title";
            service.Update(result);
            var updatedResult = service.Get(TaskListsMemoryRepository.User1RowKey, repository.TaskList1RowKey);

            // Assert
            Assert.IsTrue(updatedResult.Title == "Test title");
        }

        [TestMethod]
        public void TaskListsServiceMethodDeleteShouldDeleteATaskList()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new TaskListsMemoryRepository();
            var notesRepository = new NotesMemoryRepository();
            var service = new TaskListsService(unitOfWorkMock.Object, repository, notesRepository);
            var result = service.Get(TaskListsMemoryRepository.User1RowKey, repository.TaskList1RowKey);
            var user = new User { PartitionKey = TaskListsMemoryRepository.IdentityProvider, RowKey = TaskListsMemoryRepository.User1RowKey };
            var note = new Note { PartitionKey = TaskListsMemoryRepository.User1RowKey, RowKey = repository.Note1RowKey, Owner = user };
            result.Notes.Add(note);

            // Act
            service.Delete(result);
            result = service.Get(TaskListsMemoryRepository.User1RowKey, repository.TaskList1RowKey);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TaskListsServiceMethodAddShareShouldAddAUserToATaskListShares()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new TaskListsMemoryRepository();
            var notesRepository = new NotesMemoryRepository();
            var taskList = new TaskList { PartitionKey = TaskListsMemoryRepository.User1RowKey, RowKey = repository.TaskList1RowKey };
            var service = new TaskListsService(unitOfWorkMock.Object, repository, notesRepository);
            var taskListSharesCount = repository.TaskListShares.Count;

            // Act
            service.AddShare(taskList, TaskListsMemoryRepository.User2RowKey);

            // Assert
            Assert.IsTrue(repository.TaskListShares.Count == taskListSharesCount + 1);
        }

        [TestMethod]
        public void TaskListsServiceMethodRemoveShareShouldARemoveAUserFromTheTaskListShares()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new TaskListsMemoryRepository();
            var notesRepository = new NotesMemoryRepository();
            var taskList = new TaskList { PartitionKey = TaskListsMemoryRepository.User1RowKey, RowKey = repository.TaskList1RowKey };
            var service = new TaskListsService(unitOfWorkMock.Object, repository, notesRepository);
            var taskListSharesCount = repository.TaskListShares.Count;

            // Act
            service.RemoveShare(taskList, TaskListsMemoryRepository.User3RowKey);

            // Assert
            Assert.IsTrue(repository.TaskListShares.Count == taskListSharesCount - 1);
        }

        [TestMethod]
        public void TaskListsServiceMethodHasPermissionToEditShouldReturnTrueForAUserInTheShare()
        {
            // Arrange
            var user = new User { PartitionKey = TaskListsMemoryRepository.IdentityProvider, RowKey = TaskListsMemoryRepository.User1RowKey };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new TaskListsMemoryRepository();
            var notesRepository = new NotesMemoryRepository();
            var taskList = new TaskList { PartitionKey = TaskListsMemoryRepository.User1RowKey, RowKey = repository.TaskList1RowKey, Owner = user};
            var service = new TaskListsService(unitOfWorkMock.Object, repository, notesRepository);

            // Act
            var result = service.HasPermissionToEdit(user, taskList);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TaskListsServiceMethodHasPermissionToEditShouldReturnFalseForAUserNotInTheShare()
        {
            // Arrange
            var user = new User { PartitionKey = TaskListsMemoryRepository.IdentityProvider, RowKey = TaskListsMemoryRepository.User2RowKey };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new TaskListsMemoryRepository();
            var notesRepository = new NotesMemoryRepository();
            var taskList = new TaskList { PartitionKey = TaskListsMemoryRepository.User1RowKey, RowKey = repository.TaskList1RowKey, Owner = user };
            var service = new TaskListsService(unitOfWorkMock.Object, repository, notesRepository);

            // Act
            var result = service.HasPermissionToEdit(user, taskList);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TaskListsServiceMethodGetSharedShouldReturnAllTaskListsThatTheUserIsInTheShare()
        {
            // Arrange
            var user = new User { PartitionKey = TaskListsMemoryRepository.IdentityProvider, RowKey = TaskListsMemoryRepository.User3RowKey };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var taskListsRepository = new TaskListsMemoryRepository();
            var notesRepository = new NotesMemoryRepository();
            var service = new TaskListsService(unitOfWorkMock.Object, taskListsRepository, notesRepository);

            // Act
            var result = service.GetShared(user);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IEnumerable<TaskList>));
            Assert.IsTrue(result.Count() == 2);
        }

        #endregion TaskListsService tests

        #region UsersService tests

        [TestMethod]
        public void UsersServiceMethodLoadShouldReturnAllUsers()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var service = new UsersService(unitOfWorkMock.Object, repository);

            // Act
            var result = service.Load();

            // Assert
            Assert.IsInstanceOfType(result, typeof(IQueryable<User>));
            Assert.IsTrue(result.Count() == 3);
        }

        [TestMethod]
        public void UsersServiceMethodGetShouldReturnAnExistingUser()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var service = new UsersService(unitOfWorkMock.Object, repository);

            // Act
            var result = service.Get(UsersMemoryRepository.IdentityProvider, UsersMemoryRepository.User1RowKey);

            // Assert
            Assert.IsInstanceOfType(result, typeof(User));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void UsersServiceMethodGetByRowkeyShouldReturnAnExistingUser()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var service = new UsersService(unitOfWorkMock.Object, repository);

            // Act
            var result = service.Get(UsersMemoryRepository.User1RowKey);

            // Assert
            Assert.IsInstanceOfType(result, typeof(User));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void UsersServiceMethodGetByFilterShouldReturnAUser()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var service = new UsersService(unitOfWorkMock.Object, repository);

            // Act
            var result = service.Get(u => u.PartitionKey == UsersMemoryRepository.IdentityProvider && u.RowKey == UsersMemoryRepository.User1RowKey);

            // Assert
            Assert.IsInstanceOfType(result, typeof(User));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void UsersServiceMethodGetShouldReturnNullForANonExistingUser()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var service = new UsersService(unitOfWorkMock.Object, repository);

            // Act
            var result = service.Get(UsersMemoryRepository.IdentityProvider, UsersMemoryRepository.User4RowKey);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void UsersServiceMethodCreateShouldCreateAUser()
        {
            // Arrange
            var user = new User { PartitionKey = UsersMemoryRepository.IdentityProvider, RowKey = UsersMemoryRepository.User4RowKey };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var service = new UsersService(unitOfWorkMock.Object, repository);

            // Act
            service.Create(user);
            var result = service.Get(UsersMemoryRepository.IdentityProvider, UsersMemoryRepository.User4RowKey);

            // Assert
            Assert.IsInstanceOfType(result, typeof(User));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void UsersServiceMethodUpdateShouldUpdateAUser()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var service = new UsersService(unitOfWorkMock.Object, repository);
            var result = service.Get(UsersMemoryRepository.IdentityProvider, UsersMemoryRepository.User1RowKey);

            // Act
            result.Email = "test@test.com";
            service.Update(result);
            var updatedResult = service.Get(UsersMemoryRepository.IdentityProvider, UsersMemoryRepository.User1RowKey);

            // Assert
            Assert.IsTrue(updatedResult.Email == "test@test.com");
        }

        [TestMethod]
        public void UsersServiceMethodDeleteShouldDeleteAUser()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var service = new UsersService(unitOfWorkMock.Object, repository);
            var result = service.Get(UsersMemoryRepository.IdentityProvider, UsersMemoryRepository.User1RowKey);

            // Act
            service.Delete(result);
            result = service.Get(UsersMemoryRepository.IdentityProvider, UsersMemoryRepository.User1RowKey);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void UsersServiceMethodIsRegisteredReturnsTrueForAnExistingUser()
        {
            // Arrange
            var claims = new[]
                                     {
                                         new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "WindowsLiveID") ,
                                         new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "user1"),
                                         new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", "test@test.com")
                                     };

            IClaimsIdentity identity = new ClaimsIdentity(claims);
            IClaimsPrincipal principal = new ClaimsPrincipal(new[] { identity });
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var service = new UsersService(unitOfWorkMock.Object, repository);

            // Act
            var result = service.UserIsRegistered(principal);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UsersServiceMethodIsRegisteredReturnsFalseForANonExistingUser()
        {
            // Arrange
            var claims = new[]
                                     {
                                         new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "WindowsLiveID") ,
                                         new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "user4"),
                                         new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", "test@test.com")
                                     };

            IClaimsIdentity identity = new ClaimsIdentity(claims);
            IClaimsPrincipal principal = new ClaimsPrincipal(new[] { identity });
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var service = new UsersService(unitOfWorkMock.Object, repository);

            // Act
            var result = service.UserIsRegistered(principal);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UsersServiceMethodGetByIdentifiersShouldReturnAUser()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var service = new UsersService(unitOfWorkMock.Object, repository);

            // Act
            var result = service.GetByIdentifiers("user1", UsersMemoryRepository.IdentityProvider);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void UsersServiceMethodLoadNoteOwnerShouldLoadOwner()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var note = new Note { PartitionKey = UsersMemoryRepository.Note1PartitionKey, RowKey = repository.Note1RowKey };
            var service = new UsersService(unitOfWorkMock.Object, repository);

            // Act
            service.LoadOwner(note);

            // Assert
            Assert.IsNotNull(note.Owner);
        }

        [TestMethod]
        public void UsersServiceMethodLoadTaskListOwnerShouldLoadOwner()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var taskList = new TaskList { PartitionKey = UsersMemoryRepository.TaskList1PartitionKey, RowKey = repository.Note1RowKey };
            var service = new UsersService(unitOfWorkMock.Object, repository);

            // Act
            service.LoadOwner(taskList);

            // Assert
            Assert.IsNotNull(taskList.Owner);
        }

        [TestMethod]
        public void UsersServiceMethodLoadShareShouldLoadTheNoteShares()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var note = new Note { PartitionKey = UsersMemoryRepository.Note1PartitionKey, RowKey = repository.Note1RowKey };
            var service = new UsersService(unitOfWorkMock.Object, repository);

            // Act
            service.LoadShare(note);

            // Assert
            Assert.IsTrue(note.Share.Count == 2);
        }

        [TestMethod]
        public void UsersServiceMethodLoadShareShouldLoadTheTaskListShares()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var taskList = new TaskList { PartitionKey = UsersMemoryRepository.TaskList1PartitionKey, RowKey = repository.TaskList1RowKey };
            var service = new UsersService(unitOfWorkMock.Object, repository);

            // Act
            service.LoadShare(taskList);

            // Assert
            Assert.IsTrue(taskList.Share.Count == 2);
        }

        [TestMethod]
        public void UsersServiceMethodGetUserAuthenticationInfoShouldReturnTheAuthenticationInfoWithNameAndEmail()
        {
            // Arrange
            var claims = new[]
                                     {
                                         new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "WindowsLiveID") ,
                                         new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "user1"),
                                         new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", "test@test.com"),
                                         new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", "User1"), 
                                     };

            IClaimsIdentity identity = new ClaimsIdentity(claims);
            IClaimsPrincipal principal = new ClaimsPrincipal(new[] { identity });
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var service = new UsersService(unitOfWorkMock.Object, repository);

            // Act
            var result = service.GetUserAuthenticationInfo(principal);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void UsersServiceMethodGetUserAuthenticationInfoShouldReturnTheAuthenticationInfoWithoutNameAndEmail()
        {
            // Arrange
            var claims = new[]
                                     {
                                         new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "WindowsLiveID") ,
                                         new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "user1")
                                     };

            IClaimsIdentity identity = new ClaimsIdentity(claims);
            IClaimsPrincipal principal = new ClaimsPrincipal(new[] { identity });
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var service = new UsersService(unitOfWorkMock.Object, repository);

            // Act
            var result = service.GetUserAuthenticationInfo(principal);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void UsersServiceMethodGFillUserAuthenticationInfoShouldFillTheAuthenticationInfo()
        {
            // Arrange
            var claims = new[]
                                     {
                                         new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "WindowsLiveID") ,
                                         new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "user1"),
                                         new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", "test@test.com"),
                                         new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", "user1"), 
                                     };

            IClaimsIdentity identity = new ClaimsIdentity(claims);
            IClaimsPrincipal principal = new ClaimsPrincipal(new[] { identity });
            var user = new User { Name = "user1" };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var service = new UsersService(unitOfWorkMock.Object, repository);

            // Act
            service.FillAuthenticationInfo(user, principal);

            // Assert
            Assert.IsTrue(user.PartitionKey == "windowsliveid");
            Assert.IsTrue(user.RowKey == "user1-windowsliveid");
            Assert.IsTrue(user.UniqueIdentifier == "user1");
        }

        [TestMethod]
        public void UserServiceMethodParseIdentityProviderShouldParseIdentityProviders()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var service = new UsersService(unitOfWorkMock.Object, repository);

            // Act
            var windowsLiveIdProvider = service.ParseIdentityProvider("WindowsLiveID");
            var googleProvider = service.ParseIdentityProvider("Google");
            var yahooProvider = service.ParseIdentityProvider("Yahoo");

            // Assert
            Assert.IsTrue(windowsLiveIdProvider == "windowsliveid");
            Assert.IsTrue(googleProvider == "google");
            Assert.IsTrue(yahooProvider == "yahoo");
        }

        #endregion UsersService tests
    }
}