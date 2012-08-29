using CloudNotes.Tests.Domain;
using CloudNotes.WebRole;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcRouteUnitTester;

namespace CloudNotes.Tests.Presentation.MVC
{
    [TestClass]
    public class MVCTests
    {
        [TestMethod]
        public void TestIncomingRoutes()
        {
            // Arrange
            var tester = new RouteTester<MvcApplication>();
            var taskListId = ShortGuid.NewGuid().ToString();
            const string taskListOwnerId = "user-windowsliveid";
            var noteId = ShortGuid.NewGuid().ToString();
            const string noteOwnerId = "user-windowsliveid";
            const string fileName = "test.txt";

            // Assert
            tester.WithIncomingRequest("/handler.axd/pathInfo").ShouldBeIgnored();
            tester.WithIncomingRequest("/").ShouldMatchRoute("Users", "Home");

            tester.WithIncomingRequest("/TaskLists/Index").ShouldMatchRoute("TaskLists", "Index");
            tester.WithIncomingRequest(string.Format("/TaskLists/{0}/{1}/Details", taskListOwnerId, taskListId)).ShouldMatchRoute("TaskLists", "Details", new { taskListOwnerId, taskListId });
            tester.WithIncomingRequest(string.Format("/TaskLists/{0}/{1}/Edit", taskListOwnerId, taskListId)).ShouldMatchRoute("TaskLists", "Edit", new { taskListOwnerId, taskListId });
            tester.WithIncomingRequest(string.Format("/TaskLists/{0}/{1}/Delete", taskListOwnerId, taskListId)).ShouldMatchRoute("TaskLists", "Delete", new { taskListOwnerId, taskListId });
            tester.WithIncomingRequest(string.Format("/TaskLists/{0}/{1}/Share", taskListOwnerId, taskListId)).ShouldMatchRoute("TaskLists", "Share", new { taskListOwnerId, taskListId });

            tester.WithIncomingRequest(string.Format("/Notes/TaskList/{0}/{1}/Index", taskListOwnerId, taskListId)).ShouldMatchRoute("Notes", "Index", new { taskListOwnerId, taskListId });
            tester.WithIncomingRequest(string.Format("/Notes/TaskList/{0}/{1}/Create", taskListOwnerId, taskListId)).ShouldMatchRoute("Notes", "Create", new { taskListOwnerId, taskListId });
            tester.WithIncomingRequest(string.Format("/Notes/{0}/{1}/Details", noteOwnerId, noteId)).ShouldMatchRoute("Notes", "Details", new { noteOwnerId, noteId });
            tester.WithIncomingRequest(string.Format("/Notes/{0}/{1}/Delete", noteOwnerId, noteId)).ShouldMatchRoute("Notes", "Delete", new { noteOwnerId, noteId });
            tester.WithIncomingRequest(string.Format("/Notes/{0}/{1}/Edit", noteOwnerId, noteId)).ShouldMatchRoute("Notes", "Edit", new { noteOwnerId, noteId });
            tester.WithIncomingRequest(string.Format("/Notes/{0}/{1}/Share", noteOwnerId, noteId)).ShouldMatchRoute("Notes", "Share", new { noteOwnerId, noteId });
            tester.WithIncomingRequest(string.Format("/Notes/{0}/{1}/Copy", noteOwnerId, noteId)).ShouldMatchRoute("Notes", "Copy", new { noteOwnerId, noteId });
            tester.WithIncomingRequest(string.Format("/Notes/{0}/{1}/Move", noteOwnerId, noteId)).ShouldMatchRoute("Notes", "Move", new { noteOwnerId, noteId });

            tester.WithIncomingRequest(string.Format("/Files/Note/{0}/{1}/Index", noteOwnerId, noteId)).ShouldMatchRoute("Files", "Index", new { noteOwnerId, noteId });
            tester.WithIncomingRequest(string.Format("/Files/Note/{0}/{1}/Download/{2}", noteOwnerId, noteId, fileName)).ShouldMatchRoute("Files", "Download", new { noteOwnerId, noteId, fileName });
            tester.WithIncomingRequest(string.Format("/Files/Note/{0}/{1}/Delete/{2}", noteOwnerId, noteId, fileName)).ShouldMatchRoute("Files", "Delete", new { noteOwnerId, noteId, fileName });
        }

