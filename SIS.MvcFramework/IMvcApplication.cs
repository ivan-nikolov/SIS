namespace SIS.MvcFramework
{
    using SIS.MvcFramework.DependencyContainer;
    using SIS.MvcFramework.Routing;

    public interface IMvcApplication
    {
        void Configure(IServerRoutingTable serverRoutingTable);

        void ConfigureServices(IServiceProvider serviceProvider); //DI
    }
}
