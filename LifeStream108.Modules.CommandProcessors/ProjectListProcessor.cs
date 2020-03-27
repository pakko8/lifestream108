using System.Text;
using LifeStream108.Libs.Entities.DictionaryEntities;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Modules.DictionaryManagement.Managers;

namespace LifeStream108.Modules.CommandProcessors
{
    public class ProjectListProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            Project[] projects = ProjectManager.GetProjects();
            StringBuilder sbProjects = new StringBuilder("Проекты:\r\n");
            foreach (Project project in projects)
            {
                string currentProjectFlag = project.Id == session.ProjectId ? $" (текущий)" : "";
                sbProjects.Append($"[{project.UserCode}] <b>{project.Name}</b>{currentProjectFlag}\r\n");
            }
            return ExecuteCommandResult.CreateSuccessObject(sbProjects.ToString());
        }
    }
}
