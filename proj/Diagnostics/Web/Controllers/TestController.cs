using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using Microsoft.Web.WebSockets;
using Web.Handlers;

namespace Web.Controllers
{
    public class TestController : ApiController
    {
        [AcceptVerbs("GET")]
        public HttpResponseMessage WebSocket(string message)
        {
            HttpContext.Current.AcceptWebSocketRequest(new TestWebSocketHandler(message));
            return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
        }

        [AcceptVerbs("GET")]
        public HttpResponseMessage FastGet()
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [AcceptVerbs("GET")]
        public HttpResponseMessage SlowGet()
        {
            Thread.Sleep(10 * 1000);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
