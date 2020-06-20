using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.DictionaryEntities;
using LifeStream108.Libs.PostgreSqlHelper;
using System;
using System.Data;

namespace LifeStream108.Modules.DictionaryManagement
{
    public static class ProjectManager
    {
        private const string TableName = "todo_list.todo_categories";

        public static Project[] GetProjects()
        {
            return PostgreSqlCommandUtils.GetEntities($"select * from {TableName}", ReadProject);
        }

        public static Project GetProject(int projectId)
        {
            return PostgreSqlCommandUtils.GetEntity($"select * from {TableName} where id={projectId}", ReadProject);
        }

        public static Project GetProjectByCode(string code)
        {
            return PostgreSqlCommandUtils.GetEntity($"select * from {TableName} where user_code='{code}'", ReadProject);
        }

        private static Project ReadProject(IDataReader reader)
        {
            Project project = new Project();
            project.Id = PgsqlUtils.GetInt("id", reader);
            project.UserCode = PgsqlUtils.GetString("user_code", reader);
            project.Name = PgsqlUtils.GetString("name", reader);
            project.Description = PgsqlUtils.GetString("description", reader);
            project.HelpUrl = PgsqlUtils.GetString("help_url", reader);
            project.AssemblyName = PgsqlUtils.GetString("assembly_name", reader);
            project.AssemblyRootNamespace = PgsqlUtils.GetString("assembly_root_namespace", reader);
            project.RegTime = PgsqlUtils.GetDateTime("reg_time", reader, DateTime.MinValue);
            return project;
        }
    }
}
