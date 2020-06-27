using LifeStream108.Libs.Entities.DictionaryEntities;
using LifeStream108.Libs.PostgreSqlHelper;
using LifeStream108.Modules.DictionaryManagement;
using NUnit.Framework;

namespace LifeStream108.Tests.Tester.EntityTests
{
    public static class MeasureTester
    {
        internal static Measure[] Measures;

        static MeasureTester()
        {
            Measures = new Measure[2];
            Measures[0] = new Measure
            {
                UserId = UserTester.UserId_1,
                Name = "Measure #1",
                ShortName = "M1",
                Declanation1 = "D1_1",
                Declanation2 = "D1_2",
                Declanation3 = "D1_3"
            };

            Measures[1] = new Measure
            {
                UserId = UserTester.UserId_1,
                Name = "Measure #2",
                ShortName = "M2",
                Declanation1 = "D2_1",
                Declanation2 = "D2_2",
                Declanation3 = "D2_3"
            };
        }

        public static void Run()
        {
            DeleteMeasures();

            MeasureManager.AddMeasure(Measures[0]);
            Assert.IsTrue(Measures[0].Id > 0, "Measure #1 not has id");
            MeasureManager.AddMeasure(Measures[1]);
            Assert.IsTrue(Measures[1].Id > 0, "Measure #2 not has id");

            Measure[] measures = MeasureManager.GetMeasuresForUser(Constants.UserId);
            Assert.IsTrue(measures.Length > 0, "Message list has too few items");

            DeleteMeasures();
        }

        private static void DeleteMeasures()
        {
            string tableName = TestHelper.GetClassConstantValue("TableName", typeof(MeasureManager));
            PostgreSqlCommandUtils.UpdateEntity(
                $"delete from {tableName} where name in ('{Measures[0].Name}', '{Measures[1].Name}')", null);
        }
    }
}
