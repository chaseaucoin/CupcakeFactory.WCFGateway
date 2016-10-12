using System;

namespace CupcakeFactory.WCFGateway
{
    /// <summary>
    /// A container for the wcf proxy and 
    /// </summary>
    internal class ProxyContainer
    {


        /// <summary>
        /// Gets or sets the type of the service.
        /// </summary>
        /// <value>
        /// The type of the service.
        /// </value>
        public Type ServiceType { get; set; }

        /// <summary>
        /// Gets or sets the service.
        /// </summary>
        /// <value>
        /// The service.
        /// </value>        
        public object Service { get; set; }

        /// <summary>
        /// Gets or sets the proxy.
        /// </summary>
        /// <value>
        /// The proxy.
        /// </value>
        public object Proxy { get; set; }
    }
}
