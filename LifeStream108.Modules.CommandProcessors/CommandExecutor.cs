using LifeStream108.Libs.Common;
using LifeStream108.Libs.Common.Exceptions;
using LifeStream108.Libs.Common.Grammar;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.Entities.DictionaryEntities;
using LifeStream108.Libs.Entities.SessionEntities;
using LifeStream108.Modules.CommandManagement.Managers;
using LifeStream108.Modules.DictionaryManagement.Managers;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace LifeStream108.Modules.CommandProcessors
{
    public class CommandExecutor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private CommandName[] _commandNames = null;

        public ExecuteCommandResult Run(string requestText, Session session)
        {
            if (requestText.ToUpper().StartsWith("BACKMETHOD"))
            {
                // TODO
                return ExecuteCommandResult.CreateErrorObject("Запрос не поддерживается: " + requestText);
                /*
                int index = requestText.IndexOf(';');
                string methodName = requestText.Substring("BACKMETHOD=".Length, index - "BACKMETHOD=".Length);

                index = requestText.IndexOf('=');
                string paramsString = requestText.Substring(index + 1);
                string[] paramsArray = paramsString.Split(new[] { ';' });

                BackgroudCommandsProcessor backgroundProcessor = (BackgroudCommandsProcessor)LoadClass("BackgroudCommandsProcessor", requestText);
                MethodInfo method = backgroundProcessor.GetType().GetMethod(methodName);
                return (ExecuteCommandResult)method.Invoke(backgroundProcessor, paramsArray);
                */
            }

            _commandNames = CommandManager.GetCommandNamesForProject(session.ProjectId);

            var commandInfo = ParseRequest(requestText);
            if (!string.IsNullOrEmpty(commandInfo.Error)) return ExecuteCommandResult.CreateErrorObject(commandInfo.Error);

            string className = commandInfo.Command.ProcessorClassName;
            string assemblyName = null;
            string assemblyDirectory = null;
            if (commandInfo.Command.ProjectId != 0)
            {
                Project project = ProjectManager.GetProject(commandInfo.Command.ProjectId);
                className = project.AssemblyRootNamespace + "." + className;
                assemblyName = project.AssemblyName;
                assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
            
            BaseCommandProcessor commandProcessor = (BaseCommandProcessor)LoadClass(
                className, assemblyName, assemblyDirectory, requestText);
            ExecuteCommandResult executeResult = commandProcessor.Execute(commandInfo.Values, session);
            executeResult.CommandId = commandInfo.Command.Id;
            return executeResult;
        }

        private (Command Command, CommandParameterAndValue[] Values, string Error) ParseRequest(string requestText)
        {
            string[] requestParts = requestText.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            // Find command and it's parameters in database
            string commandName = requestParts[0].Trim();
            var commandInfo = FindCommand(commandName);
            if (commandInfo.Command == null) return (null, null, $"Команда \"{commandName}\" не найдена");

            CommandParameterAndValue[] resultParameters = null;
            if (requestParts.Length > 1)
            {
                string[] paramParts = requestParts[1].Split(new char[] { ';' });
                if (commandInfo.Parameters.Length == 0 && paramParts.Length > 0)
                    return (null, null, $"Команда \"{commandName}\" не требует параметров");

                int countRequiredParams = commandInfo.Parameters.Count(n => n.Required);
                if (paramParts.Length < countRequiredParams)
                    return (null, null,
                        $"Эта команда требует минимум {countRequiredParams} " +
                        $"{Declanations.DeclineByNumeral(countRequiredParams, "параметр", "параметра", "параметров")}. " +
                        PrepareHelpForCommand(commandInfo.Command, commandInfo.Parameters));
                // Create parameters
                resultParameters = new CommandParameterAndValue[paramParts.Length];
                List<string> checkParametersErrorList = new List<string>();
                for (int paramIndex = 0; paramIndex < paramParts.Length; paramIndex++)
                {
                    CommandParameter cmdParam = commandInfo.Parameters[paramIndex];
                    string paramValue = paramParts[paramIndex].Trim();

                    string errorText = CheckParameterValue(paramValue, cmdParam, out paramValue);
                    if (!string.IsNullOrEmpty(errorText)) checkParametersErrorList.Add(errorText);

                    if (checkParametersErrorList.Count == 0)
                    {
                        resultParameters[paramIndex] = new CommandParameterAndValue
                        {
                            Parameter = cmdParam,
                            Value = paramValue
                        };
                    }
                }

                if (checkParametersErrorList.Count > 0)
                {
                    string error = CollectionUtils.Array2String(checkParametersErrorList, "\r\n");
                    return (null, null, error);
                }
            }
            else
            {
                if (commandInfo.Parameters.Length > 0)
                    return (null, null, $"Команда \"{commandName}\" требует параметров. " +
                        PrepareHelpForCommand(commandInfo.Command, commandInfo.Parameters));
            }

            return (commandInfo.Command, resultParameters, "");
        }

        private static string CheckParameterValue(string paramValue, CommandParameter cmdParam, out string paramValueAdj)
        {
            paramValueAdj = paramValue;

            if (string.IsNullOrEmpty(paramValueAdj))
            {
                if (!cmdParam.Required)
                {
                    paramValueAdj = cmdParam.DefaultValue;
                    return "";
                }
                else
                    return $"Для параметра \"{cmdParam.Name}\" не указано значение";
            }

            if (cmdParam.MinLength > 0 && paramValueAdj.Length < cmdParam.MinLength)
                return $"Для параметра \"{cmdParam.Name}\" минимальная длина {cmdParam.MinLength} {Declanations.DeclineByNumeral(cmdParam.MinLength, "символ", "символа", "символов")}";

            if (cmdParam.MaxLength > 0 && paramValueAdj.Length > cmdParam.MaxLength)
                return $"Для параметра \"{cmdParam.Name}\" максимальная длина {cmdParam.MinLength} {Declanations.DeclineByNumeral(cmdParam.MaxLength, "символ", "символа", "символов")}";

            if (!string.IsNullOrEmpty(cmdParam.Regex) && !Regex.IsMatch(paramValueAdj, cmdParam.Regex))
            {
                if (!string.IsNullOrEmpty(cmdParam.DataFormatDescription))
                {
                    string example = !string.IsNullOrEmpty(cmdParam.Example) ? $". Пример: {cmdParam.Example}" : "";
                    return $"Значение параметра \"{cmdParam.Name}\" должно быть указано в формате: {cmdParam.DataFormatDescription}{example}";
                }
                else
                    return $"Значение параметра \"{cmdParam.Name}\" указано в неверном формате";
            }

            if (cmdParam.InputDataType == InputDataType.OneElementFromList)
            {
                string[] allowedValues = cmdParam.PredefinedValuesList;
                if (!allowedValues.Contains(paramValueAdj.ToUpper()))
                    return $"Значение параметра \"{cmdParam.Name}\" может прининить одно из следующих значений: {cmdParam.PredefinedValues}";
            }

            return null;
        }

        private (Command Command, CommandParameter[] Parameters) FindCommand(string commandName)
        {
            string commandNameAdj = commandName.Replace(" ", "").ToUpper();

            var foundNames = _commandNames.Where(n => n.Alias.ToUpper() == commandNameAdj).ToArray();
            if (foundNames.Length > 0)
            {
                var uniqueCommandIds = foundNames.Select(n => n.CommandId).Distinct();
                if (uniqueCommandIds.Count() > 1)
                    throw new LifeStream108Exception(
                        ErrorType.CommandNameReferMoreThanOneCommands,
                        $"For command name <{commandName}> found more than one command",
                        "Для этой команды не найдено точного соответствие.",
                        $"Ошибка во время выполнения команды '{commandName}'");
                var foundCommand = CommandManager.GetCommand(uniqueCommandIds.First());
                if (foundCommand.Command == null)
                    throw new LifeStream108Exception(
                        ErrorType.CommandNameHasNoCommand,
                        $"Command with id <{uniqueCommandIds.First()}> not found",
                        "Эта команда неверно настроена.",
                        $"Ошибка во время выполнения команды '{commandName}'");

                return foundCommand;
            }

            return (null, null);
        }

        public static string PrepareHelpForCommand(Command command, CommandParameter[] parameters)
        {
            CommandName[] commandNames = CommandManager.GetCommandNames(command.Id);
            if (commandNames.Length == 0) throw new LifeStream108Exception(ErrorType.CommandHasNoCommandNames,
                $"Command with id <{command.Id}> has no command names",
                "Эта команда неверно настроена", "");

            commandNames = commandNames.OrderBy(n => n.SortOrder).ToArray();
            StringBuilder sbResult = new StringBuilder();
            sbResult.Append($"Синтаксис команды:\r\n<b>{commandNames[0].GetReadableAias()}");
            if (parameters != null && parameters.Length > 0)
            {
                sbResult.Append(" : ");
                for (int i = 0; i < parameters.Length; i++)
                {
                    CommandParameter parameter = parameters[i];
                    sbResult.Append(parameter.Name);
                    if (!string.IsNullOrEmpty(parameter.DataFormat)) sbResult.Append($" ({parameter.DataFormat})");
                    if (i < parameters.Length - 1) sbResult.Append(" ; ");
                }
            }
            sbResult.Append("</b>");
            return sbResult.ToString();
        }

        private object LoadClass(string className, string assemblyName, string assemblyPath, string requestText)
        {
            try
            {
                if (!string.IsNullOrEmpty(assemblyName))
                    return ReflectionUtils.LoadClass(className, assemblyName, assemblyPath);

                // Load class from this assembly
                Assembly thisAssembly = Assembly.GetAssembly(GetType());
                Type handlerType = thisAssembly.GetType(thisAssembly.GetName().Name + "." + className, true, true);
                return Activator.CreateInstance(handlerType);
            }
            catch (Exception ex)
            {
                Logger.Error("Error to load class: " + ex);
                throw new LifeStream108Exception(ErrorType.LoadCommandProcessorClassError, "", "Ошибка обработки запроса", requestText);
            }
        }
    }
}
