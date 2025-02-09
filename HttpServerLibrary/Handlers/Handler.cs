using HttpServerLibrary.Core;
using System.Net;

namespace HttpServerLibrary.Handlers
{
    internal abstract class Handler
    {
        public Handler Successor { get; set; }

        public abstract void HandleRequest(HttpRequestContext context);
    }
}
