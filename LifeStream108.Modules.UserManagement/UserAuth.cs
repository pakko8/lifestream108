namespace LifeStream108.Modules.UserManagement
{
    internal class UserAuth
    {
        public virtual int UserId { get; set; }

        public virtual string Email { get; set; }

        public virtual string PasswordHash { get; set; }
    }
}
