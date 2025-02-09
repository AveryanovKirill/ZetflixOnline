using HttpServerLibrary.HttpResponse;
using HttpServerLibrary.Session;

namespace HttpServerLibrary.Core
{
    public class BaseEndpoint
    {
        protected HttpRequestContext Context { get; private set; }

        internal void SetContext(HttpRequestContext context)
        {
            Context = context;
        }

        protected IHttpResponseResult Html(string responseText) => new HtmlResult(responseText);

        protected IHttpResponseResult Json(object data) => new JsonResult(data);
        
        protected IHttpResponseResult Redirect(string url) => new RedirectResponse(url);
        
        public bool IsAuthorized(HttpRequestContext context)
        {
            // Проверка наличия Cookie с session-token
            if (context.Request.Cookies.Any(c => c.Name == "session-token"))
            {
                var cookie = context.Request.Cookies["session-token"];
                return SessionStorage.ValidateToken(cookie.Value);
            }

            return false;
        }

    }
}
