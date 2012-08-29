using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        #region Fields

        private const string User1PartitionKey = "WindowsLiveID";
        private const string User2PartitionKey = "WindowsLiveID";
        private const string User3PartitionKey = "WindowsLiveID";
        private const string User1RowKey = "user1-WindowsLiveID";
        private const string User2RowKey = "user2-WindowsLiveID";
        private const string User3RowKey = "user3-WindowsLiveID";
        private const string Note1PartitionKey = "user1-WindowsLiveID";
        private const string Note2PartitionKey = "user2-WindowsLiveID";
        private const string Note3PartitionKey = "user3-WindowsLiveID";
        private readonly string _note1RowKey = ShortGuid.NewGuid().ToString();
        private readonly string _note2RowKey = ShortGuid.NewGuid().ToString();
        private readonly string _note3RowKey = ShortGuid.NewGuid().ToString();
        private const string TaskList1PartitionKey = "user1-WindowsLiveID";
        private const string TaskList2PartitionKey = "user2-WindowsLiveID";
        private const string TaskList3PartitionKey = "user3-WindowsLiveID";
        private readonly string _taskList1RowKey = ShortGuid.NewGuid().ToString();
        private readonly string _taskList2RowKey = ShortGuid.NewGuid().ToString();
        private readonly string _taskList3RowKey = ShortGuid.NewGuid().ToString();

        #endregion Fields

        #region NotesRepository tests

        [TestMethod]
        public void NotesRepositoryLoadCallsLoadFromTheUnitOfWork()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Load<NoteEntity>(It.Is<string>(s => s == "Notes"))).Returns(BuildNotesTable());
            var repository = new NotesRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.Load().ToList();

            // Assert
            Assert.IsTrue(result.Count() == 2);
            unitOfWorkMock.Verify(uow => uow.Load<NoteEntity>("Notes"), Times.Once());
        }

        [TestMethod]
        public void NotesRepositoryGetWihtFilterCallsGetFromTheUnitOfWorkAndReturnsAnExistingNote()
        {
            // Arrange
            var user = new User { PartitionKey = User1PartitionKey, RowKey = User1RowKey };
            var taskList = new TaskList("Test title", user) { PartitionKey = TaskList1PartitionKey, RowKey = _taskList1RowKey };
            var note = new Note("Test title", "Test content", user, taskList);
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Get("Notes", It.IsAny<Expression<Func<Note, bool>>>())).Returns(note);
            var repository = new NotesRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.Get(n => n.PartitionKey == Note1PartitionKey && n.RowKey == _note1RowKey);

            // Assert
            Assert.IsNotNull(result);
            unitOfWorkMock.Verify(uow => uow.Get("Notes", It.IsAny<Expression<Func<Note, bool>>>()), Times.Once());
        }

        [TestMethod]
        public void NotesRepositoryGetCallsGetFromTheUnitOfWorkAndReturnsAnExistingNote()
        {
            // Arrange
            var note = new NoteEntity(Note1PartitionKey, _note1RowKey);
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Get("Notes", It.IsAny<Expression<Func<NoteEntity, bool>>>())).Returns(note);
            var repository = new NotesRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.Get(Note1PartitionKey, _note1RowKey);

            // Assert
            Assert.IsNotNull(result);
            unitOfWorkMock.Verify(uow => uow.Get("Notes", It.IsAny<Expression<Func<NoteEntity, bool>>>()), Times.Once());
        }

        [TestMethod]
        public void NotesRepositoryGetCallsGetFromTheUnitOfWorkAndReturnsNullForANonExistingNote()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new NotesRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.Get(Note3PartitionKey, _note3RowKey);

            // Assert
            Assert.IsNull(result);
            unitOfWorkMock.Verify(uow => uow.Get("Notes", It.IsAny<Expression<Func<NoteEntity, bool>>>()), Times.Once());
        }

        [TestMethod]
        public void NotesRepositoryCreateCallsCreateFromTheUnitOfWork()
        {
            // Arrange
            var user = new User { PartitionKey = User1PartitionKey, RowKey = User1RowKey };
            var taskList = new TaskList("Test title", user) { PartitionKey = TaskList1PartitionKey, RowKey = _taskList1RowKey };
            var note = new Note("Test title", "Test content", user, taskList);
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Load("Notes", It.IsAny<Expression<Func<NoteEntity, bool>>>())).Returns(BuildNotesTable());
            var repository = new NotesRepository(unitOfWorkMock.Object);

            // Act
            repository.Create(note);

            // Assert
            Assert.IsTrue(note.PartitionKey != string.Empty);
            Assert.IsTrue(note.RowKey != string.Empty);
            unitOfWorkMock.Verify(uow => uow.Create(It.IsAny<NoteEntity>(), "Notes"), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Create(It.IsAny<NoteShareEntity>(), "NoteShares"), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Create(It.IsAny<TaskListNoteEntity>(), "TaskListNotes"), Times.Once());
        }

        [TestMethod]
        public void NotesRepositoryUpdateCallsUpdateFromTheUnitOfWork()
        {
            // Arrange
            var user = new User { PartitionKey = User1PartitionKey, RowKey = User1RowKey };
            var taskList = new TaskList("Test title", user) { PartitionKey = TaskList1PartitionKey, RowKey = _taskList1RowKey };
            var note = new Note("Test title", "Test content", user, taskList);
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new NotesRepository(unitOfWorkMock.Object);

            // Act
            repository.Update(note);

            // Assert
            unitOfWorkMock.Verify(uow => uow.Update("Notes", It.IsAny<NoteEntity>()), Times.Once());
        }

        [TestMethod]
        public void NotesRepositoryDeleteCallsDeletesFromTheUnitOfWork()
        {
            // Arrange
            var user = new User { PartitionKey = User1PartitionKey, RowKey = User1RowKey };
            var taskList = new TaskList("Test title", user) { PartitionKey = TaskList1PartitionKey, RowKey = _taskList1RowKey };
            var note = new Note("Test title", "Test content", user, taskList) { PartitionKey = Note1PartitionKey, RowKey = _note1RowKey };
            var taskListNote = new TaskListEntity(string.Format("{0}+{1}", TaskList1PartitionKey, _taskList1RowKey), string.Format("{0}+{1}", Note1PartitionKey, _note1RowKey));

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.Get("TaskListNotes", It.IsAny<Expression<Func<TaskListEntity, bool>>>())).Returns(taskListNote);
            unitOfWorkMock.Setup(uow => uow.Load<NoteShareEntity>(It.Is<string>(s => s == "NoteShares"))).Returns(BuildNoteSharesTable());
            var repository = new NotesRepository(unitOfWorkMock.Object);

            // Act
            repository.Delete(note);

            // Assert
            unitOfWorkMock.Verify(uow => uow.Delete<NoteEntity>("Notes", Note1PartitionKey, _note1RowKey), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Delete<TaskListEntity>("TaskListNotes", taskListNote.PartitionKey, taskListNote.RowKey), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Delete<NoteShareEntity>("NoteShares", string.Format("{0}+{1}", Note1PartitionKey, _note1RowKey), User1RowKey), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Delete<NoteShareEntity>("NoteShares", string.Format("{0}+{1}", Note1PartitionKey, _note1RowKey), User3RowKey), Times.Once());
        }

        [TestMethod]
        public void NotesRepositoryAddShareCallsCreateFromTheUnitOfWork()
        {
            // Arrange
            var user = new User { PartitionKey = User1PartitionKey, RowKey = User1RowKey };
            var taskList = new TaskList("Test title", user) { PartitionKey = TaskList1PartitionKey, RowKey = _taskList1RowKey };
            var note = new Note("Test title", "Test content", user, taskList);
            note.Share.Add(user);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new NotesRepository(unitOfWorkMock.Object);

            // Act
            repository.AddShare(note, string.Format("{0}+{1}", User1PartitionKey, User1RowKey));

            // Assert
            unitOfWorkMock.Verify(uow => uow.Create(It.IsAny<NoteShareEntity>(), "NoteShares"), Times.Once());
        }

        [TestMethod]
        public void NotesRepositoryRemoveShareCallsDeleteFromTheUnitOfWork()
        {
            // Arrange
            var user = new User { PartitionKey = User1PartitionKey, RowKey = User1RowKey };
            var taskList = new TaskList("Test title", user) { PartitionKey = TaskList1PartitionKey, RowKey = _taskList1RowKey };
            var note = new Note("Test title", "Test content", user, taskList) { PartitionKey = Note1PartitionKey, RowKey = _note1RowKey };
            note.Share.Add(user);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new NotesRepository(unitOfWorkMock.Object);

            // Act
            repository.RemoveShare(note, User1RowKey);

            // Assert
            unitOfWorkMock.Verify(uow => uow.Delete<NoteShareEntity>("NoteShares", string.Format("{0}+{1}", Note1PartitionKey, _note1RowKey), User1RowKey), Times.Once());
        }

        [TestMethod]
        public void NotesRepositoryLoadNotesCallsLoadAndGetsFromTheUnitOfWork()
        {
            // Arrange
            var user = new User { PartitionKey = User1PartitionKey, RowKey = User1RowKey };
            var taskList = new TaskList("Test title", user) { PartitionKey = User1RowKey, RowKey = _taskList1RowKey };
            var note = new Note("Test title", "Test content", user, taskList) { PartitionKey = Note1PartitionKey, RowKey = _note1RowKey };
            var noteEntity = new NoteEntity(Note1PartitionKey, _note1RowKey);
            note.Share.Add(user);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Load("TaskListNotes", It.IsAny<Expression<Func<TaskListNoteEntity, bool>>>())).Returns(BuildTaskListNotesTable());
            unitOfWorkMock.Setup(u => u.Get("Notes", It.IsAny<Expression<Func<NoteEntity, bool>>>())).Returns(noteEntity);
            var repository = new NotesRepository(unitOfWorkMock.Object);

            // Act
            repository.LoadNotes(taskList);

            // Assert
            unitOfWorkMock.Verify(uow => uow.Load("TaskListNotes", It.IsAny<Expression<Func<TaskListNoteEntity, bool>>>()), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Get("Notes", It.IsAny<Expression<Func<NoteEntity, bool>>>()), Times.Exactly(2));
        }

        [TestMethod]
        public void NotesRepositoryHasPermisionToEditCallsGetFromTheUnitOfWork()
        {
            // Arrange
            var user = new User { PartitionKey = User1PartitionKey, RowKey = User1RowKey };
            var taskList = new TaskList("Test title", user) { PartitionKey = TaskList1PartitionKey, RowKey = _taskList1RowKey };
            var note = new Note("Test title", "Test content", user, taskList) { PartitionKey = Note1PartitionKey, RowKey = _note1RowKey };
            var noteShare = new NoteShareEntity(string.Format("{0}+{1}", Note1PartitionKey, _note1RowKey), User1RowKey);
            note.Share.Add(user);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Get("NoteShares", It.IsAny<Expression<Func<NoteShareEntity, bool>>>())).Returns(noteShare);
            var repository = new NotesRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.HasPermissionToEdit(user, note);

            // Assert
            Assert.IsTrue(result);
            unitOfWorkMock.Verify(uow => uow.Get("NoteShares", It.IsAny<Expression<Func<NoteShareEntity, bool>>>()), Times.Once());
        }

        [TestMethod]
        public void NotesRepositoryNoteWithTitleExistsCallsGetFromTheUnitOfWork()
        {
            // Arrange
            var user = new User { PartitionKey = User1PartitionKey, RowKey = User1RowKey };
            var taskList = new TaskList("Test title", user) { PartitionKey = User1RowKey, RowKey = _taskList1RowKey };
            var note = new Note("Test title", "Test content", user, taskList) { PartitionKey = Note1PartitionKey, RowKey = _note1RowKey };
            note.Share.Add(user);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Get("Notes", It.IsAny<Expression<Func<Note, bool>>>())).Returns(note);
            var repository = new NotesRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.NoteWithTitleExists("Test title", taskList);

            // Assert
            Assert.IsTrue(result);
            unitOfWorkMock.Verify(uow => uow.Get("Notes", It.IsAny<Expression<Func<Note, bool>>>()), Times.Once());
        }

        #endregion NotesRepository tests

        #region TaskListsRepository tests

        [TestMethod]
        public void TaskListsRepositoryLoadCallsLoadFromTheUnitOfWork()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Load<TaskListEntity>(It.Is<string>(s => s == "TaskLists"))).Returns(BuildTaskListsTable());
            var repository = new TaskListsRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.Load().ToList();

            // Assert
            Assert.IsTrue(result.Count() == 2);
            unitOfWorkMock.Verify(uow => uow.Load<TaskListEntity>("TaskLists"), Times.Once());
        }

        [TestMethod]
        public void TaskListsRepositoryGetCallsWithFilterGetFromTheUnitOfWorkAndReturnsAExistingTaskList()
        {
            // Arrange
            var user = new User { PartitionKey = User1PartitionKey, RowKey = User1RowKey };
            var taskList = new TaskList("Test title", user) { PartitionKey = TaskList1PartitionKey, RowKey = _taskList1RowKey };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Get("TaskLists", It.IsAny<Expression<Func<TaskList, bool>>>())).Returns(taskList);
            var repository = new TaskListsRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.Get(n => n.PartitionKey == Note1PartitionKey && n.RowKey == _note1RowKey);

            // Assert
            Assert.IsNotNull(result);
            unitOfWorkMock.Verify(uow => uow.Get("TaskLists", It.IsAny<Expression<Func<TaskList, bool>>>()), Times.Once());
        }

        [TestMethod]
        public void TaskListRepositoryGetCallsGetFromTheUnitOfWorkAndReturnsAnExistingTaskList()
        {
            // Arrange
            var taskList = new TaskListEntity(TaskList1PartitionKey, _taskList1RowKey);
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Get("TaskLists", It.IsAny<Expression<Func<TaskListEntity, bool>>>())).Returns(taskList);
            var repository = new TaskListsRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.Get(Note1PartitionKey, _note1RowKey);

            // Assert
            Assert.IsNotNull(result);
            unitOfWorkMock.Verify(uow => uow.Get("TaskLists", It.IsAny<Expression<Func<TaskListEntity, bool>>>()), Times.Once());
        }

        [TestMethod]
        public void TaskListsRepositoryGetCallsGetFromTheUnitOfWorkAndReturnsNullForANonExistingTaskList()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new TaskListsRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.Get(TaskList3PartitionKey, _taskList3RowKey);

            // Assert
            Assert.IsNull(result);
            unitOfWorkMock.Verify(uow => uow.Get("TaskLists", It.IsAny<Expression<Func<TaskListEntity, bool>>>()), Times.Once());
        }

        [TestMethod]
        public void TaskListsRepositoryGetSharedCallsLoadFromTheUnitOfWork()
        {
            // Arrange
            var user = new User { PartitionKey = User1PartitionKey, RowKey = User1RowKey };
            var taskList = new TaskListEntity(TaskList1PartitionKey, _taskList1RowKey);
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Load("TaskListShares", It.IsAny<Expression<Func<TaskListShareEntity, bool>>>())).Returns(BuildTaskListSharesTable());
            unitOfWorkMock.Setup(u => u.Get("TaskLists", It.IsAny<Expression<Func<TaskListEntity, bool>>>())).Returns(taskList);
            var repository = new TaskListsRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.GetShared(user);

            // Assert
            Assert.IsTrue(result.Count() == 2);
            unitOfWorkMock.Verify(uow => uow.Load("TaskListShares", It.IsAny<Expression<Func<TaskListShareEntity, bool>>>()), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Get("TaskLists", It.IsAny<Expression<Func<TaskListEntity, bool>>>()), Times.Exactly(2));
        }

        [TestMethod]
        public void TaskListRepositoryLoadContainerCallsGetsFromTheUnitOfWork()
        {
            // Arrange
            var taskList = new TaskListEntity(TaskList1PartitionKey, _taskList1RowKey);
            var taskListNote = new TaskListEntity { PartitionKey = string.Format("{0}+{1}", TaskList1PartitionKey, _taskList1RowKey),
                                                    RowKey = string.Format("{0}+{1}", Note1PartitionKey, _note1RowKey) };
            var note = new Note { PartitionKey = Note1PartitionKey, RowKey = _note1RowKey };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Get("TaskListNotes", It.IsAny<Expression<Func<TaskListEntity, bool>>>())).Returns(taskListNote);
            unitOfWorkMock.Setup(u => u.Get("TaskLists", It.IsAny<Expression<Func<TaskListEntity, bool>>>())).Returns(taskList);
            var repository = new TaskListsRepository(unitOfWorkMock.Object);

            // Act
            repository.LoadContainer(note);

            // Assert
            Assert.IsNotNull(note.Container);
            unitOfWorkMock.Verify(uow => uow.Get("TaskListNotes", It.IsAny<Expression<Func<TaskListEntity, bool>>>()), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Get("TaskLists", It.IsAny<Expression<Func<TaskListEntity, bool>>>()), Times.Once());
        }

        [TestMethod]
        public void TaskListsRepositoryCreateCallsCreatesFromTheUnitOfWork()
        {
            // Arrange
            var user = new User { PartitionKey = User1PartitionKey, RowKey = User1RowKey };
            var taskList = new TaskList("Test title", user);
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new TaskListsRepository(unitOfWorkMock.Object);

            // Act
            repository.Create(taskList);

            // Assert
            Assert.IsTrue(taskList.PartitionKey != string.Empty);
            Assert.IsTrue(taskList.RowKey != string.Empty);
            unitOfWorkMock.Verify(uow => uow.Create(It.IsAny<TaskListEntity>(), "TaskLists"), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Create(It.IsAny<TaskListShareEntity>(), "TaskListShares"), Times.Once());
        }

        [TestMethod]
        public void TaskListsRepositoryUpdateCallsUpdateFromTheUnitOfWork()
        {
            // Arrange
            var user = new User { PartitionKey = User1PartitionKey, RowKey = User1RowKey };
            var taskList = new TaskList("Test title", user) { PartitionKey = TaskList1PartitionKey, RowKey = _taskList1RowKey };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new TaskListsRepository(unitOfWorkMock.Object);

            // Act
            repository.Update(taskList);

            // Assert
            unitOfWorkMock.Verify(uow => uow.Update("TaskLists", It.IsAny<TaskListEntity>()), Times.Once());
        }

        [TestMethod]
        public void TaskListsRepositoryDeleteCallsDeleteFromTheUnitOfWork()
        {
            // Arrange
            var user = new User { PartitionKey = User1PartitionKey, RowKey = User1RowKey };
            var taskList = new TaskList("Test title", user) { PartitionKey = TaskList1PartitionKey, RowKey = _taskList1RowKey };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(uow => uow.Load("TaskListShares", It.IsAny<Expression<Func<TaskListShareEntity, bool>>>())).Returns(BuildTaskListSharesTable());
            var repository = new TaskListsRepository(unitOfWorkMock.Object);

            // Act
            repository.Delete(taskList);

            // Assert
            unitOfWorkMock.Verify(uow => uow.Delete<TaskListEntity>("TaskLists", TaskList1PartitionKey, _taskList1RowKey), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Delete<TaskListShareEntity>("TaskListShares", string.Format("{0}+{1}", TaskList1PartitionKey, _taskList1RowKey), User1RowKey), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Delete<TaskListShareEntity>("TaskListShares", string.Format("{0}+{1}", TaskList1PartitionKey, _taskList1RowKey), User3RowKey), Times.Once());
        }

        [TestMethod]
        public void TaskListsRepositoryAddShareCallsCreateFromTheUnitOfWork()
        {
            // Arrange
            var user = new User { PartitionKey = User1PartitionKey, RowKey = User1RowKey };
            var taskList = new TaskList("Test title", user) { PartitionKey = TaskList1PartitionKey, RowKey = _taskList1RowKey };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new TaskListsRepository(unitOfWorkMock.Object);

            // Act
            repository.AddShare(taskList, user.RowKey);

            // Assert
            unitOfWorkMock.Verify(uow => uow.Create(It.IsAny<TaskListShareEntity>(), "TaskListShares"), Times.Once());
        }

        [TestMethod]
        public void TaskListsRepositoryRemoveShareCallsDeleteFromTheUnitOfWork()
        {
            // Arrange
            var user = new User { PartitionKey = User1PartitionKey, RowKey = User1RowKey };
            var taskList = new TaskList("Test title", user) { PartitionKey = TaskList1PartitionKey, RowKey = _taskList1RowKey };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new TaskListsRepository(unitOfWorkMock.Object);

            // Act
            repository.RemoveShare(taskList, user.RowKey);

            // Assert
            unitOfWorkMock.Verify(uow => uow.Delete<TaskListShareEntity>("TaskListShares", string.Format("{0}+{1}", TaskList1PartitionKey, _taskList1RowKey), User1RowKey), Times.Once());
        }

        [TestMethod]
        public void TaskListsRepositoryHasPermisionToEditCallsGetFromTheUnitOfWork()
        {
            // Arrange
            var user = new User { PartitionKey = User1PartitionKey, RowKey = User1RowKey };
            var taskList = new TaskList("Test title", user) { PartitionKey = TaskList1PartitionKey, RowKey = _taskList1RowKey };
            var taskListShare = new TaskListShareEntity(string.Format("{0}+{1}", TaskList1PartitionKey, _taskList1RowKey), User1RowKey);
            taskList.Share.Add(user);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Get("TaskListShares", It.IsAny<Expression<Func<TaskListShareEntity, bool>>>())).Returns(taskListShare);
            var repository = new TaskListsRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.HasPermissionToEdit(user, taskList);

            // Assert
            Assert.IsTrue(result);
            unitOfWorkMock.Verify(uow => uow.Get("TaskListShares", It.IsAny<Expression<Func<TaskListShareEntity, bool>>>()), Times.Once());
        }

        #endregion TaskListsRepository tests

        #region UsersRepository tests

        [TestMethod]
        public void UsersRepositoryLoadCallsLoadFromTheUnitOfWork()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Load<UserEntity>(It.Is<string>(s => s == "Users"))).Returns(BuildUsersTable());
            var repository = new UsersRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.Load().ToList();

            // Assert
            Assert.IsTrue(result.Count() == 2);
            unitOfWorkMock.Verify(uow => uow.Load<UserEntity>("Users"), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryGetCallsWithFilterGetFromTheUnitOfWorkAndReturnsAExistingTaskList()
        {
            // Arrange
            var user = new User { PartitionKey = User1PartitionKey, RowKey = User1RowKey };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Get("Users", It.IsAny<Expression<Func<User, bool>>>())).Returns(user);
            var repository = new UsersRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.Get(n => n.PartitionKey == User1PartitionKey && n.RowKey == User1RowKey);

            // Assert
            Assert.IsNotNull(result);
            unitOfWorkMock.Verify(uow => uow.Get("Users", It.IsAny<Expression<Func<User, bool>>>()), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryGetCallsGetFromTheUnitOfWorkAndReturnsAExistingUser()
        {
            // Arrange
            var user = new UserEntity { PartitionKey = User1PartitionKey, RowKey = User1RowKey };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Get<UserEntity>("Users", User1PartitionKey, User1RowKey)).Returns(user);
            var repository = new UsersRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.Get(User1PartitionKey, User1RowKey);

            // Assert
            Assert.IsNotNull(result);
            unitOfWorkMock.Verify(uow => uow.Get<UserEntity>("Users", User1PartitionKey, User1RowKey), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryGetCallsGetFromTheUnitOfWorkAndReturnsNullForANonExistingUser()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.Get(User3PartitionKey, User3RowKey);

            // Assert
            Assert.IsNull(result);
            unitOfWorkMock.Verify(uow => uow.Get<UserEntity>("Users", User3PartitionKey, User3RowKey), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryCreateCallsCreateFromTheUnitOfWork()
        {
            // Arrange
            var user = new User { PartitionKey = User1PartitionKey, RowKey = User1RowKey };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersRepository(unitOfWorkMock.Object);

            // Act
            repository.Create(user);

            // Assert
            Assert.IsTrue(user.PartitionKey != string.Empty);
            Assert.IsTrue(user.RowKey != string.Empty);
            unitOfWorkMock.Verify(uow => uow.Create(It.IsAny<UserEntity>(), "Users"), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryUpdateCallsUpdateFromTheUnitOfWork()
        {
            // Arrange
            var user = new User { PartitionKey = User1PartitionKey, RowKey = User1RowKey };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersRepository(unitOfWorkMock.Object);

            // Act
            repository.Update(user);

            // Assert
            unitOfWorkMock.Verify(uow => uow.Update("Users", It.IsAny<UserEntity>()), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryDeleteCallsDeleteFromTheUnitOfWork()
        {
            // Arrange
            var user = new User { PartitionKey = User1PartitionKey, RowKey = User1RowKey };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersRepository(unitOfWorkMock.Object);

            // Act
            repository.Delete(user);

            // Assert
            unitOfWorkMock.Verify(uow => uow.Delete<UserEntity>("Users", User1PartitionKey, User1RowKey), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryUserIsRegisteredCallsLoadFromTheUnitOfWorkForANewUser()
        {
            // Arrange
            var claims = new[]
                                     {
                                         new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "WindowsLiveID") ,
                                         new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "nameIdentifier"),
                                         new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", "test@test.com")
                                     };

            IClaimsIdentity identity = new ClaimsIdentity(claims);
            IClaimsPrincipal principal = new ClaimsPrincipal(new[] { identity });
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersRepository(unitOfWorkMock.Object);
            unitOfWorkMock.Setup(uow => uow.Load("Users", It.IsAny<Expression<Func<UserEntity, bool>>>())).Returns(new List<UserEntity>().AsQueryable());

            // Act
            var result = repository.UserIsRegistered(principal);

            // Assert
            Assert.IsFalse(result);
            unitOfWorkMock.Verify(uow => uow.Load("Users", It.IsAny<Expression<Func<UserEntity, bool>>>()), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryUserIsRegisteredCallsLoadFromTheUnitOfWorkForAnExistingUser()
        {
            // Arrange
            var claims = new[]
                                     {
                                         new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "WindowsLiveID") ,
                                         new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "nameIdentifier"),
                                         new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", "test@test.com")
                                     };

            IClaimsIdentity identity = new ClaimsIdentity(claims);
            IClaimsPrincipal principal = new ClaimsPrincipal(new[] { identity });
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var repository = new UsersRepository(unitOfWorkMock.Object);
            unitOfWorkMock.Setup(uow => uow.Load("Users", It.IsAny<Expression<Func<UserEntity, bool>>>())).Returns(BuildUsersTable());

            // Act
            var result = repository.UserIsRegistered(principal);

            // Assert
            Assert.IsTrue(result);
            unitOfWorkMock.Verify(uow => uow.Load("Users", It.IsAny<Expression<Func<UserEntity, bool>>>()), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryLoadNoteOwnerCallsLoadAndGetFromTheUnitOfWork()
        {
            // Arrange
            var userEntity = new UserEntity(User1PartitionKey, User1RowKey);
            var note = new Note { PartitionKey = Note1PartitionKey, RowKey = _note1RowKey };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Get<UserEntity>("Users", User1PartitionKey, User1RowKey)).Returns(userEntity);
            var repository = new UsersRepository(unitOfWorkMock.Object);

            // Act
            repository.LoadOwner(note);

            // Assert
            Assert.IsNotNull(note.Owner);
            unitOfWorkMock.Verify(uow => uow.Get<UserEntity>("Users", User1PartitionKey, User1RowKey), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryGetByIdentifiersCallsLoadFromTheUnitOfWorkForNonExistingUser()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Load("Users", It.IsAny<Expression<Func<UserEntity, bool>>>())).Returns(new List<UserEntity>().AsQueryable());
            var repository = new UsersRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.GetByIdentifiers(Guid.NewGuid().ToString(), "WindowsLiveID");

            // Assert
            Assert.IsNull(result);
            unitOfWorkMock.Verify(uow => uow.Load("Users", It.IsAny<Expression<Func<UserEntity, bool>>>()), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryGetByIdentifiersCallsLoadFromTheUnitOfWorkForAnExistingUser()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Load("Users", It.IsAny<Expression<Func<UserEntity, bool>>>())).Returns(BuildUsersTable());
            var repository = new UsersRepository(unitOfWorkMock.Object);

            // Act
            var result = repository.GetByIdentifiers(Guid.NewGuid().ToString(), "WindowsLiveID");

            // Assert
            Assert.IsNotNull(result);
            unitOfWorkMock.Verify(uow => uow.Load("Users", It.IsAny<Expression<Func<UserEntity, bool>>>()), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryLoadTaskListOwnerCallsLoadAndGetFromTheUnitOfWork()
        {
            // Arrange
            var userEntity = new UserEntity(User1PartitionKey, User1RowKey);
            var taskList = new TaskList { PartitionKey = TaskList1PartitionKey, RowKey = _taskList1RowKey };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Get<UserEntity>("Users", User1PartitionKey, User1RowKey)).Returns(userEntity);
            var repository = new UsersRepository(unitOfWorkMock.Object);

            // Act
            repository.LoadOwner(taskList);

            // Assert
            Assert.IsNotNull(taskList.Owner);
            unitOfWorkMock.Verify(uow => uow.Get<UserEntity>("Users", User1PartitionKey, User1RowKey), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryLoadShareCallsLoadAndGetFromTheUnitOfWork()
        {
            // Arrange
            var user = new User { PartitionKey = User1PartitionKey, RowKey = User1RowKey };
            var user1Entity = new UserEntity(User1PartitionKey, User1RowKey);
            var user3Entity = new UserEntity(User3PartitionKey, User3RowKey);
            var taskList = new TaskList("Test title", user) { PartitionKey = TaskList1PartitionKey, RowKey = _taskList1RowKey };
            var note = new Note("Test title", "Test content", user, taskList);
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Load("NoteShares", It.IsAny<Expression<Func<NoteShareEntity, bool>>>())).Returns(BuildNoteSharesTable());
            unitOfWorkMock.Setup(u => u.Get<UserEntity>("Users", User1PartitionKey, User1RowKey)).Returns(user1Entity);
            unitOfWorkMock.Setup(u => u.Get<UserEntity>("Users", User3PartitionKey, User3RowKey)).Returns(user3Entity);
            var repository = new UsersRepository(unitOfWorkMock.Object);

            // Act
            repository.LoadShare(note);

            // Assert
            Assert.IsTrue(note.Share.Count == 2);
            unitOfWorkMock.Verify(uow => uow.Load("NoteShares", It.IsAny<Expression<Func<NoteShareEntity, bool>>>()), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Get<UserEntity>("Users", User1PartitionKey, User1RowKey), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Get<UserEntity>("Users", User3PartitionKey, User3RowKey), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryLoadTaskListAssociatedUsersCallsLoadAndGetFromTheUnitOfWork()
        {
            // Arrange
            var user = new User { PartitionKey = User1PartitionKey, RowKey = User1RowKey };
            var user1Entity = new UserEntity(User1PartitionKey, User1RowKey);
            var user3Entity = new UserEntity(User3PartitionKey, User3RowKey);
            var taskList = new TaskList("Test title", user) { PartitionKey = TaskList1PartitionKey, RowKey = _taskList1RowKey };
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Load("TaskListShares", It.IsAny<Expression<Func<TaskListShareEntity, bool>>>())).Returns(BuildTaskListSharesTable());
            unitOfWorkMock.Setup(u => u.Get<UserEntity>("Users", User1PartitionKey, User1RowKey)).Returns(user1Entity);
            unitOfWorkMock.Setup(u => u.Get<UserEntity>("Users", User3PartitionKey, User3RowKey)).Returns(user3Entity);
            var repository = new UsersRepository(unitOfWorkMock.Object);

            // Act
            repository.LoadShare(taskList);

            // Assert
            Assert.IsTrue(taskList.Share.Count == 2);
            unitOfWorkMock.Verify(uow => uow.Load("TaskListShares", It.IsAny<Expression<Func<TaskListShareEntity, bool>>>()), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Get<UserEntity>("Users", User1PartitionKey, User1RowKey), Times.Once());
            unitOfWorkMock.Verify(uow => uow.Get<UserEntity>("Users", User3PartitionKey, User3RowKey), Times.Once());
        }

        [TestMethod]
        public void UsersRepositoryParseIdentityProviderParsesWindowsLiveIDCorrectly()
        {
            // Arrange
            const string identityProviderClaim = "urn:WindowsLiveID";

            // Act
            var result = UsersRepository.ParseIdentityProvider(identityProviderClaim);

            // Assert
            Assert.IsTrue(result == "windowsliveid");
        }

        [TestMethod]
        public void UsersRepositoryParseIdentityProviderParsesGoogleCorrectly()
        {
            // Arrange
            const string identityProviderClaim = "Google";

            // Act
            var result = UsersRepository.ParseIdentityProvider(identityProviderClaim);

            // Assert
            Assert.IsTrue(result == "google");
        }

        [TestMethod]
        public void UsersRepositoryParseIdentityProviderParsesYahooCorrectly()
        {
            // Arrange
            const string identityProviderClaim = "Yahoo";

            // Act
            var result = UsersRepository.ParseIdentityProvider(identityProviderClaim);

            // Assert
            Assert.IsTrue(result == "yahoo");
        }

        #endregion UsersRepository tests

        #region Private methods

        private IQueryable<NoteEntity> BuildNotesTable()
        {
            var notesTable = new List<NoteEntity>
                                 {
                                     new NoteEntity(User1PartitionKey, User1RowKey), 
                                     new NoteEntity(User2PartitionKey, User2RowKey)
                                 };

            return notesTable.AsQueryable();
        }

        private IQueryable<TaskListEntity> BuildTaskListsTable()
        {
            var taskListsTable = new List<TaskListEntity>
                                     {
                                         new TaskListEntity(TaskList1PartitionKey, _taskList1RowKey),
                                         new TaskListEntity(TaskList2PartitionKey, _taskList2RowKey)
                                     };

            return taskListsTable.AsQueryable();
        }

        private IQueryable<UserEntity> BuildUsersTable()
        {
            var usersTable = new List<UserEntity>
                                 {
                                     new UserEntity(User1PartitionKey, User1RowKey) { UniqueIdentifier = Guid.NewGuid().ToString() }, 
                                     new UserEntity(User2PartitionKey, User2RowKey) { UniqueIdentifier = Guid.NewGuid().ToString() }
                                 };

            return usersTable.AsQueryable();
        }

        private IQueryable<NoteShareEntity> BuildNoteSharesTable()
        {
            var shares = new List<NoteShareEntity>
                            {
                                new NoteShareEntity(string.Format("{0}+{1}", Note1PartitionKey, _note1RowKey), User1RowKey),
                                new NoteShareEntity(string.Format("{0}+{1}", Note1PartitionKey, _note1RowKey), User3RowKey)
                            };

            return shares.AsQueryable();
        }

        private IQueryable<TaskListShareEntity> BuildTaskListSharesTable()
        {
            var shares = new List<TaskListShareEntity>
                            {
                                new TaskListShareEntity(string.Format("{0}+{1}", TaskList1PartitionKey, _taskList1RowKey), User1RowKey),
                                new TaskListShareEntity(string.Format("{0}+{1}", TaskList1PartitionKey, _taskList1RowKey), User3RowKey)
                            };

            return shares.AsQueryable();
        }

        private IQueryable<TaskListNoteEntity> BuildTaskListNotesTable()
        {
            var taskListNotes = new List<TaskListNoteEntity>
                                    {
                                        new TaskListNoteEntity(string.Format("{0}+{1}", TaskList1PartitionKey, _taskList1RowKey), 
                                                                string.Format("{0}+{1}", Note1PartitionKey, _note1RowKey)),
                                        new TaskListNoteEntity(string.Format("{0}+{1}", TaskList1PartitionKey, _taskList1RowKey), 
                                                                string.Format("{0}+{1}", Note2PartitionKey, _note2RowKey))
                                    };

            return taskListNotes.AsQueryable();
        }

        #endregion Private methods
    }
}