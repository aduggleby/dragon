using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using Dragon.Diagnostics.Web.Handlers;
using Microsoft.Web.WebSockets;

namespace Dragon.Diagnostics.Web.Controllers
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
