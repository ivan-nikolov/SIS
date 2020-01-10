﻿namespace SIS.MvcFramework
{
    using System;
    using System.Linq;
    using System.Reflection;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Responses;
    using SIS.MvcFramework.Attributes.Action;
    using SIS.MvcFramework.Attributes.Http;
    using SIS.MvcFramework.Attributes.Security;
    using SIS.MvcFramework.Results;
    using SIS.MvcFramework.Routing;
    using SIS.MvcFramework.Sessions;

    public static class WebHost
    {
        public static void Start(IMvcApplication application)
        {
            IServerRoutingTable serverRoutingTable = new ServerRoutingTable();
            IHttpSessionStorage httpSessionStorage = new HttpSessionStorage();

            AutoRegisterRoutes(application, serverRoutingTable);

            application.ConfigureServices();

            application.Configure(serverRoutingTable);

            Server server = new Server(8000, serverRoutingTable, httpSessionStorage);
            server.Run();
        }

        private static void AutoRegisterRoutes(IMvcApplication application, IServerRoutingTable serverRoutingTable)
        {
            var controllers = application.GetType().Assembly
                .GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract
                   && typeof(Controller).IsAssignableFrom(type));

            foreach (var controller in controllers)
            {
                var actions = controller
                    .GetMethods(BindingFlags.DeclaredOnly
                    | BindingFlags.Instance
                    | BindingFlags.Public)
                    .Where(x => !x.IsSpecialName
                        && x.DeclaringType == controller)
                    .Where(x => x.GetCustomAttributes().All(a => a.GetType() != typeof(NonActionAttribute)));

                foreach (var action in actions)
                {
                    var attribute = action.GetCustomAttributes() 
                        .Where(x => x.GetType().IsSubclassOf(typeof(BaseHttpAttribute))).LastOrDefault() as BaseHttpAttribute;

                    var path = $"/{controller.Name.Replace("Controller", string.Empty)}/{action.Name}";
                    var httpMethod = HttpRequestMethod.Get;

                    if (attribute != null)
                    {
                        httpMethod = attribute.Method;
                    }

                    if (attribute?.Url != null)
                    {
                        path = attribute.Url;
                    }

                    if (attribute?.ActionName != null)
                    {
                        path = $"/{controller.Name.Replace("Controller", string.Empty)}/{attribute.ActionName}";
                    }

                    serverRoutingTable.Add(httpMethod, path, request => 
                    {
                        var controllerInstance = Activator.CreateInstance(controller);
                        ((Controller)controllerInstance).Request = request;

                        var controllerPrincipal = ((Controller)controllerInstance).User;
                        var authorizeAttribute = action.GetCustomAttributes()
                            .LastOrDefault(a => a.GetType() == typeof(AuthorizeAttribute)) as AuthorizeAttribute;
                        if (authorizeAttribute != null &&
                        (controllerPrincipal == null
                            || !authorizeAttribute.IsInAuthority(controllerPrincipal)))
                        {
                            return new HttpResponse(HttpResponseStatusCode.Forbidden); //TODO Redirect to configured URL
                        }


                        var response = action.Invoke(controllerInstance, new object[] {}) as ActionResult;
                        return response;
                    });

                    Console.WriteLine(httpMethod + " " + path);
                }
            }
        }
    }
}
