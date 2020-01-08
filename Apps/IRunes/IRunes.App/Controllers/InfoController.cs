namespace IRunes.App.Controllers
{
    using System.Collections.Generic;
    using SIS.HTTP.Requests;
    using SIS.HTTP.Responses;
    using SIS.MvcFramework;
    using SIS.MvcFramework.Results;

    public class InfoController : Controller
    {
        public IHttpResponse About(IHttpRequest httpRequest)
        {
            return this.View();
        }
    }
}
