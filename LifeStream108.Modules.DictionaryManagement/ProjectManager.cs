using LifeStream108.Libs.Common;
using LifeStream108.Libs.Entities.DictionaryEntities;
using LifeStream108.Modules.SettingsManagement;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;

namespace LifeStream108.Modules.DictionaryManagement
{
    public static class ProjectManager
    {
        public static Project[] GetProjects()
        {
            List<Project> projects = new List<Project>();
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var command = connection.CreateCommand();
                command.CommandText = $"select * from projects";
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        projects.Add(ReadProject(reader));
                    }
                }
            }
            return projects.ToArray();
        }

        public static Project GetProject(int projectId)
        {
            Project project = null;
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var command = connection.CreateCommand();
                command.CommandText = $"select * from projects where id={projectId}";
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        project = ReadProject(reader);
                    }
                }
            }
            return project;
        }

        public static Project GetProjectByCode(string code)
        {
            Project project = null;
            using (var connection = new NpgsqlConnection(SettingsManager.GetSettingEntryByCode(SettingCode.MainDbConnString).Value))
            {
                var command = connection.CreateCommand();
                command.CommandText = $"select * from projects where user_code='{code}'";
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        project = ReadProject(reader);
                    }
                }
            }
            return project;
        }

        private static Project ReadProject(IDataReader reader)
        {
            Project project = new Project();
            project.Id = PgsqlUtils.GetInt("id", reader, 0);
            project.UserCode = PgsqlUtils.GetString("user_code", reader, "");
            project.Name = PgsqlUtils.GetString("name", reader, "");
            project.Description = PgsqlUtils.GetString("description", reader, "");
            project.HelpUrl = PgsqlUtils.GetString("help_url", reader, "");
            project.AssemblyName = PgsqlUtils.GetString("assembly_name", reader, "");
            project.AssemblyRootNamespace = PgsqlUtils.GetString("assembly_root_namespace", reader, "");
            project.RegTime = PgsqlUtils.GetDateTime("reg_time", reader, DateTime.MinValue);
            return project;
        }
    }
}
