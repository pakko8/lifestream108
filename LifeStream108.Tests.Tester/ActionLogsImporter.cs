using LifeStream108.Libs.Entities.LifeActityEntities;
using LifeStream108.Modules.LifeActivityManagement;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace LifeStream108.Tests.Tester
{
    public static class ActionLogsImporter
    {
        public static void Run()
        {
            int userId = 1;
            int lineNumber = 1;
            try
            {
                LifeActivity[] allActivities = LifeActivityManager.GetActivitiesForUser(userId);
                LifeActivityParameter[] allActivityParameters = LifeActivityParameterManager.GetParametersForUser(userId);

                TextReader fileReader = new StreamReader(@"c:\_projects\kirtan_result.txt");
                string line;
                while ((line = fileReader.ReadLine()) != null)
                {
                    line = line.Replace("log:", "");
                    string[] lineParts = line.Split(new[] { ';' });

                    int activityId = int.Parse(lineParts[0]);
                    LifeActivity activity = allActivities.FirstOrDefault(n => n.Id == activityId);
                    if (activity == null) throw new Exception($"Activity with id {activityId} not found");
                    LifeActivityParameter[] activityParameters = allActivityParameters.Where(n => n.ActivityId == activity.Id).ToArray();

                    double[] valuesAdj = lineParts[2].Split(new[] { '+' }).Select(n => double.Parse(n, new CultureInfo("en-US"))).ToArray();
                    if (valuesAdj.Length != activityParameters.Length) throw new Exception(
                        $"{line}. Activity {activity.Id} has {activityParameters.Length} parameters, but the're {valuesAdj.Length} values");

                    LifeActivityLog newLog = new LifeActivityLog();
                    newLog.UserId = userId;
                    newLog.LifeActivityId = activity.Id;
                    newLog.Period = DateTime.ParseExact(lineParts[1], "dd.MM.yyyy", null);
                    newLog.Comment = lineParts.Length > 3 ? lineParts[3] : "";

                    LifeActivityLogWithValues[] existingLogs = LifeActivityLogManager.GetLogsForDate(activity.Id, newLog.Period, userId);
                    if (existingLogs.Length > 0) throw new Exception($"Value {line} already exists");

                    activityParameters = activityParameters.OrderBy(n => n.SortOrder).ToArray();
                    LifeActivityLogValue[] valueList = new LifeActivityLogValue[activityParameters.Length];
                    for (int i = 0; i < activityParameters.Length; i++)
                    {
                        LifeActivityLogValue newValue = new LifeActivityLogValue();
                        newValue.UserId = userId;
                        newValue.ActivityParamId = activityParameters[i].Id;
                        newValue.Period = newLog.Period;
                        newValue.NumericValue = valuesAdj[i];
                        valueList[i] = newValue;
                    }

                    LifeActivityLogManager.AddLog(newLog, valueList.ToArray());
                    lineNumber++;
                }
                fileReader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Count saved lines: " + lineNumber);
                Console.WriteLine(ex);
            }
            Console.ReadLine();
        }
    }
}
