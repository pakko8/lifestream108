using System;

namespace LifeStream108.Libs.Entities.UserEntities
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public bool Superuser { get; set; }

        public int TelegramId { get; set; }

        public int LanguageId { get; set; }

        public int CurrencyId { get; set; }

        public int DefaultProjectId { get; set; }

        public DateTime CheckActLogsTime { get; set; }

        public UserStatus Status { get; set; }

        public DateTime RegTime { get; set; }

        public override string ToString()
        {
            return $"User: TelegramId={TelegramId}, Name={Name}";
        }
    }

    public enum UserStatus
    {
        Active,

        Paused,

        Bloked,

        Deleted
    }

    public static class UserStatusExtensions
    {
        public static string GetDescriptiveString(this UserStatus status)
        {
            switch (status)
            {
                case UserStatus.Active: return "Активен";
                case UserStatus.Paused: return "Приостановлен";
                case UserStatus.Bloked: return "Заблокирован";
                case UserStatus.Deleted: return "Удалён";
                default: return "Статус неизвестен";
            }
        }
    }
}
