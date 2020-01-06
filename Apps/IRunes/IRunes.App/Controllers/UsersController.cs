using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using IRunes.Data;
using IRunes.Models;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;

namespace IRunes.App.Controllers
{
    public class UsersController : BaseController
    {
        public IHttpResponse Login(IHttpRequest httpRequest)
        {
            return this.View();
        }

        public IHttpResponse LoginConfirm(IHttpRequest httpRequest)
        {
            using (var context = new RunesDbContext())
            {
                string username = ((ISet<string>)httpRequest.FormData["username"]).FirstOrDefault();

                string password = ((ISet<string>)httpRequest.FormData["password"]).FirstOrDefault();

                User user = context.Users
                    .SingleOrDefault(u => (u.Username == username || u.Email == username)
                    && u.Password == this.HashPassword(password));

                if (user == null)
                {
                    return this.Redirect("/Users/Login");
                }

                this.SignIn(httpRequest, user);
            }


            return this.Redirect("/");
        }

        public IHttpResponse Register(IHttpRequest httpRequest)
        {
            return this.View();
        }

        public IHttpResponse RegisterConfirm(IHttpRequest httpRequest)
        {
            string username = ((ISet<string>)httpRequest.FormData["username"]).FirstOrDefault();
            string password = ((ISet<string>)httpRequest.FormData["password"]).FirstOrDefault();
            string confirmPassword = ((ISet<string>)httpRequest.FormData["confirmPassword"]).FirstOrDefault();
            string email = ((ISet<string>)httpRequest.FormData["email"]).FirstOrDefault();

            if (password != confirmPassword)
            {
                return this.Redirect("/Users/Register");
            }

            User user = new User()
            {
                Id =  Guid.NewGuid().ToString(),
                Username = username,
                Password = this.HashPassword(password),
                Email = email
            };

            using (var context = new RunesDbContext())
            {
                context.Users.Add(user);
                context.SaveChanges();
            }

            return this.Redirect("/Users/Login");
        }

        public IHttpResponse Logout(IHttpRequest httpRequest)
        {
            this.SignOut(httpRequest);

            return this.Redirect("/");
        }

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
