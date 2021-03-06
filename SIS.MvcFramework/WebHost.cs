﻿namespace SIS.MvcFramework
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Requests;
    using SIS.HTTP.Responses;
    using SIS.MvcFramework.Attributes.Action;
    using SIS.MvcFramework.Attributes.Http;
    using SIS.MvcFramework.Attributes.Security;
    using SIS.MvcFramework.DependencyContainer;
    using SIS.MvcFramework.Logging;
    using SIS.MvcFramework.Results;
    using SIS.MvcFramework.Routing;
    using SIS.MvcFramework.Sessions;

    public static class WebHost
    {
        public static void Start(IMvcApplication application)
        {
            IServerRoutingTable serverRoutingTable = new ServerRoutingTable();
            IHttpSessionStorage httpSessionStorage = new HttpSessionStorage();
            IServiceProvider serviceProvider = new ServiceProvider();
            serviceProvider.Add<ILogger, ConsoleLogger>();

            AutoRegisterRoutes(application, serverRoutingTable, serviceProvider);

            application.ConfigureServices(serviceProvider);

            application.Configure(serverRoutingTable);

            Server server = new Server(8000, serverRoutingTable, httpSessionStorage);
            server.Run();
        }

        private static void AutoRegisterRoutes(IMvcApplication application, IServerRoutingTable serverRoutingTable, IServiceProvider serviceProvider)
        {
            var controllers = application.GetType().Assembly
                .GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract
                   && typeof(Controller).IsAssignableFrom(type));

            foreach (var controllerType in controllers)
            {
                var actions = controllerType
                    .GetMethods(BindingFlags.DeclaredOnly
                    | BindingFlags.Instance
                    | BindingFlags.Public)
                    .Where(x => !x.IsSpecialName
                        && x.DeclaringType == controllerType)
                    .Where(x => x.GetCustomAttributes().All(a => a.GetType() != typeof(NonActionAttribute)));

                foreach (var action in actions)
                {
                    var attribute = action.GetCustomAttributes()
                        .Where(x => x.GetType().IsSubclassOf(typeof(BaseHttpAttribute))).LastOrDefault() as BaseHttpAttribute;

                    var path = $"/{controllerType.Name.Replace("Controller", string.Empty)}/{action.Name}";
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
                        path = $"/{controllerType.Name.Replace("Controller", string.Empty)}/{attribute.ActionName}";
                    }

                    serverRoutingTable.Add(httpMethod, path, request =>  ProcessRequest(serviceProvider, controllerType, action, request));

                    System.Console.WriteLine(httpMethod + " " + path);
                }
            }
        }

        private static IHttpResponse ProcessRequest(
            IServiceProvider serviceProvider,
            System.Type controllerType,
            MethodInfo action,
            IHttpRequest request)
        {
            var controllerInstance = serviceProvider.CreateInstance(controllerType) as Controller;
            controllerInstance.Request = request;

            var controllerPrincipal = controllerInstance.User;
            var authorizeAttribute = action.GetCustomAttributes()
                .LastOrDefault(a => a.GetType() == typeof(AuthorizeAttribute)) as AuthorizeAttribute;
            if (authorizeAttribute != null &&
            (controllerPrincipal == null
                || !authorizeAttribute.IsInAuthority(controllerPrincipal)))
            {
                return new HttpResponse(HttpResponseStatusCode.Forbidden); //TODO Redirect to configured URL
            }

            var actionParameters = action.GetParameters();
            var parameterValues = new List<object>();

            foreach (var parameter in actionParameters)
            {
                ISet<string> httpDataValue = TryGetHttpParameter(request, parameter.Name);

                //TODO: Support lists
                /*if (parameter.ParameterType.GetInterfaces().Any(
                    i => i.IsGenericType
                    && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                {
                    var genericType = parameter.ParameterType.GetGenericArguments().FirstOrDefault();

                    var collection = httpDataValue
                        .Select(x => System.Convert.ChangeType(x, genericType));

                    parameterValues.Add(collection);
                    continue;
                }*/

                try
                {
                    var httpStringValue = httpDataValue.FirstOrDefault();
                    var parameterValue = System.Convert.ChangeType(httpStringValue, parameter.ParameterType);
                    parameterValues.Add(parameterValue);
                }
                catch
                {
                    var parameterValue = System.Activator.CreateInstance(parameter.ParameterType);
                    var properties = parameter.ParameterType.GetProperties();


                    foreach (var property in properties)
                    {
                        ISet<string> propertyHttpDataValue = TryGetHttpParameter(request, property.Name);
                        var firstValue = propertyHttpDataValue.FirstOrDefault();
                        var propertyValue = System.Convert.ChangeType(firstValue, property.PropertyType);

                        property.SetMethod.Invoke(parameterValue, new object[] { propertyValue });
                    }

                    parameterValues.Add(parameterValue);
                }
            }

            var response = action.Invoke(controllerInstance, parameterValues.ToArray()) as ActionResult;
            return response;
        }

        private static ISet<string> TryGetHttpParameter(IHttpRequest request, string parameterName)
        {
            parameterName = parameterName.ToLower();
            ISet<string> httpDataValue = null;
            if (request.QueryData.Any(x => x.Key.ToLower() == parameterName))
            {
                httpDataValue = request.QueryData
                    .FirstOrDefault(x => x.Key.ToLower() == parameterName)
                    .Value;
            }
            else if (request.FormData.Any(x => x.Key.ToLower() == parameterName))
            {
                httpDataValue = request.FormData
                    .FirstOrDefault(x => x.Key.ToLower() == parameterName)
                    .Value;
            }

            return httpDataValue;
        }
    }
}
