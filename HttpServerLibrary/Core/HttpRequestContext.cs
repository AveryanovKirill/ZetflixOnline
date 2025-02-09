using System.Net;

namespace HttpServerLibrary.Core
{
    public class HttpRequestContext
    {
        public HttpListenerRequest Request { get; set; }

        public HttpListenerResponse Response { get; set; }

        public HttpRequestContext(HttpListenerContext context)
        {
            Request = context.Request;
            Response = context.Response;
        }
    }
}
