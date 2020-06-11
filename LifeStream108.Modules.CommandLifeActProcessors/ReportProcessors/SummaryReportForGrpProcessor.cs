using System;
using System.Linq;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Modules.CommandProcessors;
using LifeStream108.Modules.LifeActivityManagement.Managers;

namespace LifeStream108.Modules.CommandLifeActProcessors.ReportProcessors
{
    public class SummaryReportForGrpProcessor : ReportBaseProcessor
    {
        public override ExecuteCommandResult Execute(CommandParameterAndValue[] commandParameters, Session session)
        {
            CommandParameterAndValue periodFromParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.Period);

            CommandParameterAndValue grpCodeParameter = commandParameters.FirstOrDefault(
                n => n.Parameter.ParameterCode == CommandParameterCode.LifeGroupCode);

            LifeGroup group = LifeGroupManager.GetGroupByCode(grpCodeParameter.IntValue, session.UserId);
            if (group == null)
                return ExecuteCommandResult.CreateErrorObject($"Группа с кодом {grpCodeParameter.IntValue} не найдена");

            throw new NotImplementedException();
        }
    }
}
