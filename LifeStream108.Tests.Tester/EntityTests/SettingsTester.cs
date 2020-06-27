using LifeStream108.Modules.SettingsManagement;
using NUnit.Framework;

namespace LifeStream108.Tests.Tester.EntityTests
{
    internal static class SettingsTester
    {
        public static void Run()
        {
            SettingEntry setting = SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString);
            Assert.NotNull(setting, "Setting entry is null");
        }
    }
}
