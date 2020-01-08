namespace IRunes.App.Controllers
{
    using SIS.HTTP.Requests;
    using SIS.HTTP.Responses;
    using SIS.MvcFramework;
    using SIS.MvcFramework.Attributes.Http;

    public class HomeController : Controller
    {
        [HttpGet(Url = "/")]
        public IHttpResponse IndexSlash(IHttpRequest httpRequest)
        {
            return Index(httpRequest);
        }

        public IHttpResponse Index(IHttpRequest httpRequest)
        {
            if (this.IsLoggedIn(httpRequest))
            {
                this.ViewData["Username"] = httpRequest.Session.GetParameter("username");

                return this.View("Home");
            }

            return this.View();
        }
    }
}
