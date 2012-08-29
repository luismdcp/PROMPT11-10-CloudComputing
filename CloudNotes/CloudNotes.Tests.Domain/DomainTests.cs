using System;
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
            // Arrange
            var user = new User() { PartitionKey = "windowsliveid", RowKey = "user.test-windowsliveid" };
            var taskList = new TaskList("Test title", user) { PartitionKey = "user.test-windowsliveid", RowKey = ShortGuid.NewGuid().ToString() };
            var invalidTitle = string.Empty;
            const string validContent = "Test content";
            var note = new Note(invalidTitle, validContent, user, taskList) { PartitionKey = taskList.RowKey, RowKey = ShortGuid.NewGuid().ToString() };

            // Act
            var validationResult = note.IsValid();

            // Assert
            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void ANoteWithAnAllWhitespaceTittleIsInvalid()
        {
            // Arrange
            var user = new User() { PartitionKey = "windowsliveid", RowKey = "user.test-windowsliveid" };
            var taskList = new TaskList("Test title", user) { PartitionKey = "user.test-windowsliveid", RowKey = ShortGuid.NewGuid().ToString() };
            const string invalidTitle = " ";
            const string validContent = "Test content";
            var note = new Note(invalidTitle, validContent, user, taskList) { PartitionKey = taskList.RowKey, RowKey = ShortGuid.NewGuid().ToString() };

            // Act
            var validationResult = note.IsValid();

            // Assert
            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void ANoteWithATittleLongerThan10CharactersIsInvalid()
        {
            // Arrange
            var user = new User() { PartitionKey = "windowsliveid", RowKey = "user.test-windowsliveid" };
            var taskList = new TaskList("Test title", user) { PartitionKey = "user.test-windowsliveid", RowKey = ShortGuid.NewGuid().ToString() };
            const string invalidTitle = "A very long invalid title";
            const string validContent = "Test content";
            var note = new Note(invalidTitle, validContent, user, taskList) { PartitionKey = taskList.RowKey, RowKey = ShortGuid.NewGuid().ToString() };

            // Act
            var validationResult = note.IsValid();

            // Assert
            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void ANoteWithAnAllWhitespaceContentIsInvalid()
        {
            // Arrange
            var user = new User() { PartitionKey = "windowsliveid", RowKey = "user.test-windowsliveid" };
            var taskList = new TaskList("Test title", user) { PartitionKey = "user.test-windowsliveid", RowKey = ShortGuid.NewGuid().ToString() };
            const string validTitle = "Test title";
            const string invalidContent = " ";
            var note = new Note(validTitle, invalidContent, user, taskList) { PartitionKey = taskList.RowKey, RowKey = ShortGuid.NewGuid().ToString() };

            // Act
            var validationResult = note.IsValid();

            // Assert
            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void ANoteWithAnEmptyContentIsInvalid()
        {
            // Arrange
            var user = new User() { PartitionKey = "windowsliveid", RowKey = "user.test-windowsliveid" };
            var taskList = new TaskList("Test title", user) { PartitionKey = "user.test-windowsliveid", RowKey = ShortGuid.NewGuid().ToString() };
            const string validTitle = "Test title";
            string invalidContent = string.Empty;
            var note = new Note(validTitle, invalidContent, user, taskList) { PartitionKey = taskList.RowKey, RowKey = ShortGuid.NewGuid().ToString() };

            // Act
            var validationResult = note.IsValid();

            // Assert
            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void ANoteWithContentLongerThan50CharactersIsInvalid()
        {
            // Arrange
            var user = new User() { PartitionKey = "windowsliveid", RowKey = "user.test-windowsliveid" };
            var taskList = new TaskList("Test title", user) { PartitionKey = "user.test-windowsliveid", RowKey = ShortGuid.NewGuid().ToString() };
            const string validTitle = "Test title";
            const string invalidContent = "blablablablablablablablablablablablablablablablablablablablablablablablablablablablablabla";
            var note = new Note(validTitle, invalidContent, user, taskList) { PartitionKey = taskList.RowKey, RowKey = ShortGuid.NewGuid().ToString() };

            // Act
            var validationResult = note.IsValid();

            // Assert
            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void ANoteWithANullOwnerIsInvalid()
        {
            // Arrange
            var user = new User() { PartitionKey = "windowsliveid", RowKey = "user.test-windowsliveid" };
            var taskList = new TaskList("Test title", user) { PartitionKey = "user.test-windowsliveid", RowKey = ShortGuid.NewGuid().ToString() };
            const string validTitle = "Test title";
            const string validContent = "Test content";
            var note = new Note(validTitle, validContent, null, taskList) { PartitionKey = taskList.RowKey, RowKey = ShortGuid.NewGuid().ToString() };

            // Act
            var validationResult = note.IsValid();

            // Assert
            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void ANoteWithANullContainerListIsInvalid()
        {
            // Arrange
            var user = new User() { PartitionKey = "windowsliveid", RowKey = "user.test-windowsliveid" };
            const string validTitle = "Test title";
            const string validContent = "Test content";
            var note = new Note(validTitle, validContent, user, null) { PartitionKey = string.Empty, RowKey = Guid.NewGuid().ToString() };

            // Act
            var validationResult = note.IsValid();

            // Assert
            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void ANoteWithAllValidPropertiesIsValid()
        {
            // Arrange
            var user = new User() { PartitionKey = "windowsliveid", RowKey = "user.test-windowsliveid" };
            var taskList = new TaskList("Test title", user) { PartitionKey = "user.test-windowsliveid", RowKey = ShortGuid.NewGuid().ToString() };
            const string validTitle = "Test title";
            const string validContent = "Test content";
            var note = new Note(validTitle, validContent, user, taskList) { PartitionKey = taskList.RowKey, RowKey = ShortGuid.NewGuid().ToString() };

            // Act
            var validationResult = note.IsValid();

            // Assert
            Assert.IsTrue(validationResult);
        }

        #endregion Note unit tests

        #region TaskList unit tests

        [TestMethod]
        public void ATaskListWithAnEmptyTittleIsInvalid()
        {
            // Arrange
            var user = new User() { PartitionKey = "windowsliveid", RowKey = "user.test-windowsliveid" };
            var invalidTitle = string.Empty;
            var taskList = new TaskList(invalidTitle, user) { PartitionKey = "user.test-windowsliveid", RowKey = ShortGuid.NewGuid().ToString() };

            // Act
            var validationResult = taskList.IsValid();

            // Assert
            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void ATaskListWithAnAllWhitespaceTittleIsInvalid()
        {
            // Arrange
            var user = new User() { PartitionKey = "windowsliveid", RowKey = "user.test-windowsliveid" };
            const string invalidTitle = " ";
            var taskList = new TaskList(invalidTitle, user) { PartitionKey = "user.test-windowsliveid", RowKey = ShortGuid.NewGuid().ToString() };

            // Act
            var validationResult = taskList.IsValid();

            // Assert
            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void ATaskListWithATittleLongerThan20CharactersIsInvalid()
        {
            // Arrange
            var user = new User() { PartitionKey = "windowsliveid", RowKey = "user.test-windowsliveid" };
            const string invalidTitle = "An invalid tittle with more than 10 characters";
            var taskList = new TaskList(invalidTitle, user) { PartitionKey = "user.test-windowsliveid", RowKey = ShortGuid.NewGuid().ToString() };

            // Act
            var validationResult = taskList.IsValid();

            // Assert
            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void ATaskListWithAllPropertiesValidIsValid()
        {
            // Arrange
            var user = new User() { PartitionKey = "windowsliveid", RowKey = "user.test-windowsliveid" };
            const string validTitle = "Test Title";
            var taskList = new TaskList(validTitle, user) { PartitionKey = "user.test-windowsliveid", RowKey = ShortGuid.NewGuid().ToString() };

            // Act
            var validationResult = taskList.IsValid();

            // Assert
            Assert.IsTrue(validationResult);
        }

        #endregion TaskList unit tests

        #region User unit tests

        [TestMethod]
        public void AUserWithAnNameThatHasTheCharacterHypenIsInvalid()
        {
            // Arrange
            var user = new User() { PartitionKey = "windowsliveid", RowKey = "user.test-windowsliveid", Name = "user-name" };

            // Act
            var validationResult = user.IsValid();

            // Assert
            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void AUserWithAnNameThatHasTheCharacterPlusIsInvalid()
        {
            // Arrange
            var user = new User() { PartitionKey = "windowsliveid", RowKey = "user.test-windowsliveid", Name = "user+name" };

            // Act
            var validationResult = user.IsValid();

            // Assert
            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void AUserWithAnEmptyNameIsInvalid()
        {
            // Arrange
            var user = new User() { PartitionKey = "windowsliveid", RowKey = "user.test-windowsliveid", Name = "" };

            // Act
            var validationResult = user.IsValid();

            // Assert
            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void AUserWithAnNameAllWhitespacesIsInvalid()
        {
            // Arrange
            var user = new User() { PartitionKey = "windowsliveid", RowKey = "user.test-windowsliveid", Name = " " };

            // Act
            var validationResult = user.IsValid();

            // Assert
            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void AUserWithAnInvalidEmailIsInvalid()
        {
            // Arrange
            var user = new User() { PartitionKey = "windowsliveid", RowKey = "user.test-windowsliveid", Name = "usertest", Email = "Invalid Email" };

            // Act
            var validationResult = user.IsValid();

            // Assert
            Assert.IsFalse(validationResult);
        }

        [TestMethod]
        public void AUserWithAValidEmailIsValid()
        {
            // Arrange
            var user = new User() { PartitionKey = "windowsliveid", RowKey = "user.test-windowsliveid", Name = "usertest", Email = "user@test.rog" };

            // Act
            var validationResult = user.IsValid();

            // Assert
            Assert.IsTrue(validationResult);
        }

        #endregion User unit tests
    }
}