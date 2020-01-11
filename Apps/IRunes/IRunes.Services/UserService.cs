namespace IRunes.Services
{
    using System.Linq;
    using IRunes.Data;
    using IRunes.Models;

    public class UserService : IUserService
    {
        private readonly RunesDbContext context;

        public UserService(RunesDbContext context)
        {
            this.context = context;
        }

        public User CreateUser(User user)
        {
            user = this.context.Users.Add(user).Entity;
            this.context.SaveChanges();

            return user;
        }

        public User GetUserByUsernameAndPassword(string username, string password)
        {
            return this.context.Users.SingleOrDefault(u => (u.Username == username || u.Email == username) && u.Password == password);
        }
    }
}
