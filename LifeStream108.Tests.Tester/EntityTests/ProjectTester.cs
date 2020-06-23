using LifeStream108.Libs.Entities.DictionaryEntities;
using LifeStream108.Modules.DictionaryManagement;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeStream108.Tests.Tester.EntityTests
{
    internal static class ProjectTester
    {
        public static void Run()
        {
            Project[] projects = ProjectManager.GetProjects();
            Assert.IsTrue(projects.Length > 0);
        }
    }
}
