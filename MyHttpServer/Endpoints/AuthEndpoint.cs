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
        if (SessionStorage.IsAuthorized(Context))
        {
            return Redirect("zetflix");
        }

        var file = File.ReadAllText(
            @"Templates/Pages/Auth/auth.html");
        return Html(file);
    }

    [Post("login")]
    public IHttpResponseResult LoginPost(string email, string password)
    {
        try
        {
            string connectionString =
                @"Data Source=localhost;Initial Catalog=master;User ID=sa;Password=P@ssw0rd;TrustServerCertificate=true;";

            var connection = new SqlConnection(connectionString);
            var context = new ORMContext<User>(connection);

            var user = context.Where($"email = '{email}' AND password = '{password}'").FirstOrDefault();
            if (user == null)
            {
                return Redirect("login");
            }

            string token = Guid.NewGuid().ToString();
            Cookie cookie = new Cookie("session-token", token);
            Context.Response.SetCookie(cookie);

            SessionStorage.SaveSession(token, user.Id.ToString());

            return Redirect("catalog");
        }
        catch
        {
            return Redirect("login");
        }
    }

    [Get("register")]
    public IHttpResponseResult RegisterGet()
    {
        if (SessionStorage.IsAuthorized(Context))
        {
            return Redirect("catalog");
        }

        var file = File.ReadAllText(
            @"Templates/Pages/Auth/auth.html");
        return Html(file);
    }

    [Post("register")]
    public IHttpResponseResult RegisterPost(string user_email, string user_password, string user_confirm_password)
    {
        if (user_password != user_confirm_password)
            return Redirect("register");

        try
        {
            string connectionString =
                @"Server=localhost; Database=UsersDb; User Id=sa; Password=P@ssw0rd;TrustServerCertificate=true;";

            var connection = new SqlConnection(connectionString);
            var context = new ORMContext<User>(connection);

            context.ExecuteQuerySingle(
                $"INSERT INTO Users (email, password) VALUES ('{user_email}', '{user_password}')");

            return Redirect("login");
        }
        catch
        {
            return Redirect("register");
        }
    }   
}