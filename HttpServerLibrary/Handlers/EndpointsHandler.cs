using HttpServerLibrary.Core;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.HttpResponse;
using System.Net;
using System.Reflection;
using System.Web;
using System.Text;

namespace HttpServerLibrary.Handlers
{
    internal class EndpointsHandler : Handler
    {
        private readonly Dictionary<string, List<(HttpMethod method, MethodInfo handler, Type endpointType)>> _routes = new();

        public EndpointsHandler()
        {
            // Автоматически регистрируем все контроллеры
            RegisterEndpointsFromAssemblies(new[] { Assembly.GetEntryAssembly() });
        }

        public override void HandleRequest(HttpRequestContext context)
        {
            var url = context.Request.Url?.LocalPath.Trim('/');
            var methodType = context.Request.HttpMethod.ToUpperInvariant();


            if (_routes.ContainsKey(url))
            {
                var route = _routes[url].FirstOrDefault(r =>
                    r.method.ToString().Equals(methodType, StringComparison.InvariantCultureIgnoreCase));

                if (route.handler != null)
                {
                    var endpointInstance = Activator.CreateInstance(route.endpointType) as BaseEndpoint;

                    if (endpointInstance != null)
                    {
                        endpointInstance.SetContext(context);

                        var parameters = GetParams(context, route.handler);
                        var result = route.handler.Invoke(endpointInstance, parameters) as IHttpResponseResult;
                        result?.Execute(context.Response); // Execute the result


                    }
                }
            }
            else
            {
                Console.WriteLine("route no found");
                byte[] buffer = Encoding.UTF8.GetBytes("<h1>404</h1>");
                context.Response.ContentLength64 = buffer.Length;
                context.Response.ContentType = "text/html";
                using Stream output = context.Response.OutputStream;
                output.WriteAsync(buffer);
                output.FlushAsync();
            }
        }

        private void RegisterEndpointsFromAssemblies(Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var endpointsTypes = assembly.GetTypes()
                    .Where(t => typeof(BaseEndpoint).IsAssignableFrom(t) && !t.IsAbstract);

                foreach (var endpointType in endpointsTypes)
                {
                    var methods = endpointType.GetMethods();

                    foreach (var method in methods)
                    {
                        // TODO: можно отрефакторить
                        var getAttribute = method.GetCustomAttribute<GetAttribute>();
                        if (getAttribute != null)
                        {
                            RegisterRoute(getAttribute.Route, HttpMethod.Get, method, endpointType);
                        }

                        var postAttribute = method.GetCustomAttribute<PostAttribute>();
                        if (postAttribute != null)
                        {
                            RegisterRoute(postAttribute.Route, HttpMethod.Post, method, endpointType);
                        }
                    }
                }
            }
        }

        private void RegisterRoute(string route, HttpMethod method, MethodInfo handler, Type endpointType)
        {
            if (!_routes.ContainsKey(route))
            {
                _routes[route] = new();
            }

            _routes[route].Add((method, handler, endpointType));
        }

        private object?[] GetParams(HttpRequestContext context, MethodInfo handler)
        {
            var parameters = handler.GetParameters();
            var result = new List<object?>();

            if (context.Request.HttpMethod == "GET" || context.Request.HttpMethod == "POST")
            {
                using var reader = new StreamReader(context.Request.InputStream);
                string body = reader.ReadToEnd();
                var data = HttpUtility.ParseQueryString(body);
                foreach (var parameter in parameters)
                {
                    if (context.Request.HttpMethod == "GET")
                    {
                        result.Add(Convert.ChangeType(context.Request.QueryString[parameter.Name],
                            parameter.ParameterType));
                    }
                    else if (context.Request.HttpMethod == "POST")
                    {
                        // using var reader = new StreamReader(context.Request.InputStream);
                        // string body = reader.ReadToEnd();
                        // var data = HttpUtility.ParseQueryString(body);
                        result.Add(Convert.ChangeType(data[parameter.Name], parameter.ParameterType));
                    }
                }
            }
            else
            {
                // Дополнительная обработка для сегментов URL
                var urlSegments = context.Request.Url?.Segments
                    .Skip(2) // Пропуск первых двух сегментов
                    .Select(s => s.Replace("/", ""))
                    .ToArray();

                for (int i = 0; i < parameters.Length; i++)
                {
                    result.Add(Convert.ChangeType(urlSegments?[i], parameters[i].ParameterType));
                }
            }

            return result.ToArray();
        }

        
    }
}
