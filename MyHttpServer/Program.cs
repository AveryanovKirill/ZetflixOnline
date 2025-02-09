using HttpServerLibrary;
using HttpServerLibrary.Configuration;
using System.Text.Json;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyHttpServer
{
    internal class Program
    {
        #region
        // TODO: Необходимо проект АнимеГО или ЯндексЕда правильно разделить по файлам и по каталогам (js, css, images),
        // после обновить папку public
        // TODO: По просьбе Гайфуллина переверстать(переструктурировать) по принципу сайт БЭМ

        // TODO: По URL отображать файл из папки public если он есть (включая вложенность)
        // TODO: по умолчанию если не указан файл тогда искать файл index.html
        // TODO: Если нет ни файла ни index.html отображать страницу с кодом 404 такой страницы нет
        // TODO: В зависимости от типа файла подставлять соответствующий Content-Type (js, css, image)
        #endregion

        #region
        // 16/11/2024

        // TODO: [Архитектурно] Необходимо  из запроса передавать параметры: если это Get -> query, если Post -> formData в метод соответствующий в Endpoints
        // TODO: [Архитектурно] Превратить AppConfig в синглтон, не меняя его текущей логики работы. AppConfig переместить в новую папку  /Core/Configuration
        // TODO: [Архитектурно] EndpointsHandler. При регистрации роутингов если существует уже такое роутинг + метод выкидывать в лог ошибку об этом и не запускать сервер
        // TODO: Добавить в проект HomeWorkEndpoints. Рeализовать в нем метод (роутинг "send-home-work") который вызывает отправку сообщения
        // с вашим выполненным ДЗ на почту которое приходит в параметрах запроса со страницы EA/login (метод Get + Post)

        // TODO*: [Архитектурно] Необходимо доработать вызов метода в endpoints таким образом, чтобы не было необходимости превращать в каждом методе все в байты

        #endregion

        #region
        // 20/11/2024

        // TODO: Задокументировать Код. XML Документация MSDN
        // TODO: Добавить комментарий к каждой строке кода в проекте с тем что делается в данной строке
        // TODO: [Кодинг] Добавить логику в EndpointsHandler что если метод ничего не возвращает тогда делать так же REsponse пустой
        // не должно быть долгово ловадера если вызвался просто метод POST
        #endregion

        #region
        // 22/11/2024

        // TODO: Необходимо реализовать метод GetHtmlByTemplate в классе CustomTemplator таким образом,
        // чтобы он получал Тип и замену делал по свойствам данного типа
        // TODO: проверить вышеописанный метод на тесте который написали на занятии
        // [Теория] Razor Шаблонитор
        // [Теория] MSTest
        // [Теория] Moq

        #endregion

        #region
        // 23/11/2024

        // TODO: Необходимо реализовать метод GetHtmlByTemplate в классе CustomTemplator таким образом,
        // чтобы он обрабатывал шаблон следующего вида:
        // "if(gender){<h1>Да мой господин, {name}</h1> }else{ <h1>Да моя госпожа, {name}</h1>}"

        #endregion

        static async Task Main(string[] args)
        {
            string filePath = "config.json";

            try
            {
                await AppConfig.LoadFromFileAsync(filePath);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
            }
            var prefixes = new[] { $"http://{AppConfig.Domain}:{AppConfig.Port}/" };
            var server = new HttpServer(prefixes);

            await server.StartAsync();          

        }
    }
}
