using SIS.HTTP.Enums;
using SIS.HTTP.Requests.Contracts;
using SIS.HTTP.Responses.Contracts;
using SIS.WebServer.Results;

namespace SIS.Demo.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IHttpRequest httpRequest)
        {
            this.httpRequest = httpRequest;
        }

        public IHttpResponse Index(IHttpRequest request)
        {
            return this.View();
        }

        public IHttpResponse Home(IHttpRequest httpRequest)
        {
            if (!this.IsLoggedIn())
            {
                return this.Redirect("login");
            }

            this.ViewData["Username"] = this.httpRequest.Session.GetParameter("username");

            return this.View();
        }
    }
}
