using System.Data;
using System.Data.SqlClient;
using System.Net;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.Core;
using HttpServerLibrary.HttpResponse;
using HttpServerLibrary.HttpResponse;
using HttpServerLibrary.Session;
using MyHttpServer.Models;
using MyORMLibrary;

namespace MyHttpServer.Endpoints;

internal class RegistrationEndpoint : BaseEndpoint
{
    [Get("register")]
    public IHttpResponseResult GetRegisterPage()
    {
        if (IsAuthorized(Context)) return Redirect("dashboard");

        var file = File.ReadAllText(@"Templates/Pages/Registration/registration.html");
        return Html(file);
    }

    [Post("register")]
    public IHttpResponseResult RegisterUser(string email, string password, string name)
    {
        string connectionString =
            "Data Source=localhost;Initial Catalog=master;User ID=sa;Password=P@ssw0rd;TrustServerCertificate=true;";

        try
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Ошибка: Заполнены не все поля.");
                return Redirect("register");
            }

            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var context = new ORMContext<User>(connection);

            if (context.ReadByAll($"Email = '{email}'").Any())
            {
                Console.WriteLine("Ошибка: Пользователь с таким email уже существует.");
                return Redirect("register");
            }

            var newUser = new User { Email = email, Password = password, Name = name };
            context.Create(newUser);

            var createdUser = context.ReadByAll($"Email = '{email}'").FirstOrDefault();
            if (createdUser == null)
            {
                Console.WriteLine("Ошибка: Не удалось создать пользователя.");
                return Redirect("register");
            }

            Console.WriteLine($"Пользователь успешно зарегистрирован: ID={createdUser.Id}");

            // Устанавливаем куку с сессионным токеном
            string token = Guid.NewGuid().ToString();
            Cookie cookie = new Cookie("session-token", token);
            Context.Response.SetCookie(cookie);
            SessionStorage.SaveSession(token, newUser.Id);
            

            // Перенаправляем на dashboard
            return Redirect("dashboard");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при регистрации: {ex.Message}");
            return Html("<h3>Ошибка на сервере. Попробуйте позже.</h3>");
        }
    }
}
