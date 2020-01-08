using SIS.HTTP.Requests;
using SIS.HTTP.Responses;

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
