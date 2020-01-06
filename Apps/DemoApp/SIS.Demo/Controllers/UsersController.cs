using System.Linq;
using Demo.Data;
using Demo.Models;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;

namespace SIS.Demo.Controllers
{
    public class UsersController : BaseController
    {
        public UsersController(IHttpRequest httpRequest)
        {
            this.httpRequest = httpRequest;
        }

        public IHttpResponse Login(IHttpRequest httpRequest)
        {
            return this.View();
        }

        public IHttpResponse LoginConfirm(IHttpRequest httpRequest)
        {
            using (var context = new DemoDbContext())
            {
                string username = httpRequest.FormData["username"].ToString();
                string password = httpRequest.FormData["password"].ToString();

                var user = context.Users
                    .SingleOrDefault(u => u.Username == username && u.Password == password);

                if (user == null)
                {
                    return Redirect("/login");
                }

                httpRequest.Session.AddParameter("username", user.Username);
            }

            return Redirect("/home");
        }

        public IHttpResponse Register(IHttpRequest httpRequest)
        {
            return this.View();
        }

        public IHttpResponse RegisterConfirm(IHttpRequest httpRequest)
        {
            string username = httpRequest.FormData["username"].ToString();
            string password = httpRequest.FormData["password"].ToString();
            string confirmPassword = httpRequest.FormData["confirmPassword"].ToString();

            if (password != confirmPassword)
            {
                return Redirect("/register");
            }

            using (var context = new DemoDbContext())
            {

                User user = new User()
                {
                    Id = httpRequest.Session.Id,
                    Username = username,
                    Password = password
                };

                context.Users.Add(user);
                context.SaveChanges();
            }

            return Redirect("/login");
        }

        public IHttpResponse Logout(IHttpRequest httpRequest)
        {
            if (this.IsLoggedIn())
            {
                httpRequest.Session.ClearParameters();
            }

            return this.Redirect("/");
        }
    }
}
