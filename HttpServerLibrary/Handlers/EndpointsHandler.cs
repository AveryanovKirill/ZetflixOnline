using HttpServerLibrary.Core;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.HttpResponse;
using System.Net;
using System.Reflection;
using System.Web;

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
            var url = context.Request.Url.LocalPath.Trim('/');
            var methodType = context.Request.HttpMethod;

            if (_routes.ContainsKey(url))
            {
                var route = _routes[url].FirstOrDefault(r => r.method.ToString().Equals(methodType, StringComparison.InvariantCultureIgnoreCase));
                if (route.handler != null)
                {
                    var endpointInstance = Activator.CreateInstance(route.endpointType) as BaseEndpoint;
                    if (endpointInstance != null)
                    {
                        endpointInstance.SetContext(context);



                        // вызываем метод
                        // TODO: подсказка, null - это параметры  (если это Get -> query, если Post -> formData)
                        var parameters = GetParams(context, route.handler);
                        var result = route.handler.Invoke(endpointInstance, parameters) as IHttpResponseResult;
                        result?.Execute(context.Response);

                        // TODO: Добавить базовый обработчик если нет result типа IHttpResponseResult
                    }
                }
            }
            else if (Successor != null)
            {
                Successor.HandleRequest(context);
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

        private object[] GetParams(HttpRequestContext context, MethodInfo handler)
        {
            var parameters = handler.GetParameters();
            var result = new List<object>();

            if (context.Request.HttpMethod == "GET" || context.Request.HttpMethod == "POST")
            {
                foreach (var parameter in parameters)
                {
                    if (context.Request.HttpMethod == "GET")
                    {
                        AddGetMethod(context, result, parameter);
                    }
                    else if (context.Request.HttpMethod == "POST")
                    {
                        AddPostMethod(context, result, parameter);
                    }
                }
            }
            else
            {
                AddOtherMethods(context, result, parameters);
            }
            return result.ToArray();
        }

        private void AddGetMethod(HttpRequestContext context, List<object> result, ParameterInfo parameter)
        {
            result.Add(Convert.ChangeType(context.Request.QueryString[parameter.Name], parameter.ParameterType));
        }

        private void AddPostMethod(HttpRequestContext context, List<object> result, ParameterInfo parameter)
        {
            using var reader = new StreamReader(context.Request.InputStream);
            string body = reader.ReadToEnd();
            var data = HttpUtility.ParseQueryString(body);
            result.Add(Convert.ChangeType(data[parameter.Name], parameter.ParameterType));
        }

        private void AddOtherMethods(HttpRequestContext context, List<object> result, ParameterInfo[] parameters)
        {
            var urlSegments = context.Request.Url.Segments
                    .Skip(2)
                    .Select(s => s.Replace("/", ""))
                    .ToArray();

            for (int i = 0; i < parameters.Length; i++)
            {
                result.Add(Convert.ChangeType(urlSegments[i], parameters[i].ParameterType));
            }
        }
    }
}
