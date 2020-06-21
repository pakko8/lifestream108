using System.Linq;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.DictionaryEntities;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Modules.DictionaryManagement;

namespace LifeStream108.Modules.CommandProcessors
{
    internal class ChooseProjectProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue projectCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.ProjectCode);

            Project project = ProjectManager.GetProjectByCode(projectCodeParameter.Value);
            if (project == null) return ExecuteCommandResult.CreateErrorObject(
                $"Проект с кодом '{projectCodeParameter.Value}' не найден");

            ExecuteCommandResult commandResult =
                ExecuteCommandResult.CreateSuccessObject($"Проект '{project.Name}' выбран");
            commandResult.ProjectId = project.Id;
            return commandResult;
        }
    }
}
