using System.Web.Http;

namespace CupcakeFactory.WCFGateway
{
    public static class WCFGatewayExtension
    {
        public static ServiceProxyConfiguration ConfigureWcfControllers(this HttpConfiguration httpConfig)
        {
            return new ServiceProxyConfiguration(httpConfig);
        }
    }
}
