﻿namespace IRunes.App.Controllers
{
    using IRunes.App.ViewModels;
    using SIS.MvcFramework;
    using SIS.MvcFramework.Attributes.Http;
    using SIS.MvcFramework.Results;

    public class HomeController : Controller
    {
        [HttpGet(Url = "/")]
        public ActionResult IndexSlash()
        {
            return Index();
        }

        public ActionResult Index()
        {
            if (this.IsLoggedIn())
            {
                return this.View( new UserHomeViewModel() { Username = this.User.Username },"Home");
            }

            return this.View();
        }
    }
}
