using LifeStream108.Libs.Entities.DictionaryEntities;
using LifeStream108.Libs.PostgreSqlHelper;
using LifeStream108.Modules.DictionaryManagement;
using NUnit.Framework;

namespace LifeStream108.Tests.Tester.EntityTests
{
    public static class MeasureTester
    {
        public static void Run()
        {
            Measure measure1 = new Measure
            {
                UserId = Constants.UserId,
                Name = "Measure #1",
                ShortName = "M1",
                Declanation1 = "D1_1",
                Declanation2 = "D1_2",
                Declanation3 = "D1_3"
            };
            Measure measure2 = new Measure
            {
                UserId = Constants.UserId,
                Name = "Measure #2",
                ShortName = "M2",
                Declanation1 = "D2_1",
                Declanation2 = "D2_2",
                Declanation3 = "D2_3"
            };

            PostgreSqlCommandUtils.UpdateEntity(
                $"delete from measures where name in ('{measure1.Name}', '{measure2.Name}')", null);

            MeasureManager.AddMeasure(measure1);
            Assert.IsTrue(measure1.Id > 0);
            MeasureManager.AddMeasure(measure2);
            Assert.IsTrue(measure2.Id > 0);

            Measure[] measures = MeasureManager.GetMeasuresForUser(Constants.UserId);
            Assert.IsTrue(measures.Length > 0);

        }
    }
}
