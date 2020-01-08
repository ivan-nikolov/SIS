namespace SIS.MvcFramework
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Xml.Serialization;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Requests;
    using SIS.HTTP.Responses;
    using SIS.MvcFramework.Extensions;
    using SIS.MvcFramework.Results;

    public abstract class Controller
    {
        protected Dictionary<string, object> ViewData;

        protected Controller()
        {
            this.ViewData = new Dictionary<string, object>();
        }

        protected ActionResult View([CallerMemberName] string view = null)
        {
            string controllerName = this.GetType().Name.Replace("Controller", string.Empty);
            string viewName = view;

            string viewContent = System.IO.File.ReadAllText("Views/" + controllerName + "/" + viewName + ".html");

            viewContent = this.ParseTemplate(viewContent);

            HtmlResult htmlResult = new HtmlResult(viewContent, HttpResponseStatusCode.Ok);

            return htmlResult;
        }

        protected void SignIn(IHttpRequest httpRequest, string id, string username, string email)
        {
            httpRequest.Session.AddParameter("id", id);
            httpRequest.Session.AddParameter("username", username);
            httpRequest.Session.AddParameter("email", email);
        }

        protected void SignOut(IHttpRequest httpRequest)
        {
            httpRequest.Session.ClearParameters();
        }

        protected bool IsLoggedIn(IHttpRequest httpRequest)
        {
            return httpRequest.Session.ContainsParameter("username");
        }

        private string ParseTemplate(string viewContent)
        {
            foreach (var kvp in this.ViewData)
            {
                viewContent = viewContent.Replace($"@Model.{kvp.Key}", kvp.Value.ToString());
            }

            return viewContent;
        }

        protected ActionResult Redirect(string url)
        {
            return new RedirectResult(url);
        }

        protected ActionResult Xml(object param)
        {
            return new XmlResult(param.ToXml());
        }

        protected ActionResult Json(object param)
        {
            return new JsonResult(param.ToJson());
        }

        protected ActionResult File(byte[] content)
        {
            return new FileResult(content);
        }
    }
}
