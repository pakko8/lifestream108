using LifeStream108.Libs.Entities.DictionaryEntities;
using LifeStream108.Libs.HibernateManagement;
using NHibernate;
using System.Linq;

namespace LifeStream108.Modules.DictionaryManagement.Managers
{
    public static class ProjectManager
    {
        public static Project[] GetProjects()
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                return CommonManager<Project>.GetAll(session);
            }
        }

        public static Project GetProject(int projectId)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                return CommonManager<Project>.GetById(projectId, session);
            }
        }

        public static Project GetProjectByCode(string code)
        {
            using (ISession session = HibernateLoader.CreateSession())
            {
                var query = from prj in session.Query<Project>()
                            where prj.UserCode.ToUpper() == code.ToUpper()
                            select prj;
                return query.FirstOrDefault();
            }
        }
    }
}
