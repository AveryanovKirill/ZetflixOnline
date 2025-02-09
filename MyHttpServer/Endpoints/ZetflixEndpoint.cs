using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.Core;
using HttpServerLibrary.HttpResponse;
using HttpServerLibrary.Session;
using Microsoft.AspNetCore.Mvc;
using MyHttpServer.Models;
using MyORMLibrary;
using TemplateEngine;

namespace MyHttpServer.Endponts;

internal class ZetflixEndpoint : BaseEndpoint
{
    private readonly ORMContext<Film> _dbContext;
    
    public ZetflixEndpoint()
    {
        string connectionString =
            @"Data Source=localhost;Initial Catalog=master;User ID=sa;Password=P@ssw0rd;TrustServerCertificate=true;";
        var connection = new SqlConnection(connectionString);
        _dbContext = new ORMContext<Film>(connection);
    }
    [Get("zetflix")]
    public IHttpResponseResult GenerateFilmsHtml()
    {
        int page = 1;
        int pageSize = 9;

        // Попробуйте получить параметры из запроса
        if (int.TryParse(Context.Request.QueryString["page"], out int parsedPage))
        {
            page = parsedPage;
        }

        if (int.TryParse(Context.Request.QueryString["pageSize"], out int parsedPageSize))
        {
            pageSize = parsedPageSize;
        }

        var file = File.ReadAllText(@"Templates/Pages/Zetflix/index.html");

        var films = _dbContext.GetByAll()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var filmTemplate = $@"
            <div class=""video-item with-mask new-item"">
                <div class=""vi-in"">
                    <a class=""vi-img img-resp-h"" href=""/film?id={{{{Id}}}}"" alt=""{{{{Title}}}}"">
                        <img class="""" src=""{{{{PosterURL}}}}"" data-src=""{{{{PosterURL}}}}"" alt=""{{{{Title}}}}"">
                        <div class=""vi-desc"">
                            <div class=""vi-title"">{{{{Title}}}}</div>
                        </div>
                        <div class=""th-mask fx-col fx-center fx-middle"">
                            <span class=""fa fa-play""></span>
                        </div>
                        <div class=""th-inf icon-l"" data-source=""""><span class=""fa fa-info""></span></div>
                    </a>
                </div>
            </div>";

        var templateEngine = new TemplateEngine.TemplateEngine();

        var filmsHtml = new StringBuilder();
        foreach (var film in films)
        {
            var filmHtml = templateEngine.Render(filmTemplate, film);
            filmsHtml.Append(filmHtml);
        }

        // Replace the placeholder in the HTML template with the generated film HTML
        var finalHtml = file.Replace("<!-- FILMS_PLACEHOLDER -->", filmsHtml.ToString());

        // Return the final HTML
        return Html(finalHtml);
    }

    [Post("zetflix/filtered")]
    public IHttpResponseResult PostMoviesPage(string genre)
    {
        Console.WriteLine($"GENRE: {genre}");
        if (genre == "all")
            return Redirect("http://localhost:2323/zetflix");

        var films = _dbContext.GetByAll()
            .Where(x => x.Genre == genre)
            .ToList();

        var templatePath = @"Templates\Pages\Zetflix\index.html";
        if (!File.Exists(templatePath))
        {
            return Html("<h1>Template was not found</h1>");
        }

        var template = File.ReadAllText(templatePath);
        var templateEngine = new TemplateEngine.TemplateEngine();
        var data = new
        {
            Items = films.Select(f => new
            {
                f.Id,
                f.Title,
                f.Year,
                f.Duration,
                f.Genre,
                f.Country,
                f.Director,
                f.Cast,
                f.Description,
                f.Rating,
                f.PosterURL,
                f.ExtendedDescription
            }).ToList()
        };

        var htmlContent = templateEngine.Render(template, data);
        return Html(htmlContent);
    }
}



