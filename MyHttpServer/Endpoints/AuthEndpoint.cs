using HttpServerLibrary.Attributes;
using HttpServerLibrary.Core;
using HttpServerLibrary.HttpResponse;
using System.Data.SqlClient;
using System.Net;
using global::MyHttpServer.Models;
using HttpServerLibrary.Session;
using MyORMLibrary;

namespace MyHttpServer.Endponts;



public class AuthEndpoint : BaseEndpoint
{
    
    [Get("login")]
    public IHttpResponseResult LoginGet()
    {

        var file = File.ReadAllText(
            @"Templates/Pages/Auth/auth.html");
        return Html(file);
    }

    [Post("login")]
    public IHttpResponseResult LoginPost(string login, string password)
    {
        try
        {
            
            Console.WriteLine($"Login: {login}, Password: {password}");
            
            string connectionString = @"Data Source=localhost;Initial Catalog=master;User ID=sa;Password=P@ssw0rd;TrustServerCertificate=true;";
            var connection = new SqlConnection(connectionString);
            var context = new ORMContext<User>(connection);


            var user = context.Where($"login = '{login}' AND password = '{password}'").FirstOrDefault();
            if (user == null)
            {
                return Redirect("login");
            }

            string token = Guid.NewGuid().ToString();
            Cookie cookie = new Cookie("session-token", token);
            Context.Response.SetCookie(cookie);
            SessionStorage.SaveSession(token, user.Id.ToString());
            return Redirect("admin");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            return Redirect("login");
        }
    }
}