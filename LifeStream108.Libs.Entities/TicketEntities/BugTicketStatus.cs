namespace LifeStream108.Libs.Entities.TicketEntities
{
    public enum BugTicketStatus
    {
        /// <summary>
        /// Ошибка зарегистрирована
        /// </summary>
        New = 0,

        /// <summary>
        /// Ошибка в процессе исправления
        /// </summary>
        InProgress = 1,

        /// <summary>
        /// Ошибка исправлена
        /// </summary>
        Fixed = 2,

        /// <summary>
        /// Уведомление об исправлении ошибки отправлено пользователю
        /// </summary>
        NotificationSent = 3
    }
}
