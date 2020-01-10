using Demo.Data;

namespace SIS.Demo
{
    using SIS.Demo.Controllers;
    using SIS.HTTP.Enums;
    using SIS.MvcFramework;
    using SIS.MvcFramework.Routing;
    using SIS.MvcFramework.Sessions;

    class Launcher
    {
        static void Main(string[] args)
        {
            using (var context = new DemoDbContext())
            {
                context.Database.EnsureCreated();
            }

            IServerRoutingTable serverRoutingTable = new ServerRoutingTable();
            IHttpSessionStorage httpSessionStorage = new HttpSessionStorage();

            //[HttpGet]
            serverRoutingTable.Add(HttpRequestMethod.Get, "/", request => new HomeController(request).Index(request));
            serverRoutingTable.Add(HttpRequestMethod.Get, "/home", request => new HomeController(request).Home(request));

            serverRoutingTable.Add(HttpRequestMethod.Get, "/login", request => new UsersController(request).Login(request));
            serverRoutingTable.Add(HttpRequestMethod.Get, "/register", request => new UsersController(request).Register(request));
            serverRoutingTable.Add(HttpRequestMethod.Get, "/logout", request => new UsersController(request).Logout(request));

            //[HttpPost]
            serverRoutingTable.Add(HttpRequestMethod.Post, "/login", request => new UsersController(request).LoginConfirm(request));
            serverRoutingTable.Add(HttpRequestMethod.Post, "/register", request => new UsersController(request).RegisterConfirm(request));

            var server = new Server(25000, serverRoutingTable, httpSessionStorage);

            server.Run();
        }
    }
}
