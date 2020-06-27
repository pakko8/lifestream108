using LifeStream108.Libs.Entities.DictionaryEntities;
using LifeStream108.Modules.DictionaryManagement;
using NUnit.Framework;

namespace LifeStream108.Tests.Tester.EntityTests
{
    internal static class ProjectTester
    {
        public static void Run()
        {
            Project[] projects = ProjectManager.GetProjects();
            Assert.IsTrue(projects.Length > 0, "Project list has too few items");
        }
    }
}
