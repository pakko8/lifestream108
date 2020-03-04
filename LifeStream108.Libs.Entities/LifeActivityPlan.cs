using System;

namespace LifeStream108.Libs.Entities
{
    public class LifeActivityPlan
    {
        public virtual int Id { get; set; }

        public virtual int UserId { get; set; }

        public virtual int LifeActivityId { get; set; }

        public virtual int Year { get; set; }

        public virtual int Month { get; set; }

        public virtual double MinPlanValue { get; set; }

        public virtual double MiddlePlanValue { get; set; }

        public virtual double MaxPlanValue { get; set; }

        /// <summary>
        /// Время уведомления пользователя, когда был достигнут минимальный план
        /// </summary>
        public virtual DateTime NotifyTimeMinPlan { get; set; }

        /// <summary>
        /// Время уведомления пользователя, когда был достигнут средний план
        /// </summary>
        public virtual DateTime NotifyTimeMiddlePlan { get; set; }

        /// <summary>
        /// Время уведомления пользователя, когда был достигнут высокий план
        /// </summary>
        public virtual DateTime NotifyTimeMaxPlan { get; set; }

        /// <summary>
        /// Время уведомления пользователя об итогах выполнения плана. Выдаётся в первый день месяца за предыдущий период
        /// </summary>
        public virtual DateTime NotifyTimeResume { get; set; }

        public virtual bool Active { get; set; }

        public virtual DateTime RegTime { get; set; }
    }
}
