using System.Net;

namespace HttpServerLibrary.HttpResponse
{
    public interface IHttpResponseResult
    {
        void Execute(HttpListenerResponse response);
    }
}
