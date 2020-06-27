namespace LifeStream108.Tests.Tester.EntityTests
{
    internal static class AllEntitiesRunner
    {
        public static void Run()
        {
            SettingsTester.Run();

            UserTester.Run();
            SessionTester.Run();
            MeasureTester.Run();
            ProjectTester.Run();
            CommandTester.Run();

            ToDoEntitiesTester.Run();
        }
    }
}
