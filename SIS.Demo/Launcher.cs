﻿using System;
using SIS.Demo.Controllers;
using SIS.HTTP.Enums;
using SIS.WebServer;
using SIS.WebServer.Routing;
using SIS.WebServer.Routing.Contracts;

namespace SIS.Demo
{
    class Launcher
    {
        static void Main(string[] args)
        {
            IServerRoutingTable serverRoutingTable = new ServerRoutingTable();

            serverRoutingTable.Add(HttpRequestMethod.Get, "/", request => new HomeController().Home(request));

            var server = new Server(25000, serverRoutingTable);

            server.Run();
        }
    }
}