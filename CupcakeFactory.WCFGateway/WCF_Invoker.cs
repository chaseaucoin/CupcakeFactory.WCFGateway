using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.Filters;

namespace CupcakeFactory.WCFGateway
{
    internal class WCF_Invoker : IHttpController
    {
        private class Helper : ApiController
        {

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            HttpControllerDescriptor controllerDescriptor = controllerContext.ControllerDescriptor;
            ServicesContainer controllerServices = controllerDescriptor.Configuration.Services;
            ReflectedHttpActionDescriptor actionDescriptor = (ReflectedHttpActionDescriptor)controllerServices.GetActionSelector().SelectAction(controllerContext);
            HttpActionContext _actionContext = new HttpActionContext();
            _actionContext.ControllerContext = controllerContext;
            _actionContext.ActionDescriptor = actionDescriptor;

            var binder = actionDescriptor.ActionBinding;
            await binder.ExecuteBindingAsync(_actionContext, cancellationToken);

            var container = (ProxyContainer)controllerDescriptor.Properties["CupcakeFactory_ProxyContainer"];

            var service = container.Service;

            var filters = actionDescriptor.GetFilterPipeline();

            object result = null;

            Func<Task<HttpResponseMessage>> response = () => {
                result = actionDescriptor.MethodInfo.Invoke(service, _actionContext.ActionArguments.Values.ToArray());
                return Task.FromResult(controllerContext.Request.CreateResponse(result));
            };

            foreach (var filter in filters
                .Where(f => f.Instance is IAuthorizationFilter)
                .OrderByDescending(f => f.Scope)
                .Select(f => f.Instance)
                .Cast<IAuthorizationFilter>())
            {
                response = () => filter.ExecuteAuthorizationFilterAsync(_actionContext, cancellationToken, response);
            }

            var output = await response();

            return output;
        }        
    }
}