        [TestMethod]
        public void TestOutgoingRoutes()
        {
            // Arrange
            var tester = new RouteTester<MvcApplication>();
            var taskListId = ShortGuid.NewGuid().ToString();
            const string taskListOwnerId = "user-windowsliveid";
            var noteId = ShortGuid.NewGuid().ToString();
            const string noteOwnerId = "user-windowsliveid";
            const string fileName = "test.txt";

            // Assert
            tester.WithRouteInfo("Users", "Home").ShouldGenerateUrl("/");
            tester.WithRouteInfo("TaskLists", "Index").ShouldGenerateUrl("/TaskLists/Index");
            tester.WithRouteInfo("TaskLists", "Index", new { sortOrder = "Title", page = 1 }).ShouldGenerateUrl("/TaskLists/Index?sortOrder=Title&page=1");
            tester.WithRouteInfo("TaskLists", "Details", new { taskListOwnerId, taskListId }).ShouldGenerateUrl(string.Format("/TaskLists/{0}/{1}/Details", taskListOwnerId, taskListId));
            tester.WithRouteInfo("TaskLists", "Edit", new { taskListOwnerId, taskListId }).ShouldGenerateUrl(string.Format("/TaskLists/{0}/{1}/Edit", taskListOwnerId, taskListId));
            tester.WithRouteInfo("TaskLists", "Delete", new { taskListOwnerId, taskListId }).ShouldGenerateUrl(string.Format("/TaskLists/{0}/{1}/Delete", taskListOwnerId, taskListId));
            tester.WithRouteInfo("TaskLists", "Share", new { taskListOwnerId, taskListId }).ShouldGenerateUrl(string.Format("/TaskLists/{0}/{1}/Share", taskListOwnerId, taskListId));

            tester.WithRouteInfo("Notes", "Index", new { taskListOwnerId, taskListId }).ShouldGenerateUrl(string.Format("/Notes/TaskList/{0}/{1}", taskListOwnerId, taskListId));
            tester.WithRouteInfo("Notes", "Create", new { taskListOwnerId, taskListId }).ShouldGenerateUrl(string.Format("/Notes/TaskList/{0}/{1}/Create", taskListOwnerId, taskListId));
            tester.WithRouteInfo("Notes", "Details", new { noteOwnerId, noteId }).ShouldGenerateUrl(string.Format("/Notes/{0}/{1}/Details", noteOwnerId, noteId));
            tester.WithRouteInfo("Notes", "Delete", new { noteOwnerId, noteId }).ShouldGenerateUrl(string.Format("/Notes/{0}/{1}/Delete", noteOwnerId, noteId));
            tester.WithRouteInfo("Notes", "Edit", new { noteOwnerId, noteId }).ShouldGenerateUrl(string.Format("/Notes/{0}/{1}/Edit", noteOwnerId, noteId));
            tester.WithRouteInfo("Notes", "Share", new { noteOwnerId, noteId }).ShouldGenerateUrl(string.Format("/Notes/{0}/{1}/Share", noteOwnerId, noteId));
            tester.WithRouteInfo("Notes", "Copy", new { noteOwnerId, noteId }).ShouldGenerateUrl(string.Format("/Notes/{0}/{1}/Copy", noteOwnerId, noteId));
            tester.WithRouteInfo("Notes", "Move", new { noteOwnerId, noteId }).ShouldGenerateUrl(string.Format("/Notes/{0}/{1}/Move", noteOwnerId, noteId));

            tester.WithRouteInfo("Files", "Index", new { noteOwnerId, noteId }).ShouldGenerateUrl(string.Format("/Files/Note/{0}/{1}", noteOwnerId, noteId));
            tester.WithRouteInfo("Files", "Download", new { noteOwnerId, noteId, fileName }).ShouldGenerateUrl(string.Format("/Files/Note/{0}/{1}/Download/{2}", noteOwnerId, noteId, fileName));
            tester.WithRouteInfo("Files", "Delete", new { noteOwnerId, noteId, fileName }).ShouldGenerateUrl(string.Format("/Files/Note/{0}/{1}/Delete/{2}", noteOwnerId, noteId, fileName));
        }
    }
}