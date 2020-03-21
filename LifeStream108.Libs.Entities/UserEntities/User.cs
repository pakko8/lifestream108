using System;

namespace LifeStream108.Libs.Entities.UserEntities
{
    public class User
    {
        public virtual int Id { get; set; }

        public virtual string Email { get; set; }

        public virtual string Name { get; set; }

        public virtual int TelegramId { get; set; }

        public virtual int LanguageId { get; set; }

        public virtual int CurrencyId { get; set; }

        public virtual bool Superuser { get; set; }

        public virtual UserStatus Status { get; set; }

        public virtual DateTime RegTime { get; set; }

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
