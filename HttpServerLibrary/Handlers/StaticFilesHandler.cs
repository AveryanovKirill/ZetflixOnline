using HttpServerLibrary.Core;
using HttpServerLibrary.Configuration;
using System.Net;

namespace HttpServerLibrary.Handlers
{
    internal class StaticFilesHandler : Handler
    {
        private readonly string _staticDirectoryPath = $"{Directory.GetCurrentDirectory()}\\{AppConfig.StaticDirectoryPath}";

        public override void HandleRequest(HttpRequestContext context)
        {
            bool IsGet = context.Request.HttpMethod.Equals("Get", StringComparison.OrdinalIgnoreCase);
            string[] arr = context.Request.Url?.AbsolutePath.Split('.');
            bool IsFile = arr.Length >= 2;

            if (IsGet && IsFile)
            {
                // Получить файл
                string relativePath = context.Request.Url?.AbsolutePath.TrimStart('/');
                string filePath = Path.Combine(_staticDirectoryPath, string.IsNullOrEmpty(relativePath) ? "index.html" : relativePath);

                try
                {
                    if (!File.Exists(filePath))
                    {
                        // TODO: Если нет файла "404.html" отправлять просто статус код и текст
                        filePath = Path.Combine(_staticDirectoryPath, "404.html");
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    }

                    byte[] responseFile = File.ReadAllBytes(filePath);
                    context.Response.ContentType = GetContentType(Path.GetExtension(filePath));
                    context.Response.ContentLength64 = responseFile.Length;
                    context.Response.OutputStream.Write(responseFile, 0, responseFile.Length);
                    context.Response.OutputStream.Close();
                }
                catch
                {

                }
            }
            // передача запроса дальше по цепи при наличии в ней обработчиков
            else if (Successor != null)
            {
                // Не правим
                Successor.HandleRequest(context);
            }
        }

        private string GetContentType(string? extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException(nameof(extension), "Extension cannot be null.");
            }

            return extension.ToLower() switch
            {
                ".html" => "text/html",
                ".css" => "text/css",
                ".js" => "application/javascript",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream",
            };
        }
    }
}
