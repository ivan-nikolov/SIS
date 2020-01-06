using SIS.WebServer.Routing;

namespace SIS.WebServer
{
    public interface IMvcApplication
    {
        void Configure(ServerRoutingTable serverRoutingTable);

        void ConfigureServices(); //DI
    }
}
