using System;

namespace LifeStream108.Libs.Entities.LifeActityEntities
{
    public class LifeActivityPlan
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int LifeActivityId { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public double MinPlanValue { get; set; }

        public double MiddlePlanValue { get; set; }

        public double MaxPlanValue { get; set; }

        /// <summary>
        /// Время уведомления пользователя, когда был достигнут минимальный план
        /// </summary>
        public DateTime NotifyTimeMinPlan { get; set; }

        /// <summary>
        /// Время уведомления пользователя, когда был достигнут средний план
        /// </summary>
        public DateTime NotifyTimeMiddlePlan { get; set; }

        /// <summary>
        /// Время уведомления пользователя, когда был достигнут высокий план
        /// </summary>
        public DateTime NotifyTimeMaxPlan { get; set; }

        /// <summary>
        /// Время уведомления пользователя об итогах выполнения плана. Выдаётся в первый день месяца за предыдущий период
        /// </summary>
        public DateTime NotifyTimeResume { get; set; }

        public bool Active { get; set; }

        public DateTime RegTime { get; set; }
    }
}
