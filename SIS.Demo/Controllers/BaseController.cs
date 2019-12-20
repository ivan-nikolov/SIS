namespace SIS.Demo.Controllers
{
    using System.IO;
    using System.Runtime.CompilerServices;

    using HTTP.Enums;
    using HTTP.Responses.Contracts;
    using WebServer.Results;

    public class BaseController
    {
        public IHttpResponse View([CallerMemberName] string view = null)
        {
            string controllerName = this.GetType().Name.Replace("Controller", string.Empty);
            string viewName = view;

            string viewContent = File.ReadAllText("Views/" + controllerName + "/" + viewName + ".html");

            return new HtmlResult(viewContent, HttpResponseStatusCode.Ok);
        }
    }
}
