using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;

namespace CupcakeFactory.WCFGateway
{
    internal class CustomControllerSelector : IHttpControllerSelector
    {
        static IDictionary<string, HttpControllerDescriptor> _descriptors;
        static IHttpControllerSelector _originalSelector;

        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(params IDictionary<TKey, TValue>[]  dictionaries)
        {
            var result = new Dictionary<TKey, TValue>();
            foreach (var dict in dictionaries)
                foreach (var x in dict)
                    result[x.Key] = x.Value;
            return result;
        }

        internal CustomControllerSelector(HttpConfiguration config, IEnumerable<ProxyContainer> proxies, IHttpControllerSelector originalSelector)
        {
            _originalSelector = originalSelector;

            if (_descriptors == null)
            {
                var output = new Dictionary<string, HttpControllerDescriptor>();

                foreach (var proxy in proxies)
                {
                    var descriptor = new HttpControllerDescriptor()
                    {
                        Configuration = config,
                        ControllerName = proxy.ServiceType.Name,
                        ControllerType = typeof(WCF_Invoker)
                    };
                    
                    descriptor.Properties["CupcakeFactory_ServiceType"] = proxy.ServiceType;                    
                    descriptor.Properties["CupcakeFactory_ProxyContainer"] = proxy;

                    output.Add(proxy.ServiceType.Name, descriptor);
                }

                var defaults = _originalSelector.GetControllerMapping();

                _descriptors = Merge(output, defaults);
            }
        }

        public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            return _descriptors;
        }
        
        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var routeData = (HttpRouteData)request.Properties["MS_HttpRouteData"];
            var controller = (string)routeData.Values["controller"];

            return _descriptors[controller];
        }
    }
}
