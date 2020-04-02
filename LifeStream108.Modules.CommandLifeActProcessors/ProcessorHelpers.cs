using LifeStream108.Libs.Common;
using LifeStream108.Libs.Common.Exceptions;
using LifeStream108.Libs.Common.Grammar;
using LifeStream108.Libs.Entities;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.DictionaryEntities;
using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.CommandProcessors;
using LifeStream108.Modules.DictionaryManagement.Managers;
using LifeStream108.Modules.LifeActivityManagement.Managers;
using System.Linq;
using System.Text;

namespace LifeStream108.Modules.CommandLifeActProcessors
{
    internal static class ProcessorHelpers
    {
        public static string GetFullLifeGroupName(int groupId, int parentGroupId, int userId)
        {
            LifeGroup[] allGroups = LifeGroupManager.GetGroupsForUser(userId);
            LifeGroupAtGroup[] allGroupAtGroups = LifeGroupAtGroupManager.GetGroupsAtGroupsForUser(userId);

            LifeGroupAtGroup groupAtGroup = allGroupAtGroups.FirstOrDefault(
                n => n.LifeGroupId == groupId && n.ParentLifeGroupId == parentGroupId);
            return GetFullLifeGroupName(groupAtGroup, allGroups, allGroupAtGroups);
        }

        public static string GetFullLifeGroupName(int groupAtGroupId, int userId)
        {
            LifeGroup[] allGroups = LifeGroupManager.GetGroupsForUser(userId);
            LifeGroupAtGroup[] allGroupAtGroups = LifeGroupAtGroupManager.GetGroupsAtGroupsForUser(userId);

            LifeGroupAtGroup groupAtGroup = allGroupAtGroups.FirstOrDefault(n => n.Id == groupAtGroupId);
            return GetFullLifeGroupName(groupAtGroup, allGroups, allGroupAtGroups);
        }

        private static string GetFullLifeGroupName(LifeGroupAtGroup groupAtGroup,
            LifeGroup[] allGroups, LifeGroupAtGroup[] allGroupAtGroups)
        {
            LifeGroupAtGroup currentGroupAtGroup = groupAtGroup;
            StringBuilder sbGroupPath = new StringBuilder();
            LifeGroup currentGroup = allGroups.FirstOrDefault(n => n.Id == currentGroupAtGroup.LifeGroupId);
            int counter = 0; // Insuranse for endless cycle
            do
            {
                sbGroupPath.Insert(0, currentGroup.NameForUser + "/");

                LifeGroup nextGroup = allGroups.FirstOrDefault(n => n.Id == currentGroupAtGroup.ParentLifeGroupId);
                currentGroupAtGroup = allGroupAtGroups.
                    FirstOrDefault(n => n.LifeGroupId == currentGroup.Id && n.ParentLifeGroupId == nextGroup.Id);
                currentGroup = nextGroup;
            }
            while (currentGroupAtGroup.ParentLifeGroupId != 0 || ++counter < 10);

            return sbGroupPath.ToString();
        }

        public static string PrintMeasure(double value, Measure measure)
        {
            if (!string.IsNullOrEmpty(measure.Declanation1)
                && !string.IsNullOrEmpty(measure.Declanation2)
                && !string.IsNullOrEmpty(measure.Declanation3)) return Declanations.DeclineByNumeral((int)value,
                    measure.Declanation1, measure.Declanation2, measure.Declanation3).ToLower();

            if (!string.IsNullOrEmpty(measure.ShortName)) return measure.ShortName.ToLower();

            return measure.Name.ToLower();
        }

        public static string GetFunction(CommandParameterAndValue functionParameter)
        {
            if (functionParameter == null || string.IsNullOrEmpty(functionParameter.Value)) return "";
            switch (functionParameter.Value.ToUpper())
            {
                case "SUM":
                case "СУММА":
                    return "sum";
                case "NO":
                case "НЕТ":
                    return "";
                default:
                    throw new LifeStream108Exception(ErrorType.UnknownFunction, $"Unknown function: <{functionParameter.Value}>",
                        $"Нереализованная функция \"{functionParameter.Value}\"", "");
            }
        }

        public static string FormatLifyActivityNames(LifeActivityParameter[] activityParameters)
        {
            return CollectionUtils.Array2String(activityParameters.Select(n => n.NameForUser), ", ", "\"", "\"");
        }

        public static int GetMeasureId(string measureName, int userId)
        {
            Measure measure = MeasureManager.GetMeasureByName(measureName, userId);
            if (measure == null)
            {
                measure = new Measure
                {
                    UserId = userId,
                    Name = measureName
                };
                MeasureManager.AddMeasure(measure);
            }
            return measure.Id;
        }

        public static DataType GetActivityParameterDataType(string dataTypeName)
        {
            return dataTypeName.ToUpper() == "ЧИСЛО" ? DataType.Double : DataType.Text;
        }

        public static string PrintLog(LifeActivityLog log, LifeActivityLogValue[] logValues,
            LifeActivity act, LifeActivityParameter[] actParams, Measure[] measures)
        {
            return $"{act.NameForUser}, {log.Period:dd.MM.yyyy}: {PrintLogValues(logValues, actParams, measures)}" +
                $"{(!string.IsNullOrEmpty(log.Comment) ? ", " + log.Comment : "")}";
        }

        public static string PrintLogValues(LifeActivityLogValue[] logValues, LifeActivityParameter[] activityParams, Measure[] measures)
        {
            StringBuilder sbValues = new StringBuilder();
            foreach (LifeActivityParameter param in activityParams)
            {
                LifeActivityLogValue logValue = logValues.FirstOrDefault(n => n.ActivityParamId == param.Id);
                if (logValue == null) continue;

                Measure measure = measures.FirstOrDefault(n => n.Id == param.MeasureId);

                bool isNumberValue = false;
                string valueString;
                switch (param.DataType)
                {
                    case DataType.Double:
                        valueString = DataFormatter.FormatNumber(logValue.NumericValue);
                        isNumberValue = true;
                        break;
                    default:
                        valueString = logValue.TextValue;
                        break;
                }
                sbValues.Append($"{valueString} {PrintMeasure(isNumberValue ? logValue.NumericValue : 0, measure)}, ");
            }
            return sbValues.Length > 2 ? sbValues.ToString(0, sbValues.Length - 2) : sbValues.ToString();
        }

        /*public static ExecuteCommandResult ChooseLifeGroup(string requestText, Session session)
        {
            string[] requestParts = requestText.Split(new[] { '=' });
            int groupId = int.Parse(requestParts[1]);
            LifeGroup foundGroup = LifeGroupManager.GetGroup(groupId, session.UserId);
            if (foundGroup == null) throw new LifeStream108Exception(ErrorType.LifeGroupNotFound,
                $"Life group with id <{groupId}> not found",
                "Не найдена такая группа", "");

            ExecuteCommandResult commandResult = new ExecuteCommandResult();
            commandResult.ResponseMessage = $"Выбрана группа \"{foundGroup.Name}\"";
            commandResult.LifeGroupId = foundGroup.Id;
            commandResult.Success = true;
            return commandResult;
        }*/
    }
}
