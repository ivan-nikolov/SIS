namespace SIS.Demo.Controllers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;

    using HTTP.Enums;
    using SIS.HTTP.Requests;
    using SIS.HTTP.Responses;
    using WebServer.Results;

    public class BaseController
    {
        protected IHttpRequest httpRequest { get; set; }

        protected Dictionary<string, object> ViewData = new Dictionary<string, object>();

        public IHttpResponse View([CallerMemberName] string view = null)
        {
            string controllerName = this.GetType().Name.Replace("Controller", string.Empty);
            string viewName = view;

            string viewContent = File.ReadAllText("Views/" + controllerName + "/" + viewName + ".html");

            viewContent = this.ParseTemplate(viewContent);

            HtmlResult htmlResult = new HtmlResult(viewContent, HttpResponseStatusCode.Ok);

            return htmlResult;
        }

        protected bool IsLoggedIn()
        {
            return this.httpRequest.Session.ContainsParameter("username");
        }

        private string ParseTemplate(string viewContent)
        {
            foreach (var kvp in this.ViewData)
            {
                viewContent = viewContent.Replace($"@Model.{kvp.Key.ToLower()}", kvp.Value.ToString());
            }

            return viewContent;
        }

        public IHttpResponse Redirect(string url)
        {
            return new RedirectResult(url);
        }
    }
}
