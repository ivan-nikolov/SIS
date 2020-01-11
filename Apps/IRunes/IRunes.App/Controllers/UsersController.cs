namespace IRunes.App.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using IRunes.Models;
    using IRunes.Services;
    using SIS.MvcFramework;
    using SIS.MvcFramework.Attributes.Action;
    using SIS.MvcFramework.Attributes.Http;
    using SIS.MvcFramework.Results;

    public class UsersController : Controller
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        public ActionResult Login()
        {
            return this.View();
        }

        [HttpPost(ActionName = "Login")]
        public ActionResult LoginConfirm()
        {
            string username = ((ISet<string>)this.Request.FormData["username"]).FirstOrDefault();

            string password = ((ISet<string>)this.Request.FormData["password"]).FirstOrDefault();

            User user = this.userService.GetUserByUsernameAndPassword(username, this.HashPassword(password));

            if (user == null)
            {
                return this.Redirect("/Users/Login");
            }

            this.SignIn(user.Id, user.Username, user.Email);

            return this.Redirect("/");
        }

        public ActionResult Register()
        {
            return this.View();
        }

        [HttpPost(ActionName = "Register")]
        public ActionResult RegisterConfirm()
        {
            string username = ((ISet<string>)this.Request.FormData["username"]).FirstOrDefault();
            string password = ((ISet<string>)this.Request.FormData["password"]).FirstOrDefault();
            string confirmPassword = ((ISet<string>)this.Request.FormData["confirmPassword"]).FirstOrDefault();
            string email = ((ISet<string>)this.Request.FormData["email"]).FirstOrDefault();

            if (password != confirmPassword)
            {
                return this.Redirect("/Users/Register");
            }

            User user = new User()
            {
                Id = Guid.NewGuid().ToString(),
                Username = username,
                Password = this.HashPassword(password),
                Email = email
            };

            this.userService.CreateUser(user);

            return this.Redirect("/Users/Login");
        }

        public ActionResult Logout()
        {
            this.SignOut();

            return this.Redirect("/");
        }

        [NonAction]
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordHashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                string passwordHash = Encoding.UTF8.GetString(passwordHashBytes);

                return passwordHash;
            }
        }
    }
}
