﻿using SIS.HTTP.Enums;
using SIS.HTTP.Requests.Contracts;
using SIS.HTTP.Responses.Contracts;
using SIS.WebServer.Results;

namespace SIS.Demo.Controllers
{
    public class HomeController : BaseController
    {
        public IHttpResponse Home(IHttpRequest request)
        {
            //string content = @"<h1>Hello World</h1>";

            //return new HtmlResult(content, HttpResponseStatusCode.Ok);

            return this.View();
        }
    }
}
