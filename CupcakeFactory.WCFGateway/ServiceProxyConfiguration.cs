using SimpleWCF;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace CupcakeFactory.WCFGateway
{
    public class ServiceProxyConfiguration
    {
        List<ProxyContainer> _serviceProxies;
        HttpConfiguration _httpConfig;
        public ServiceProxyConfiguration(HttpConfiguration httpConfig)
        {
            _httpConfig = httpConfig;
            _serviceProxies = new List<ProxyContainer>();
        }

        public ServiceProxyConfiguration AddService<TServiceContract>(Func<WCFProxy<TServiceContract>, TServiceContract> config)
        {
            var serviceType = typeof(TServiceContract);
            var proxy = new WCFProxy<TServiceContract>();
            var service = config(proxy);
            
            var proxyContainer = new ProxyContainer()
            {
                ServiceType = serviceType,
                Proxy = proxy,
                Service = service
            };

            _serviceProxies.Add(proxyContainer);

            return this;
        }

        public void Activate()
        {
            var oldControllerSelector = _httpConfig.Services.GetHttpControllerSelector();
            var newControllerSelector = new CustomControllerSelector(_httpConfig, _serviceProxies, oldControllerSelector);

            _httpConfig.Services.Replace(typeof(IHttpControllerSelector), newControllerSelector);

            var oldActionSelector = _httpConfig.Services.GetActionSelector();
            var newActionSelector = new WCFActionSelector(newControllerSelector, oldActionSelector);

            _httpConfig.Services.Replace(typeof(IHttpActionSelector), newActionSelector);
        }        
    }
}
