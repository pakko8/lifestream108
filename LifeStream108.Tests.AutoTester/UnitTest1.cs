using LifeStream108.Libs.Entities.DictionaryEntities;
using LifeStream108.Libs.PostgreSqlHelper;
using LifeStream108.Modules.DictionaryManagement;
using NUnit.Framework;

namespace LifeStream108.Tests.AutoTester
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Measure measure1 = new Measure
            {
                Name = "Measure #1",
                ShortName = "M1",
                Declanation1 = "D1_1",
                Declanation2 = "D1_2",
                Declanation3 = "D1_3"
            };
            Measure measure2 = new Measure
            {
                Name = "Measure #2",
                ShortName = "M2",
                Declanation1 = "D2_1",
                Declanation2 = "D2_2",
                Declanation3 = "D2_3"
            };

            PostgreSqlCommandUtils.UpdateEntity(
                $"delete from measures where name in ('{measure1.Name}', '{measure2.Name}')", null);

            MeasureManager.AddMeasure(measure1);
            MeasureManager.AddMeasure(measure1);

            Assert.Pass();
        }
    }
}
