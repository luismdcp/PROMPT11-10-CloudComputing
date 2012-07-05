using CloudNotes.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CloudNotes.Tests.Domain
{
    [TestClass]
    public class DomainTests
    {
        #region Note unit tests

        [TestMethod]
        public void ANoteWithAnEmptyTittleIsInvalid()
        {
            var user = new User("userPartitionKey", "userRowKey");
            var taskList = new TaskList("taskListPartitionKey", "taskListRowKey");
            var invalidTitle = string.Empty;
            var validContent = "ipsum lorum";
            var note = new Note("notePartitionKey", "noteRowKey", invalidTitle, validContent, user, taskList);

            var validationResult = note.IsValid();

            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void ANoteWithAnAllWhitespaceTittleIsInvalid()
        {
            var user = new User("userPartitionKey", "userRowKey");
            var taskList = new TaskList("taskListPartitionKey", "taskListRowKey");
            var invalidTitle = " ";
            var validContent = "ipsum lorum";
            var note = new Note("notePartitionKey", "noteRowKey", invalidTitle, validContent, user, taskList);

            var validationResult = note.IsValid();

            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void ANoteWithATittleLongerThan20CharactersIsInvalid()
        {
            var user = new User("userPartitionKey", "userRowKey");
            var taskList = new TaskList("taskListPartitionKey", "taskListRowKey");
            var invalidTitle = "averylonginvalidtitle";
            var validContent = "ipsum lorum";
            var note = new Note("notePartitionKey", "noteRowKey", invalidTitle, validContent, user, taskList);

            var validationResult = note.IsValid();

            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void ANoteWithAnAllWhitespaceContentIsInvalid()
        {
            var user = new User("userPartitionKey", "userRowKey");
            var taskList = new TaskList("taskListPartitionKey", "taskListRowKey");
            var validTitle = "ipsum lorum";
            var invalidContent = " ";
            var note = new Note("notePartitionKey", "noteRowKey", validTitle, invalidContent, user, taskList);

            var validationResult = note.IsValid();

            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void ANoteWithAnEmptyContentIsInvalid()
        {
            var user = new User("userPartitionKey", "userRowKey");
            var taskList = new TaskList("taskListPartitionKey", "taskListRowKey");
            var validTitle = "ipsum lorum";
            var invalidContent = string.Empty;
            var note = new Note("notePartitionKey", "noteRowKey", validTitle, invalidContent, user, taskList);

            var validationResult = note.IsValid();

            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void ANoteWithContentLongerThan50CharactersIsInvalid()
        {
            var user = new User("userPartitionKey", "userRowKey");
            var taskList = new TaskList("taskListPartitionKey", "taskListRowKey");
            var validTitle = "ipsum lorum";
            var invalidContent = "blablablablablablablablablablablablablablablablablablablablablablablablablablablablablabla";
            var note = new Note("notePartitionKey", "noteRowKey", validTitle, invalidContent, user, taskList);

            var validationResult = note.IsValid();

            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void ANoteWithANullOwnerIsInvalid()
        {
            var taskList = new TaskList("taskListPartitionKey", "taskListRowKey");
            var validTitle = "ipsum lorum";
            var validContent = "ipsum lorum";
            var note = new Note("notePartitionKey", "noteRowKey", validTitle, validContent, null, taskList);

            var validationResult = note.IsValid();

            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void ANoteWithANullContainerListIsInvalid()
        {
            var user = new User("userPartitionKey", "userRowKey");
            var validTitle = "ipsum lorum";
            var validContent = "ipsum lorum";
            var note = new Note("notePartitionKey", "noteRowKey", validTitle, validContent, user, null);

            var validationResult = note.IsValid();

            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void ANoteWithAllValidPropertiesIsValid()
        {
            var user = new User("userPartitionKey", "userRowKey");
            var taskList = new TaskList("taskListPartitionKey", "taskListRowKey");
            var validTitle = "ipsum lorum";
            var validContent = "ipsum lorum";
            var note = new Note("notePartitionKey", "noteRowKey", validTitle, validContent, user, taskList);

            var validationResult = note.IsValid();

            Assert.IsTrue(validationResult);
        }

        #endregion Note unit tests

        #region TaskList unit tests

        [TestMethod]
        public void ATaskListWithAnEmptyTittleIsInvalid()
        {
            var user = new User("userPartitionKey", "userRowKey");
            var invalidTitle = string.Empty;
            var taskList = new TaskList("taskListPartitionKey", "taskListRowKey", invalidTitle, user);

            var validationResult = taskList.IsValid();

            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void ATaskListWithAnAllWhitespaceTittleIsInvalid()
        {
            var user = new User("userPartitionKey", "userRowKey");
            var invalidTitle = " ";
            var taskList = new TaskList("taskListPartitionKey", "taskListRowKey", invalidTitle, user);

            var validationResult = taskList.IsValid();

            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void ATaskListWithATittleLongerThan20CharactersIsInvalid()
        {
            var user = new User("userPartitionKey", "userRowKey");
            var invalidTitle = "aninvalidtittlewihtmoretahn20characters";
            var taskList = new TaskList("taskListPartitionKey", "taskListRowKey", invalidTitle, user);

            var validationResult = taskList.IsValid();

            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void ATaskListWithAllPropertiesValidIsValid()
        {
            var user = new User("userPartitionKey", "userRowKey");
            var validTitle = "ipsum lorum";
            var taskList = new TaskList("taskListPartitionKey", "taskListRowKey", validTitle, user);

            var validationResult = taskList.IsValid();

            Assert.IsTrue(validationResult);
        }

        #endregion TaskList unit tests

        #region User unit tests

        [TestMethod]
        public void AUserWithAnEmptyNameIdentitiferIdentityProviderIsInvalid()
        {
            var user = new User("userPartitionKey", "userRowKey", string.Empty, string.Empty);

            var validationResult = user.IsValid();

            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void AUserWithAnAllWhitespaceNameIdentitiferIdentityProviderIsInvalid()
        {
            var user = new User("userPartitionKey", "userRowKey", " ", string.Empty);

            var validationResult = user.IsValid();

            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void AUserWithAnInvalidEmailIsInvalid()
        {
            var user = new User("userPartitionKey", "userRowKey", "nameidnetifieridentityprovider", "ipsumlorum");

            var validationResult = user.IsValid();

            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void AUserWithAValidEmailIsValid()
        {
            var user = new User("userPartitionKey", "userRowKey", "nameidentifieridentityprovider", "test@test.com");

            var validationResult = user.IsValid();

            Assert.IsTrue(validationResult);
        }

        #endregion User unit tests
    }
}