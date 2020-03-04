using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using NLog;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace LifeStream108.Libs.HibernateManagement
{
    public class HibernateLoader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly ISessionFactory SessionFactory;

        static HibernateLoader()
        {
            try
            {
                var configNode = (XmlNode)ConfigurationManager.GetSection("fluent-hibernate-configuration");

                var rawConfig = new NHibernate.Cfg.Configuration();

                var configuration = Fluently.Configure(rawConfig)
                    .Database(PostgreSQLConfiguration.PostgreSQL82
                        .ConnectionString(x => x.FromConnectionStringWithKey(ConfigurationManager.AppSettings["LifeStream108DbConnSettName"])));
                //.ExposeConfiguration(x => x.SetInterceptor(new CustomInterceptor())); // Log sql queries

                // Add modules
                var moduleNodes = configNode["session-factory"];
                if (moduleNodes != null)
                {
                    foreach (XmlNode moduleNode in moduleNodes.SelectNodes("mapping"))
                    {
                        var assembly = System.Reflection.Assembly.Load(moduleNode.Attributes["assembly"].Value);
                        configuration.Mappings(m => m.FluentMappings.AddFromAssembly(assembly));
                    }
                }

                SessionFactory = configuration.ExposeConfiguration(cfg => { new SchemaUpdate(cfg).Execute(false, true); }).BuildSessionFactory();
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sbTrace = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sbTrace.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sbTrace.AppendLine("Fusion Log:");
                            sbTrace.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sbTrace.AppendLine();
                }
                Logger.Error("Load assembly error: " + sbTrace.ToString());
            }
            catch (Exception ex)
            {
                Logger.Error("Fluent configuration reading error: " + ex);
            }
        }

        public static ISession CreateSession()
        {
            try
            {
                return SessionFactory.OpenSession();
            }
            catch (Exception ex)
            {
                Logger.Error("Error to open session: " + ex);
                throw ex;
            }
        }
    }

    internal class CustomInterceptor : EmptyInterceptor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public override NHibernate.SqlCommand.SqlString OnPrepareStatement(NHibernate.SqlCommand.SqlString sql)
        {
            Logger.Info("Fluent NHibernate query: " + sql.ToString());
            return sql;
        }
    }
}
