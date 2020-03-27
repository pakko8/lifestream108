using LifeStream108.Libs.Entities.SettingsEntities;
using LifeStream108.Libs.HibernateManagement;
using NHibernate;
using System.Linq;

namespace LifeStream108.Modules.SettingsManagement.Managers
{
    public static class SettingsManager
    {
        public static SettingEntry[] GetAllSettings()
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                return CommonManager<SettingEntry>.GetAll(session);
            }
        }

        public static SettingEntry GetSettingEntryByCode(SettingCode code)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from sett in session.Query<SettingEntry>()
                            where sett.Code == code
                            select sett;
                return query.FirstOrDefault();
            }
        }
    }
}
