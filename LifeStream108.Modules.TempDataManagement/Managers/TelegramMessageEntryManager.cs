using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities;
using LifeStream108.Libs.HibernateManagement;
using NHibernate;
using System.Linq;

namespace LifeStream108.Modules.TempDataManagement.Managers
{
    public static class TelegramMessageEntryManager
    {
        public static TelegramMessageEntry[] GetEntriesForUser(int telegramUserId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from entry in session.Query<TelegramMessageEntry>()
                            where entry.TelegramUserId == telegramUserId
                            select entry;
                return query.ToArray();
            }
        }

        public static void AddEntry(TelegramMessageEntry entry)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                CommonManager<TelegramMessageEntry>.Add(entry, session);
                session.Flush();
            }
        }

        public static void DeleteEntries(TelegramMessageEntry[] entries)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                Batches<TelegramMessageEntry> batchIds = new Batches<TelegramMessageEntry>(entries, 30);
                foreach (TelegramMessageEntry[] batchPart in batchIds)
                {
                    foreach (TelegramMessageEntry entry in batchPart)
                    {
                        CommonManager<TelegramMessageEntry>.Delete(entry, session);
                    }
                    session.Flush();
                }
            }
        }
    }
}
