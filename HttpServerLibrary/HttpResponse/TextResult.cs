using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServerLibrary.HttpResponse
{


    internal class TextResult : IHttpResponseResult
    {
        private readonly string _message;

        public TextResult(string message)
        {
            _message = message;
        }

        public void Execute(HttpListenerResponse response)
        {
            try
            {
                // Устанавливаем заголовок контента
                response.Headers.Add(HttpRequestHeader.ContentType, "text/plain");

                // Преобразуем строку в массив байтов
                byte[] buffer = Encoding.UTF8.GetBytes(_message);

                // Устанавливаем длину контента
                response.ContentLength64 = buffer.Length;

                // Получаем поток
                using (Stream output = response.OutputStream)
                {
                    // Записываем все байты в поток
                    output.Write(buffer, 0, buffer.Length);

                    // Если данные не записаны в поток, вызываем Flush
                    if (output.CanWrite)
                    {
                        output.Flush(); // Ожидаем, что все данные записаны
                    }
                } // Поток будет закрыт автоматически при выходе из блока `using`
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при записи в поток: {ex.Message}");
            }
        }
    }

}
