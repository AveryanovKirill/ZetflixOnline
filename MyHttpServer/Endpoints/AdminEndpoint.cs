using System.Data.SqlClient;
using System.Text;
using System.Text.Json;
using System.Web;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.Core;
using HttpServerLibrary.HttpResponse;
using MyHttpServer.Models;
using MyORMLibrary;

namespace MyHttpServer.Endpoints;

internal class AdminEndpoint: BaseEndpoint
{
    private readonly ORMContext<Film> _context;
    private readonly string _dbConnection = @"Data Source=localhost;Initial Catalog=master;User ID=sa;Password=P@ssw0rd;TrustServerCertificate=true;";

    // Метод для получения админ панели
    

    // Получить все фильмы
    [Get("admin")]
    public IHttpResponseResult GetAllFilms()
    {
        var contextORM = new ORMContext<Film>(new SqlConnection(_dbConnection));
        var films = contextORM.GetByAll();

        // Логирование фильмов, чтобы увидеть, что извлекается из базы данных
        foreach (var film in films)
        {
            Console.WriteLine($"Film: {film.Title}, Year: {film.Year}, Genre: {film.Genre}, Rating: {film.Rating}");
        }

        // Передаем список фильмов в HTML-шаблон для отображения
        var filmsHtml = string.Join("", films.Select(film => $@"
        <div class='film'>
            <h3>{film.Title}</h3>
            <p>Год: {film.Year}</p>
            <p>Жанр: {film.Genre}</p>
            <p>Рейтинг: {film.Rating}</p>
            <button onclick='deleteFilm({film.Id})'>Удалить фильм</button>
        </div>
    "));

        var file = File.ReadAllText(@"Templates/Pages/Admin/admin_panel.html");
        file = file.Replace("{{films}}", filmsHtml);  // Вставляем HTML фильмов в шаблон

        return Html(file);
    }


    // Добавить новый фильм
    

    [Post("admin/addfilm")]
    public IHttpResponseResult AddFilm(HttpRequestContext context)
    {
        using (var reader = new StreamReader(context.Request.InputStream, Encoding.UTF8))
        {
            var body = reader.ReadToEnd();
            var formData = HttpUtility.ParseQueryString(body);

            // Получаем параметры формы
            var title = formData["title"];
            var year = formData["year"];
            var rating = formData["rating"];
            var description = formData["description"];
            var posterUrl = formData["poster_url"];
            var genreId = formData["genre_id"];
            var countryId = formData["country_id"];
            var duration = formData["duration"];
            var director = formData["director"];
            var cast = formData["cast"];
            var extendedDescription = formData["extended_description"];


            try
            {
                // Формируем строку запроса с параметрами
                string query = $@"
        INSERT INTO Films (Title, YearId, Duration, GenreId, CountryId, Director, Cast, Description, Rating, PosterURL, ExtendedDescription)
        VALUES ('{title}', {int.Parse(year)}, {int.Parse(duration)}, {int.Parse(genreId)}, {int.Parse(countryId)}, '{director}', '{cast}', '{description}', {float.Parse(rating)}, '{posterUrl}', '{extendedDescription}')";

                // Выполнение запроса
                _context.ExecuteQuerySingle(query);

                return Redirect("admin/films"); // Перенаправляем на страницу с фильмами после добавления
            }
            catch (Exception e)
            {
                return Html($"Error occurred: {e.Message}");
            }
        }
    }

    





    // Удалить фильм
    [Post("admin/deletefilm/{id}")]
    public IHttpResponseResult DeleteFilm(int id)
    {
        try
        {
            // Строка запроса для удаления фильма по ID
            string query = $"DELETE FROM Films WHERE Id = {id}";

            // Выполнение запроса
            _context.ExecuteQuerySingle(query);

            return Redirect("admin/films"); // Перенаправление на страницу с фильмами после удаления
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return Html($"Error occurred: {e.Message}");
        }
    }
}



