using System.Linq;
using LifeStream108.Libs.Entities;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.SessionEntities;

namespace LifeStream108.Modules.CommandProcessors
{
    internal class ChooseProjectProcessor : BaseCommandProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue projectTypeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.ProjectType);

            ProjectType projectType;
            switch (projectTypeParameter.Value.ToUpper())
            {
                case "LIFE":
                case "ЖИЗНЬ":
                    projectType = ProjectType.LifeActivity;
                    break;
                case "TODO":
                case "ТУДУ":
                    projectType = ProjectType.ToDo;
                    break;
                default:
                    return ExecuteCommandResult.CreateErrorObject("Неизвестный тип проекта: " + projectTypeParameter.Value);
            }

            ExecuteCommandResult commandResult = ExecuteCommandResult.CreateSuccessObject("Проект изменён");
            commandResult.ProjectType = projectType;
            return commandResult;
        }
    }
}
