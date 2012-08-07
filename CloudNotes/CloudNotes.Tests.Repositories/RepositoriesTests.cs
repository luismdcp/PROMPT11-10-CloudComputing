using System.Collections.Generic;
using System.Linq;
using CloudNotes.Domain.Entities;
using CloudNotes.Repositories.Contracts;
using CloudNotes.Repositories.Entities;
using CloudNotes.Repositories.Entities.Relation;
using CloudNotes.Repositories.Implementation;
using Microsoft.IdentityModel.Claims;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CloudNotes.Tests.Repositories
{
    [TestClass]
    public class RepositoriesTests
    {
        #region NotesRepository tests

        [TestMethod]
        public void NotesRepositoryLoadCallsLoadFromTheUnitOfWork()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Load<NoteTableEntry>(It.Is<string>(s => s == "Notes"))).Returns(BuildNotesTable());
            var repository = new NotesRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.Load().ToList();

            // Assert
            Assert.IsTrue(result.Count() == 2);
            unitOfWorkMock.Verify(uow => uow.Load<NoteTableEntry>("Notes"), Times.Once());
        }

        [TestMethod]
        public void NotesRepositoryGetCallsGetFromTheUnitOfWorkAndReturnsAExistentNote()
        {
            // Arrange
            var noteTableEntry = new NoteTableEntry("taskList1", "note1");
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Get<NoteTableEntry>("Notes", "taskList1", "note1")).Returns(noteTableEntry);
            var repository = new NotesRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.Get("taskList1", "note1");

            // Assert
            Assert.IsNotNull(result);
            unitOfWorkMock.Verify(uow => uow.Get<NoteTableEntry>("Notes", "taskList1", "note1"), Times.Once());
        }

        [TestMethod]
        public void NotesRepositoryGetCallsGetFromTheUnitOfWorkAndReturnsNullForANonExistentNote()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new NotesRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.Get("taskList1", "note3");

            // Assert
            Assert.IsNull(result);
            unitOfWorkMock.Verify(uow => uow.Get<NoteTableEntry>("Notes", "taskList1", "note3"), Times.Once());
        }

        [TestMethod]
        public void NotesRepositoryCreateCallsCreateFromTheUnitOfWork()
        {
            // Arrange
            var note = new Note("taskList1", "note1") { Owner = new User("users", "user1") };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new NotesRepository(unitOfWorkMock.Object);

            // Act
            repository.Create(note);

            // Assert
            unitOfWorkMock.Verify(uow => uow.Create(It.IsAny<NoteTableEntry>(), "Notes"), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Create(It.IsAny<NoteOwnerTableEntry>(), "NoteOwner"), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Create(It.IsAny<NoteAssociatedUserTableEntry>(), "NoteAssociatedUsers"), Times.Once());
        }

        [TestMethod]
        public void NotesRepositoryUpdateCallsUpdateFromTheUnitOfWork()
        {
            // Arrange
            var note = new Note("taskList1", "note1") { Owner = new User("users", "user1") };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new NotesRepository(unitOfWorkMock.Object);

            // Act
            repository.Update(note);

            // Assert
            unitOfWorkMock.Verify(uow => uow.Update(It.IsAny<NoteTableEntry>()), Times.Once());
        }

        [TestMethod]
        public void NotesRepositoryDeleteCallsDeleteFromTheUnitOfWork()
        {
            // Arrange
            var note = new Note("taskList1", "note1") { Owner = new User("users", "user1") };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Load<NoteAssociatedUserTableEntry>(It.Is<string>(s => s == "NoteAssociatedUsers"))).Returns(BuildNoteAssociatedUsersTable());
            var repository = new NotesRepository(unitOfWorkMock.Object);

            // Act
            repository.Delete(note);

            // Assert
            unitOfWorkMock.Verify(uow => uow.Delete(It.IsAny<NoteTableEntry>()), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Delete(It.IsAny<NoteAssociatedUserTableEntry>()), Times.Exactly(2));
        }

        [TestMethod]
        public void NotesRepositoryAddAssociatedUserCallsCreateFromTheUnitOfWork()
        {
            // Arrange
            var note = new Note("taskList1", "note1") { Owner = new User("users", "user1"), AssociatedUsers = { new User("users", "user1") } };
            var userToAssociate = new User("users", "user2");
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new NotesRepository(unitOfWorkMock.Object);

            // Act
            repository.AddAssociatedUser(note, userToAssociate);

            // Assert
            unitOfWorkMock.Verify(uow => uow.Create(It.IsAny<NoteAssociatedUserTableEntry>(), "NoteAssociatedUsers"), Times.Once());
        }

        [TestMethod]
        public void NotesRepositoryRemoveAssociatedUserCallsDeleteFromTheUnitOfWork()
        {
            // Arrange
            var note = new Note("taskList1", "note1") { Owner = new User("users", "user1"), AssociatedUsers = { new User("users", "user1") } };
            var associatedUser = new User("users", "user2");
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new NotesRepository(unitOfWorkMock.Object);

            // Act
            repository.RemoveAssociatedUser(note, associatedUser);

            // Assert
            unitOfWorkMock.Verify(uow => uow.Delete(It.IsAny<NoteAssociatedUserTableEntry>()), Times.Once());
        }

        #endregion NotesRepository tests

        #region TaskListsRepository tests

        [TestMethod]
        public void TaskListsRepositoryLoadCallsLoadFromTheUnitOfWork()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Load<TaskListTableEntry>(It.Is<string>(s => s == "TaskLists"))).Returns(BuildTaskListsTable());
            var repository = new TaskListsRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.Load().ToList();

            // Assert
            Assert.IsTrue(result.Count() == 2);
            unitOfWorkMock.Verify(uow => uow.Load<TaskListTableEntry>("TaskLists"), Times.Once());
        }

        [TestMethod]
        public void TaskListsRepositoryGetCallsGetFromTheUnitOfWorkAndReturnsAExistentTaskList()
        {
            // Arrange
            var taskListTableEntry = new TaskListTableEntry("user1", "taskList1");
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Get<TaskListTableEntry>("TaskLists", "user1", "taskList1")).Returns(taskListTableEntry);
            var repository = new TaskListsRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.Get("user1", "taskList1");

            // Assert
            Assert.IsNotNull(result);
            unitOfWorkMock.Verify(uow => uow.Get<TaskListTableEntry>("TaskLists", "user1", "taskList1"), Times.Once());
        }

        [TestMethod]
        public void TaskListsRepositoryGetCallsGetFromTheUnitOfWorkAndReturnsNullForANonExistentTaskList()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new TaskListsRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.Get("user1", "taskList3");

            // Assert
            Assert.IsNull(result);
            unitOfWorkMock.Verify(uow => uow.Get<TaskListTableEntry>("TaskLists", "user1", "taskList3"), Times.Once());
        }

        [TestMethod]
        public void TaskListsRepositoryCreateCallsCreateFromTheUnitOfWork()
        {
            // Arrange
            var taskList = new TaskList("user1", "taskList1") { Owner = new User("users", "user1") };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new TaskListsRepository(unitOfWorkMock.Object);

            // Act
            repository.Create(taskList);

            // Assert
            unitOfWorkMock.Verify(uow => uow.Create(It.IsAny<TaskListTableEntry>(), "TaskLists"), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Create(It.IsAny<TaskListOwnerTableEntry>(), "TaskListOwner"), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Create(It.IsAny<TaskListAssociatedUserTableEntry>(), "TaskListAssociatedUsers"), Times.Once());
        }

        [TestMethod]
        public void TaskListsRepositoryUpdateCallsUpdateFromTheUnitOfWork()
        {
            // Arrange
            var taskList = new TaskList("user1", "taskList1") { Owner = new User("users", "user1") };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new TaskListsRepository(unitOfWorkMock.Object);

            // Act
            repository.Update(taskList);

            // Assert
            unitOfWorkMock.Verify(uow => uow.Update(It.IsAny<TaskListTableEntry>()), Times.Once());
        }

        [TestMethod]
        public void TaskListsRepositoryDeleteCallsDeleteFromTheUnitOfWork()
        {
            // Arrange
            var taskList = new TaskList("user1", "taskList1") { Owner = new User("users", "user1") };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Load<TaskListAssociatedUserTableEntry>(It.Is<string>(s => s == "TaskListAssociatedUsers"))).Returns(BuildTaskListAssociatedUsersTable());
            var repository = new TaskListsRepository(unitOfWorkMock.Object);

            // Act
            repository.Delete(taskList);

            // Assert
            unitOfWorkMock.Verify(uow => uow.Delete(It.IsAny<TaskListTableEntry>()), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Delete(It.IsAny<TaskListAssociatedUserTableEntry>()), Times.Exactly(2));
        }

        [TestMethod]
        public void TaskListsRepositoryAddAssociatedUserCallsCreateFromTheUnitOfWork()
        {
            // Arrange
            var taskList = new TaskList("user1", "taskList1") { Owner = new User("users", "user1"), AssociatedUsers = { new User("users", "user1") } };
            var userToAssociate = new User("users", "user2");
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new TaskListsRepository(unitOfWorkMock.Object);

            // Act
            repository.AddAssociatedUser(taskList, userToAssociate);

            // Assert
            unitOfWorkMock.Verify(uow => uow.Create(It.IsAny<TaskListAssociatedUserTableEntry>(), "TaskListAssociatedUsers"), Times.Once());
        }

        [TestMethod]
        public void TaskListsRepositoryRemoveAssociatedUserCallsDeleteFromTheUnitOfWork()
        {
            // Arrange
            var taskList = new TaskList("user1", "taskList1") { Owner = new User("users", "user1"), AssociatedUsers = { new User("users", "user1") } };
            var userToAssociate = new User("users", "user2");
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new TaskListsRepository(unitOfWorkMock.Object);

            // Act
            repository.RemoveAssociatedUser(taskList, userToAssociate);

            // Assert
            unitOfWorkMock.Verify(uow => uow.Delete(It.IsAny<TaskListAssociatedUserTableEntry>()), Times.Once());
        }

        [TestMethod]
        public void TaskListsRepositoryGetTaskListsAssociatedByUserCallsLoadFromTheUnitOfWork()
        {
            // Arrange
            var user = new User("users", "user3");
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Load<TaskListAssociatedUserTableEntry>(It.Is<string>(s => s == "TaskListAssociatedUsers"))).Returns(BuildTaskListAssociatedUsersTable());
            unitOfWorkMock.Setup(u => u.Load<TaskListTableEntry>(It.Is<string>(s => s == "TaskLists"))).Returns(BuildTaskListsTable());
            var repository = new TaskListsRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.GetTaskListsAssociatedByUser(user);

            // Assert
            Assert.IsTrue(result.Count() == 2);
            unitOfWorkMock.Verify(uow => uow.Load<TaskListAssociatedUserTableEntry>("TaskListAssociatedUsers"), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Load<TaskListTableEntry>("TaskLists"), Times.Exactly(2));
        }

        #endregion TaskListsRepository tests

        #region UsersRepository tests

        [TestMethod]
        public void UsersRepositoryLoadCallsLoadFromTheUnitOfWork()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Load<UserTableEntry>(It.Is<string>(s => s == "Users"))).Returns(BuildUsersTable());
            var repository = new UsersRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.Load().ToList();

            // Assert
            Assert.IsTrue(result.Count() == 2);
            unitOfWorkMock.Verify(uow => uow.Load<UserTableEntry>("Users"), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryGetCallsGetFromTheUnitOfWorkAndReturnsAExistentUser()
        {
            // Arrange
            var noteTableEntry = new UserTableEntry("users", "user1");
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Get<UserTableEntry>("Users", "users", "user1")).Returns(noteTableEntry);
            var repository = new UsersRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.Get("users", "user1");

            // Assert
            Assert.IsNotNull(result);
            unitOfWorkMock.Verify(uow => uow.Get<UserTableEntry>("Users", "users", "user1"), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryGetCallsGetFromTheUnitOfWorkAndReturnsNullForANonExistentUser()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.Get("users", "user3");

            // Assert
            Assert.IsNull(result);
            unitOfWorkMock.Verify(uow => uow.Get<UserTableEntry>("Users", "users", "user3"), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryCreateCallsCreateFromTheUnitOfWork()
        {
            // Arrange
            var user = new User("users", "user1");
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersRepository(unitOfWorkMock.Object);

            // Act
            repository.Create(user);

            // Assert
            unitOfWorkMock.Verify(uow => uow.Create(It.IsAny<UserTableEntry>(), "Users"), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryUpdateCallsUpdateFromTheUnitOfWork()
        {
            // Arrange
            var user = new User("users", "user1");
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersRepository(unitOfWorkMock.Object);

            // Act
            repository.Update(user);

            // Assert
            unitOfWorkMock.Verify(uow => uow.Update(It.IsAny<UserTableEntry>()), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryDeleteCallsDeleteFromTheUnitOfWork()
        {
            // Arrange
            var user = new User("users", "user1");
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersRepository(unitOfWorkMock.Object);

            // Act
            repository.Delete(user);

            // Assert
            unitOfWorkMock.Verify(uow => uow.Delete(It.IsAny<UserTableEntry>()), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryGetOrAddCurrentUserCallsCreateFromTheUnitOfWorkForANewUser()
        {
            // Arrange
            var claims = new[]
                                 {
                                     new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "identityProvider") ,
                                     new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "nameIdentifier"),
                                     new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", "test@test.com")
                                 };

            IClaimsIdentity identity = new ClaimsIdentity(claims);
            IClaimsPrincipal principal = new ClaimsPrincipal(new[] { identity });
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersRepository(unitOfWorkMock.Object);
            unitOfWorkMock.Setup(u => u.Get<UserTableEntry>(It.Is<string>(s => s == "Users"), It.Is<string>(s => s == "users"), It.Is<string>(s => s == "nameIdentifier_identityProvider"))).Returns((UserTableEntry)null);

            // Act
            var result = repository.GetOrAddCurrentUser(principal);

            // Assert
            Assert.IsNotNull(result);
            unitOfWorkMock.Verify(uow => uow.Get<UserTableEntry>(It.Is<string>(s => s == "Users"), It.Is<string>(s => s == "users"), It.Is<string>(s => s == "nameIdentifier_identityProvider")), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Create(It.IsAny<UserTableEntry>(), "Users"), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryGetOrAddCurrentUserCallsCreateFromTheUnitOfWorkForAExistingUser()
        {
            // Arrange
            var userTableEntry = new UserTableEntry("users", "user1");
            var claims = new[]
                                 {
                                     new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "identityProvider") ,
                                     new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "nameIdentifier"),
                                     new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", "test@test.com")
                                 };

            IClaimsIdentity identity = new ClaimsIdentity(claims);
            IClaimsPrincipal principal = new ClaimsPrincipal(new[] { identity });
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Get<UserTableEntry>("Users", "users", "user1")).Returns(userTableEntry);
            var repository = new UsersRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.GetOrAddCurrentUser(principal);

            // Assert
            Assert.IsNotNull(result);
            unitOfWorkMock.Verify(uow => uow.Get<UserTableEntry>(It.Is<string>(s => s == "Users"), It.Is<string>(s => s == "users"), It.Is<string>(s => s == "nameIdentifier_identityProvider")), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryLoadNoteOwnerCallsLoadAndGetFromTheUnitOfWork()
        {
            // Arrange
            var note = new Note("taskList1", "note1");
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Load<NoteOwnerTableEntry>(It.Is<string>(s => s == "NoteOwner"))).Returns(BuildNoteOwnerTable());
            unitOfWorkMock.Setup(u => u.Get<User>(It.Is<string>(s => s == "Users"), It.Is<string>(s => s == "users"), It.Is<string>(s => s == "user1"))).Returns(new User("users", "user1"));
            var repository = new UsersRepository(unitOfWorkMock.Object);

            // Act
            repository.LoadNoteOwner(note);

            // Assert
            Assert.IsNotNull(note.Owner);
            unitOfWorkMock.Verify(uow => uow.Load<NoteOwnerTableEntry>("NoteOwner"), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Get<User>("Users", "users", "user1"), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryLoadTaskListOwnerCallsLoadAndGetFromTheUnitOfWork()
        {
            // Arrange
            var taskList = new TaskList("user1", "taskList1");
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Load<TaskListOwnerTableEntry>(It.Is<string>(s => s == "TaskListOwner"))).Returns(BuildTaskListOwnerTable());
            unitOfWorkMock.Setup(u => u.Get<User>(It.Is<string>(s => s == "Users"), It.Is<string>(s => s == "users"), It.Is<string>(s => s == "user1"))).Returns(new User("users", "user1"));
            var repository = new UsersRepository(unitOfWorkMock.Object);

            // Act
            repository.LoadTaskListOwner(taskList);

            // Assert
            Assert.IsNotNull(taskList.Owner);
            unitOfWorkMock.Verify(uow => uow.Load<TaskListOwnerTableEntry>("TaskListOwner"), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Get<User>("Users", "users", "user1"), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryLoadNoteAssociatedUsersCallsLoadAndGetFromTheUnitOfWork()
        {
            // Arrange
            var note = new Note("taskList1", "note1");
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Load<NoteAssociatedUserTableEntry>(It.Is<string>(s => s == "NoteAssociatedUsers"))).Returns(BuildNoteAssociatedUsersTable);
            unitOfWorkMock.Setup(u => u.Get<User>(It.Is<string>(s => s == "Users"), It.Is<string>(s => s == "users"), It.Is<string>(s => s == "user1"))).Returns(new User("users", "user1"));
            unitOfWorkMock.Setup(u => u.Get<User>(It.Is<string>(s => s == "Users"), It.Is<string>(s => s == "users"), It.Is<string>(s => s == "user3"))).Returns(new User("users", "user3"));
            var repository = new UsersRepository(unitOfWorkMock.Object);

            // Act
            repository.LoadNoteAssociatedUsers(note);

            // Assert
            Assert.IsTrue(note.AssociatedUsers.Count == 2);
            unitOfWorkMock.Verify(uow => uow.Load<NoteAssociatedUserTableEntry>("NoteAssociatedUsers"), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Get<User>("Users", "users", "user1"), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Get<User>("Users", "users", "user3"), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryLoadTaskListAssociatedUsersCallsLoadAndGetFromTheUnitOfWork()
        {
            // Arrange
            var taskList = new TaskList("user1", "taskList1");
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Load<TaskListAssociatedUserTableEntry>(It.Is<string>(s => s == "TaskListAssociatedUsers"))).Returns(BuildTaskListAssociatedUsersTable);
            unitOfWorkMock.Setup(u => u.Get<User>(It.Is<string>(s => s == "Users"), It.Is<string>(s => s == "users"), It.Is<string>(s => s == "user1"))).Returns(new User("users", "user1"));
            unitOfWorkMock.Setup(u => u.Get<User>(It.Is<string>(s => s == "Users"), It.Is<string>(s => s == "users"), It.Is<string>(s => s == "user3"))).Returns(new User("users", "user3"));
            var repository = new UsersRepository(unitOfWorkMock.Object);

            // Act
            repository.LoadTaskListAssociatedUsers(taskList);
            // Assert
            Assert.IsTrue(taskList.AssociatedUsers.Count == 2);
            unitOfWorkMock.Verify(uow => uow.Load<TaskListAssociatedUserTableEntry>("TaskListAssociatedUsers"), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Get<User>("Users", "users", "user1"), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Get<User>("Users", "users", "user3"), Times.Once());
        }

        #endregion UsersRepository tests

        #region Private methods

        private IQueryable<NoteTableEntry> BuildNotesTable()
        {
            var notesTable = new List<NoteTableEntry>
                                 {
                                     new NoteTableEntry("taskList1", "note1"), 
                                     new NoteTableEntry("taskList2", "note2")
                                 };

            return notesTable.AsQueryable();
        }

        private IQueryable<TaskListTableEntry> BuildTaskListsTable()
        {
            var taskListsTable = new List<TaskListTableEntry>
                                     {
                                         new TaskListTableEntry("user1", "taskList1"),
                                         new TaskListTableEntry("user2", "taskList2")
                                     };

            return taskListsTable.AsQueryable();
        }

        private IQueryable<UserTableEntry> BuildUsersTable()
        {
            var usersTable = new List<UserTableEntry>
                                 {
                                     new UserTableEntry("users", "user1"), 
                                     new UserTableEntry("users", "user2")
                                 };

            return usersTable.AsQueryable();
        }

        private IQueryable<NoteOwnerTableEntry> BuildNoteOwnerTable()
        {
            var noteOwner = new List<NoteOwnerTableEntry>
                                      {
                                          new NoteOwnerTableEntry("user1", "note1"),
                                          new NoteOwnerTableEntry("user2", "note2")
                                      };

            return noteOwner.AsQueryable();
        }

        private IQueryable<NoteAssociatedUserTableEntry> BuildNoteAssociatedUsersTable()
        {
            var associatedUsers = new List<NoteAssociatedUserTableEntry>
                                      {
                                          new NoteAssociatedUserTableEntry("note1", "user1"),
                                          new NoteAssociatedUserTableEntry("note1", "user3"),
                                          new NoteAssociatedUserTableEntry("note2", "user2"),
                                          new NoteAssociatedUserTableEntry("note2", "user4")
                                      };

            return associatedUsers.AsQueryable();
        }

        private IQueryable<TaskListAssociatedUserTableEntry> BuildTaskListAssociatedUsersTable()
        {
            var associatedUsers = new List<TaskListAssociatedUserTableEntry>
                                      {
                                          new TaskListAssociatedUserTableEntry("taskList1", "user1"),
                                          new TaskListAssociatedUserTableEntry("taskList1", "user3"),
                                          new TaskListAssociatedUserTableEntry("taskList2", "user2"),
                                          new TaskListAssociatedUserTableEntry("taskList2", "user3")
                                      };

            return associatedUsers.AsQueryable();
        }

        #endregion Private methods
    }
}