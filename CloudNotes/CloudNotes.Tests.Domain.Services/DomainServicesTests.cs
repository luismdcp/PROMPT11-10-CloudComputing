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
            var repository = new NotesMemoryRepository();
            var service = new NotesService(unitOfWorkMock.Object, repository);

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
            var repository = new NotesMemoryRepository();
            var service = new NotesService(unitOfWorkMock.Object, repository);

            // Act
            var result = service.Get("taskList1", "note1");

            // Assert
            Assert.IsInstanceOfType(result, typeof(Note));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void NotesServiceMethodCreateShouldCreateANote()
        {
            // Arrange
            var note = new Note("taskList1", "note3") { Owner = new User("users", "user1") };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new NotesMemoryRepository();
            var service = new NotesService(unitOfWorkMock.Object, repository);

            // Act
            service.Create(note);
            var result = service.Get("taskList1", "note3");

            // Assert
            Assert.IsInstanceOfType(result, typeof(Note));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void NotesServiceMethodUpdateShouldUpdateANote()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new NotesMemoryRepository();
            var service = new NotesService(unitOfWorkMock.Object, repository);
            var result = service.Get("taskList1", "note1");

            // Act
            result.Content = "test content";
            service.Update(result);
            var updatedResult = service.Get("taskList1", "note1");

            // Assert
            Assert.IsTrue(updatedResult.Content == "test content");
        }

        [TestMethod]
        public void NotesServiceMethodDeleteShouldDeleteANote()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new NotesMemoryRepository();
            var service = new NotesService(unitOfWorkMock.Object, repository);
            var result = service.Get("taskList1", "note1");

            // Act
            service.Delete(result);
            result = service.Get("taskList1", "note1");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void NotesServiceMethodAddAssociatedUserShouldAssociateAUserToANote()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new NotesMemoryRepository();
            var service = new NotesService(unitOfWorkMock.Object, repository);
            var note = new Note("taskList1", "note1") { Owner = new User("users", "user1"), AssociatedUsers = { new User("users", "user1") } };
            var userToAssociate = new User("users", "user2");

            // Act
            service.AddAssociatedUser(note, userToAssociate);

            // Assert
            Assert.IsTrue(note.AssociatedUsers.Count == 2);
        }

        [TestMethod]
        public void NotesServiceMethodRemoveAssociatedUserShouldRemoveAnAssociatedUserToANote()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new NotesMemoryRepository();
            var service = new NotesService(unitOfWorkMock.Object, repository);
            var note = new Note("taskList1", "note1") { Owner = new User("users", "user1"), AssociatedUsers = { new User("users", "user1") } };
            var userToRemove = new User("users", "user1");

            // Act
            service.RemoveAssociatedUser(note, userToRemove);

            // Assert
            Assert.IsTrue(note.AssociatedUsers.Count == 0);
        }

        #endregion NotesService tests

        #region TaskListsService tests

        [TestMethod]
        public void TaskListsServiceMethodLoadShouldReturnAllTaskLists()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var taskListsRepository = new TaskListsMemoryRepository();
            var notesRepository = new NotesMemoryRepository();
            var service = new TaskListsService(unitOfWorkMock.Object, taskListsRepository, notesRepository);

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
            var taskListsRepository = new TaskListsMemoryRepository();
            var notesRepository = new NotesMemoryRepository();
            var service = new TaskListsService(unitOfWorkMock.Object, taskListsRepository, notesRepository);

            // Act
            var result = service.Get("user1", "taskList1");

            // Assert
            Assert.IsInstanceOfType(result, typeof(TaskList));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TaskListsServiceMethodCreateShouldCreateATaskList()
        {
            // Arrange
            var taskList = new TaskList("user1", "taskList3") { Owner = new User("users", "user1") };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var taskListsRepository = new TaskListsMemoryRepository();
            var notesRepository = new NotesMemoryRepository();
            var service = new TaskListsService(unitOfWorkMock.Object, taskListsRepository, notesRepository);

            // Act
            service.Create(taskList);
            var result = service.Get("user1", "taskList3");

            // Assert
            Assert.IsInstanceOfType(result, typeof(TaskList));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TaskListsServiceMethodUpdateShouldUpdateATaskList()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var taskListsRepository = new TaskListsMemoryRepository();
            var notesRepository = new NotesMemoryRepository();
            var service = new TaskListsService(unitOfWorkMock.Object, taskListsRepository, notesRepository);
            var result = service.Get("user1", "taskList1");

            // Act
            result.Title = "Test title";
            service.Update(result);
            var updatedResult = service.Get("user1", "taskList1");

            // Assert
            Assert.IsTrue(updatedResult.Title == "Test title");
        }

        [TestMethod]
        public void TaskListsServiceMethodDeleteShouldDeleteATaskList()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var taskListsRepository = new TaskListsMemoryRepository();
            var notesRepository = new NotesMemoryRepository();
            var service = new TaskListsService(unitOfWorkMock.Object, taskListsRepository, notesRepository);
            var result = service.Get("user1", "taskList1");

            // Act
            service.Delete(result);
            result = service.Get("user1", "taskList1");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TaskListsServiceMethodAddAssociatedUserShouldAssociateAUserToATaskList()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var taskListsRepository = new TaskListsMemoryRepository();
            var notesRepository = new NotesMemoryRepository();
            var service = new TaskListsService(unitOfWorkMock.Object, taskListsRepository, notesRepository);
            var taskList = new TaskList("user1", "taskList3") { Owner = new User("users", "user1"), AssociatedUsers = { new User("users", "user1") } };
            var userToAssociate = new User("users", "user2");

            // Act
            service.AddAssociatedUser(taskList, userToAssociate);

            // Assert
            Assert.IsTrue(taskList.AssociatedUsers.Count == 2);
        }

        [TestMethod]
        public void TaskListsServiceMethodRemoveAssociatedUserShouldRemoveAnAssociatedUserToATaskList()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var taskListsRepository = new TaskListsMemoryRepository();
            var notesRepository = new NotesMemoryRepository();
            var service = new TaskListsService(unitOfWorkMock.Object, taskListsRepository, notesRepository);
            var taskList = new TaskList("user1", "taskList3") { Owner = new User("users", "user1"), AssociatedUsers = { new User("users", "user1") } };
            var userToRemove = new User("users", "user1");

            // Act
            service.RemoveAssociatedUser(taskList, userToRemove);

            // Assert
            Assert.IsTrue(taskList.AssociatedUsers.Count == 0);
        }

        [TestMethod]
        public void TaskListsServiceMethodCopyNoteShouldCopyANoteToAnotherTaskList()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var taskListsRepository = new TaskListsMemoryRepository();
            var notesRepository = new NotesMemoryRepository();
            var service = new TaskListsService(unitOfWorkMock.Object, taskListsRepository, notesRepository);
            var note = new Note("taskList1", "note1") { Owner = new User("users", "user1") };
            var taskListSource = taskListsRepository.Get("user1", "taskList1");
            var taskListDestination = taskListsRepository.Get("user2", "taskList2");

            // Act
            service.CopyNote(taskListSource, taskListDestination, note);
            var copiedNote = notesRepository.Get("taskList2", "note1");

            // Assert
            Assert.IsInstanceOfType(copiedNote, typeof(Note));
            Assert.IsNotNull(copiedNote);
        }

        [TestMethod]
        public void TaskListsServiceMethodMoveNoteShouldMoveANoteToAnotherTaskList()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var taskListsRepository = new TaskListsMemoryRepository();
            var notesRepository = new NotesMemoryRepository();
            var service = new TaskListsService(unitOfWorkMock.Object, taskListsRepository, notesRepository);
            var note = new Note("taskList1", "note1") { Owner = new User("users", "user1") };
            var taskListSource = taskListsRepository.Get("user1", "taskList1");
            var taskListDestination = taskListsRepository.Get("user2", "taskList2");

            // Act
            service.MoveNote(taskListSource, taskListDestination, note);
            var copiedNote = notesRepository.Get("taskList1", "note1");

            // Assert
            Assert.IsNull(copiedNote);
        }

        [TestMethod]
        public void TaskListsServiceMethodGetTaskListsAssociatedByUserShouldAllTaskListsThatTheUserIsAssociated()
        {
            // Arrange
            var user = new User("users", "user3");
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var taskListsRepository = new TaskListsMemoryRepository();
            var notesRepository = new NotesMemoryRepository();
            var service = new TaskListsService(unitOfWorkMock.Object, taskListsRepository, notesRepository);

            // Act
            var result = service.GetTaskListsAssociatedByUser(user);

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
        public void UsersServiceMethodGetShouldReturnAUser()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var service = new UsersService(unitOfWorkMock.Object, repository);

            // Act
            var result = service.Get("users", "user1");

            // Assert
            Assert.IsInstanceOfType(result, typeof(User));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void UsersServiceMethodCreateShouldCreateAUser()
        {
            // Arrange
            var user = new User("users", "user3");
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var service = new UsersService(unitOfWorkMock.Object, repository);

            // Act
            service.Create(user);
            var result = service.Get("users", "user3");

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
            var result = service.Get("users", "user1");

            // Act
            result.Email = "test@test.com";
            service.Update(result);
            var updatedResult = service.Get("users", "user1");

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
            var result = service.Get("users", "user1");

            // Act
            service.Delete(result);
            result = service.Get("users", "user1");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void UsersServiceMethodGetOrAddCurrentUserShouldAddAndReturnANonExistentUser()
        {
            // Arrange
            var claims = new[]
                                 {
                                     new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "testIdentityProvider") ,
                                     new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "user3"),
                                     new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", "test@test.com")
                                 };

            IClaimsIdentity identity = new ClaimsIdentity(claims);
            IClaimsPrincipal principal = new ClaimsPrincipal(new[] { identity });
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var service = new UsersService(unitOfWorkMock.Object, repository);

            // Act
            var result = service.GetOrAddCurrentUser(principal);
            var allUsers = service.Load();

            // Assert
            Assert.IsTrue(allUsers.Count() == 4);
            Assert.IsInstanceOfType(result, typeof(User));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void UsersServiceMethodGetOrAddCurrentUserShouldAddAndReturnAExistentUser()
        {
            // Arrange
            var claims = new[]
                                 {
                                     new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "testIdentityProvider") ,
                                     new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "user2"),
                                     new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", "test@test.com")
                                 };

            IClaimsIdentity identity = new ClaimsIdentity(claims);
            IClaimsPrincipal principal = new ClaimsPrincipal(new[] { identity });
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var service = new UsersService(unitOfWorkMock.Object, repository);

            // Act
            var result = service.GetOrAddCurrentUser(principal);
            var allUsers = service.Load();

            // Assert
            Assert.IsTrue(allUsers.Count() == 3);
            Assert.IsInstanceOfType(result, typeof(User));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void UsersServiceMethodLoadNoteOwnerShouldLoadOwner()
        {
            // Arrange
            var note = new Note("taskList1", "note1");
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var service = new UsersService(unitOfWorkMock.Object, repository);

            // Act
            service.LoadNoteOwner(note);

            // Assert
            Assert.IsNotNull(note.Owner);
        }

        [TestMethod]
        public void UsersServiceMethodLoadTaskListOwnerShouldLoadOwner()
        {
            // Arrange
            var taskList = new TaskList("user1", "taskList1");
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var service = new UsersService(unitOfWorkMock.Object, repository);

            // Act
            service.LoadTaskListOwner(taskList);

            // Assert
            Assert.IsNotNull(taskList.Owner);
        }

        [TestMethod]
        public void UsersServiceMethodLoadNoteAssociatedUsersShouldLoadAssociatedUsers()
        {
            // Arrange
            var note = new Note("taskList1", "note1");
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var service = new UsersService(unitOfWorkMock.Object, repository);

            // Act
            service.LoadNoteAssociatedUsers(note);

            // Assert
            Assert.IsTrue(note.AssociatedUsers.Count == 2);
        }

        [TestMethod]
        public void UsersServiceMethodLoadTaskListAssociatedUsersShouldLoadAssociatedUsers()
        {
            // Arrange
            var taskList = new TaskList("user1", "taskList1");
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersMemoryRepository();
            var service = new UsersService(unitOfWorkMock.Object, repository);

            // Act
            service.LoadTaskListAssociatedUsers(taskList);

            // Assert
            Assert.IsTrue(taskList.AssociatedUsers.Count == 2);
        }

        #endregion UsersService tests
    }
}