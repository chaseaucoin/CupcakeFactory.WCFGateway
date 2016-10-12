using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace CupcakeFactory.WCFGateway
{
    internal class WCFActionSelector : IHttpActionSelector
    {
        static List<HttpActionDescriptor> _descriptors;
        static IHttpActionSelector _originalSelector;

        public WCFActionSelector(IHttpControllerSelector controllerSelector, IHttpActionSelector originalSelector)
        {
            _originalSelector = originalSelector;

            if (_descriptors == null)
            {

                _descriptors = new List<HttpActionDescriptor>();                

                foreach (var controllerDescriptor in controllerSelector.GetControllerMapping().Values)
                {
                    Type serviceType = controllerDescriptor.ControllerType;

                    if (controllerDescriptor.Properties.ContainsKey("CupcakeFactory_ServiceType"))
                    {
                        serviceType = (Type)controllerDescriptor.Properties["CupcakeFactory_ServiceType"];

                        var methods = serviceType.GetMethods().Where(x => x.DeclaringType == serviceType);

                        foreach (var method in methods)
                        {
                            _descriptors.Add(new ReflectedHttpActionDescriptor(controllerDescriptor, method));
                        }
                    }
                    else
                    {
                        _descriptors.AddRange(_originalSelector.GetActionMapping(controllerDescriptor).SelectMany(x => x.Select(y => y)));
                    }
                }
            }
        }

        public ILookup<string, HttpActionDescriptor> GetActionMapping(HttpControllerDescriptor controllerDescriptor)
        {   
            var lookup = _descriptors
                .Where(x => x.ControllerDescriptor.ControllerName == controllerDescriptor.ControllerName)
                .ToLookup<HttpActionDescriptor, string>(p => p.ActionName);

            if (lookup == null)
                lookup = _originalSelector.GetActionMapping(controllerDescriptor);

            return lookup;
        }

        public HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
        {
            HttpActionDescriptor action = _descriptors.Cast<ReflectedHttpActionDescriptor>().Where(d => d.ActionName ==
                (string)controllerContext.RouteData.Values["action"] &&
                d.MethodInfo.DeclaringType.Name == (string)controllerContext.RouteData.Values["controller"])
                .FirstOrDefault();

            if (action == null)
                action = _originalSelector.SelectAction(controllerContext);


            return action;
        }
    }
}
