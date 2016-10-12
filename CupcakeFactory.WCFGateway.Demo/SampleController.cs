using System.Web.Http;

namespace CupcakeFactory.WCFGateway.Demo
{
    /// <summary>
    /// A simple controller to demo usign a regular controller with the WCF gateway
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class SampleController : ApiController
    {
        /// <summary>
        /// Does the stuff.
        /// </summary>
        /// <returns></returns>
        public string DoStuff()
        {
            return "I Did The Stuff!!";
        }    
    }
}
