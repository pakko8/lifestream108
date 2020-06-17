using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities;
using LifeStream108.Libs.Entities.CommandEntities;
using LifeStream108.Libs.PostgreSqlHelper;
using LifeStream108.Modules.SettingsManagement;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace LifeStream108.Modules.CommandManagement
{
    public static class CommandManager
    {
        private const string TableNameCommands = "commands";
        private const string TableNameCommandNames = "command_names";
        private const string TableNameCommandParams = "command_params";

        public static Command[] GetCommands(int projectId)
        {
            return PostgreSqlCommandUtils.GetEntities($"select * from {TableNameCommands} where project_id in (0, {projectId})", ReadCommand);
        }

        public static CommandName[] GetCommandNamesForProject(int projectId)
        {
            string query =
                $@"select nm.* from {TableNameCommandNames} as nm
                inner join commands cmd on nm.command_id=cmd.id
                where cmd.project_id in (0, {projectId})";
            return PostgreSqlCommandUtils.GetEntities(query, ReadCommandName);
        }

        public static CommandName[] GetCommandNames(int commandId)
        {
            return PostgreSqlCommandUtils.GetEntities($"select * from {TableNameCommandNames} where command_id={commandId}", ReadCommandName);
        }

        public static (Command Command, CommandParameter[] Parameters) GetCommand(int commandId)
        {
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                Command command = GetCommand(commandId, connection);
                CommandParameter[] parameters = command != null ? GetParametersForCommand(command.Id, connection) : null;
                return (command, parameters);
            }
        }

        private static Command GetCommand(int id, DbConnection connection)
        {
            return PostgreSqlCommandUtils.GetEntity($"select * from {TableNameCommands} where id={id}", ReadCommand);
        }

        private static CommandParameter[] GetParametersForCommand(int commandId, DbConnection connection)
        {
            return PostgreSqlCommandUtils.GetEntities(
                $"select * from {TableNameCommandParams} where command_id={commandId}", ReadCommandParameter);
        }

        private static Command ReadCommand(IDataReader reader)
        {
            Command command = new Command();
            command.Id = PgsqlUtils.GetInt("id", reader, 0);
            command.SortOrder = PgsqlUtils.GetInt("sort_order", reader, 0);
            command.ProjectId = PgsqlUtils.GetInt("project_id", reader, 0);
            command.EntityType = (EntityType)PgsqlUtils.GetEnum("entity_type", reader, typeof(EntityType), EntityType.NoEntity);
            command.Name = PgsqlUtils.GetString("name", reader, "");
            command.Description = PgsqlUtils.GetString("description", reader, "");
            command.ProcessorClassName = PgsqlUtils.GetString("processor_class_name", reader, "");
            command.Active = PgsqlUtils.GetBoolean("active", reader, false);
            return command;
        }

        private static CommandName ReadCommandName(IDataReader reader)
        {
            CommandName commandName = new CommandName();
            commandName.Id = PgsqlUtils.GetInt("id", reader, 0);
            commandName.SortOrder = PgsqlUtils.GetInt("sort_order", reader, 0);
            commandName.CommandId = PgsqlUtils.GetInt("command_id", reader, 0);
            commandName.Alias = PgsqlUtils.GetString("alias", reader, "");
            commandName.SpacePositions = PgsqlUtils.GetString("space_positions", reader, "");
            commandName.LanguageId = PgsqlUtils.GetInt("language_id", reader, 0);
            return commandName;
        }

        private static CommandParameter ReadCommandParameter(IDataReader reader)
        {
            CommandParameter param = new CommandParameter();
            param.Id = PgsqlUtils.GetInt("id", reader, 0);
            param.CommandId = PgsqlUtils.GetInt("command_id", reader, 0);
            param.SortOrder = PgsqlUtils.GetInt("sort_order", reader, 0);
            param.ParameterCode = (CommandParameterCode)PgsqlUtils.GetEnum("command_param_code", reader, typeof(CommandParameterCode), CommandParameterCode.None);
            param.Name = PgsqlUtils.GetString("name", reader, "");
            param.Description = PgsqlUtils.GetString("description", reader, "");
            param.DataType = (DataType)PgsqlUtils.GetEnum("data_type", reader, typeof(DataType), DataType.Text);
            param.InputDataType = (InputDataType)PgsqlUtils.GetEnum("input_data_type", reader, typeof(InputDataType), InputDataType.Simple);
            param.DataFormat = PgsqlUtils.GetString("data_format", reader, "");
            param.DataFormatDescription = PgsqlUtils.GetString("data_format_desc", reader,"");
            param.Required = PgsqlUtils.GetBoolean("required", reader, false);
            param.MinLength = PgsqlUtils.GetInt("min_length", reader, 0);
            param.MaxLength = PgsqlUtils.GetInt("max_length", reader, 0);
            param.Regex = PgsqlUtils.GetString("regex", reader, "");
            param.PredefinedValues = PgsqlUtils.GetString("predefined_values", reader, "");
            param.DefaultValue = PgsqlUtils.GetString("default_value", reader, "");
            param.Example = PgsqlUtils.GetString("example", reader, "");
            return param;
        }
    }
}
