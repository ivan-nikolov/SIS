﻿namespace SIS.MvcFramework
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Requests;
    using SIS.MvcFramework.Extensions;
    using SIS.MvcFramework.Identity;
    using SIS.MvcFramework.Results;
    using SIS.MvcFramework.ViewEngine;

    public abstract class Controller
    {
        private readonly IViewEngine viewEngine;

        protected Controller()
        {
            this.viewEngine = new SisViewEngine();
        }

        public Principal User => this.Request.Session.ContainsParameter("principal") 
            ? this.Request.Session.GetParameter("principal") as Principal
            : null;

        public IHttpRequest Request { get; set; }

        protected ActionResult View([CallerMemberName] string view = null)
        {
            return this.View<object>(null, view);
        }

        protected ActionResult View<T>( T model, [CallerMemberName] string view = null)
            where T : class
        {
            string controllerName = this.GetType().Name.Replace("Controller", string.Empty);
            string viewName = view;

            string viewContent = System.IO.File.ReadAllText("Views/" + controllerName + "/" + viewName + ".html");
            viewContent = this.viewEngine.GetHtml(viewContent, model, this.User);

            string layoutContent = System.IO.File.ReadAllText("Views/_Layout.html");
            layoutContent = this.viewEngine.GetHtml(layoutContent, model, this.User);

            layoutContent = layoutContent.Replace("@RenderBody()", viewContent);

            HtmlResult htmlResult = new HtmlResult(layoutContent, HttpResponseStatusCode.Ok);

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
            return this.Request.Session.ContainsParameter("principal");
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

        //private string ParseTemplate(string viewContent)
        //{
        //    foreach (var kvp in this.ViewData)
        //    {
        //        viewContent = viewContent.Replace($"@Model.{kvp.Key}", kvp.Value.ToString());
        //    }

        //    return viewContent;
        //}
    }
}
