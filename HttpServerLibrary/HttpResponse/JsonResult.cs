using System.Net;
using System.Text;
using System.Text.Json;

namespace HttpServerLibrary.HttpResponse
{
    internal class JsonResult : IHttpResponseResult
    {
        private readonly string _jsonResponse;

        public JsonResult(string jsonResponse)
        {
            _jsonResponse = jsonResponse;
        }

        public async void Execute(HttpListenerResponse response)
        {
            try
            {
                // Устанавливаем заголовок контента
                response.Headers.Add(HttpRequestHeader.ContentType, "application/json");

                // Преобразуем строку в массив байтов
                byte[] buffer = Encoding.UTF8.GetBytes(_jsonResponse);

                // Устанавливаем длину контента
                response.ContentLength64 = buffer.Length;

                // Записываем данные в поток асинхронно
                using (Stream output = response.OutputStream)
                {
                    await output.WriteAsync(buffer, 0, buffer.Length); // Асинхронно записываем все данные
                    await output.FlushAsync(); // Асинхронно очищаем буфер
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при записи в поток: {ex.Message}");
            }
        }
    }



}
