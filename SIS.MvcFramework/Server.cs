namespace SIS.MvcFramework
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using SIS.HTTP.Common;
    using SIS.MvcFramework.Routing;
    using SIS.MvcFramework.Sessions;
    using SIS.Common;

    public class Server
    {
        private const string LocalHostIpAddress = "127.0.0.1";
        private readonly int port;
        private TcpListener listener;

        private readonly IServerRoutingTable serverRoutingTable;
        private readonly IHttpSessionStorage httpSessionStorage;
        private bool isRunning;

        public Server(int port, IServerRoutingTable serverRoutingTable, IHttpSessionStorage httpSessionStorage)
        {
            serverRoutingTable.ThrowIfNull(nameof(serverRoutingTable));
            httpSessionStorage.ThrowIfNull(nameof(httpSessionStorage));

            this.port = port;
            this.listener = new TcpListener(IPAddress.Parse(LocalHostIpAddress), port);

            this.serverRoutingTable = serverRoutingTable;
            this.httpSessionStorage = httpSessionStorage;
        }

        public void Run()
        {
            this.listener.Start();
            this.isRunning = true;

            Console.WriteLine($"Server started at http://{LocalHostIpAddress}:{this.port}");

            while (this.isRunning)
            {
                var client = this.listener.AcceptSocketAsync().GetAwaiter().GetResult();

                Task.Run(() => this.Listen(client));
            }
        }

        public async Task Listen(Socket client)
        {
            var connectionHandler = new ConnectionHandler(client, this.serverRoutingTable, this.httpSessionStorage);

            await connectionHandler.ProcessRequestAsync();
        }
    }
}
