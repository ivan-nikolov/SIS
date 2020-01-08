﻿namespace SIS.MvcFramework
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Requests;
    using SIS.MvcFramework.Extensions;
    using SIS.MvcFramework.Identity;
    using SIS.MvcFramework.Results;

    public abstract class Controller
    {
        protected Dictionary<string, object> ViewData;

        protected Controller()
        {
            this.ViewData = new Dictionary<string, object>();
        }

        protected Principal User => this.Request.Session.GetParameter("principal") as Principal;

        public IHttpRequest Request { get; set; }

        protected ActionResult View([CallerMemberName] string view = null)
        {
            string controllerName = this.GetType().Name.Replace("Controller", string.Empty);
            string viewName = view;

            string viewContent = System.IO.File.ReadAllText("Views/" + controllerName + "/" + viewName + ".html");

            viewContent = this.ParseTemplate(viewContent);

            HtmlResult htmlResult = new HtmlResult(viewContent, HttpResponseStatusCode.Ok);

            return htmlResult;
        }

        protected void SignIn(string id, string username, string email)
        {
            this.Request.Session.AddParameter("principal", new Principal
            {
                Id = id,
                Username = username,
                Email = email
            });
        }

        protected void SignOut()
        {
            this.Request.Session.ClearParameters();
        }

        protected bool IsLoggedIn()
        {
            return this.User != null;
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

        protected ActionResult NotFound(string message = "")
        {
            return new NotFoundResult(message);
        }
    }
}
