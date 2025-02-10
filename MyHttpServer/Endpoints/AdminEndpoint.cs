using System.Data.SqlClient;
using System.Text;
using System.Text.Json;
using System.Web;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.Core;
using HttpServerLibrary.HttpResponse;
using HttpServerLibrary.Session;
using MyHttpServer.Models;
using MyORMLibrary;

namespace MyHttpServer.Endpoints;

internal class AdminEndpoint: BaseEndpoint
{
    private readonly ORMContext<Film> _context;
    private readonly string _connectionString = @"Data Source=localhost;Initial Catalog=master;User ID=sa;Password=P@ssw0rd;TrustServerCertificate=true;";  

    public AdminEndpoint()
    {
        var connection = new SqlConnection(_connectionString);
        _context = new ORMContext<Film>(connection);
    }

    // Получить все фильмы
    [Get("admin")]
    public IHttpResponseResult GetAllFilms()
    {
        //if (!SessionStorage.IsAuthorized(Context))
        //{
        //    return Redirect("login");
        //}

        var contextORM = new ORMContext<Film>(new SqlConnection(_connectionString));
        var films = contextORM.GetByAll();

        foreach (var film in films)
        {
            Console.WriteLine($"Film: {film.Title}, Year: {film.Year}, Genre: {film.Genre}, Rating: {film.Rating}");
        }

        var filmsHtml = string.Join("", films.Select(film => $@"
        <div class='film'>
            <h3>{film.Title}</h3>
            <p>Год: {film.Year}</p>
            <p>Жанр: {film.Genre}</p>
            <p>Рейтинг: {film.Rating}</p>
            <button onclick=""deleteFilm({film.Id})"">Удалить</button>
        </div>
    "));

        var file = File.ReadAllText(@"Templates/Pages/Admin/admin_panel.html");
        file = file.Replace("{{films}}", filmsHtml);

        return Html(file);
    }


    // Добавить новый фильм
    [Post("admin/addfilm")]
    public IHttpResponseResult AddFilm(
    string title, string year, string rating, string description, string posterUrl,
    string genre, string country, string duration, string director, string cast, string extendedDescription)
    {
        try
        {
            // Получаем ID для жанра и страны, используя обновленные методы
            var genreId = ConvertIdByName(genre);
            var countryId = ConvertCountryIdByName(country);
            var yearsId = ConvertYearIdByYear(int.Parse(year));



            // Вставляем фильм в базу данных
            string query = $@"
            INSERT INTO Films (Title, YearId, Duration, GenreId, CountryId, Director, Cast, Description, Rating, PosterURL, ExtendedDescription)
            VALUES ('{title}', {yearsId}, {int.Parse(duration)}, {genreId}, {countryId}, '{director}', '{cast}', '{description}', {rating}, '{posterUrl}', '{extendedDescription}')";

            _context.ExecuteQuerySingle(query); // Выполнение запроса

            return Redirect("admin"); // Перенаправляем в админ панель
        }
        catch (Exception e)
        {
            // В случае ошибки выводим сообщение
            return Html($"Error occurred: {e.Message}");
        }
    }


    // Метод для получения ID жанра по его имени
    private int ConvertIdByName(string genreName)
    {
        switch (genreName.ToLower())
        {
            case "биография":
                return 1;
            case "боевик":
                return 2;
            case "вестерн":
                return 3;
            case "военный":
                return 4;
            case "детектив":
                return 5;
            case "документальный":
                return 6;
            case "драма":
                return 7;
            case "история":
                return 8;
            case "комедия":
                return 9;
            case "криминал":
                return 10;
            case "мелодрама":
                return 11;
            case "музыка":
                return 12;
            case "мюзикл":
                return 13;
            case "приключения":
                return 14;
            case "семейный":
                return 15;
            case "спорт":
                return 16;
            case "триллер":
                return 17;
            case "ужасы":
                return 18;
            case "фантастика":
                return 19;
            case "фэнтези":
                return 20;
            default:
                return 1; 
        }
    }



    // Метод для получения ID страны по ее названию
    private int ConvertCountryIdByName(string countryName)
    {
        switch (countryName.ToLower())
        {
            case "сша":
                return 1;
            case "великобритания":
                return 2;
            case "испания":
                return 3;
            case "италия":
                return 4;
            case "франция":
                return 5;
            case "канада":
                return 6;
            case "германия":
                return 7;
            case "турция":
                return 8;
            case "индия":
                return 9;
            case "корея":
                return 10;
            case "япония":
                return 11;
            default:
                return 1; 
        }
    }

    private int ConvertYearIdByYear(int year)
    {
        switch (year)
        {
            case 2025:
                return 1;
            case 2024:
                return 2;
            case 2023:
                return 3;
            case 2022:
                return 4;
            case 2021:
                return 5;
            case 2020:
                return 6;
            case 2019:
                return 7;
            case 2018:
                return 8;
            case 2017:
                return 9;
            case 2016:
                return 10;
            case 2015:
                return 11;
            case 2014:
                return 12;
            case 2013:
                return 13;
            case 2012:
                return 14;
            case 2011:
                return 15;
            case 2010:
                return 16;
            case 2009:
                return 17;
            case 2008:
                return 18;
            case 2007:
                return 19;
            case 2006:
                return 20;
            case 2005:
                return 21;
            case 2004:
                return 22;
            case 2003:
                return 23;
            case 2002:
                return 24;
            case 2001:
                return 25;
            case 2000:
                return 26;
            default:
                return 1;
        }
    }








    [Post("admin/deletefilm")]
    public IHttpResponseResult DeleteFilm(int id)
    {
        try
        {
            string query = $"DELETE FROM Films WHERE Id = {id}";

            _context.ExecuteQuerySingle(query);

            return Redirect("admin"); 
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);

            return Text("Произошла ошибка при удалении фильма.");
        }
    }

}



