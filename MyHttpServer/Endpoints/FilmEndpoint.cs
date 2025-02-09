using HttpServerLibrary.Attributes;
using HttpServerLibrary.Core;
using HttpServerLibrary.HttpResponse;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyHttpServer.Models;
using MyORMLibrary;
using HttpServerLibrary.Session;
using TemplateEngine;

namespace MyHttpServer.Endponts
{
    internal class FilmEndpoint : BaseEndpoint
    {
        private readonly ORMContext<Film> _dbContext;

        public FilmEndpoint()
        {
            string connectionString = @"Data Source=localhost;Initial Catalog=master;User ID=sa;Password=P@ssw0rd;TrustServerCertificate=true;";
            var connection = new SqlConnection(connectionString);
            _dbContext = new ORMContext<Film>(connection);
        }

        [Get("film")]
        public IHttpResponseResult GetPage(int id)
        {
            

            try
            {
                var film = _dbContext.GetById(id);

                var templatePath = @"Templates\Pages\Film\film.html";
                if (film == null)
                {
                    return Html("<h1>Film was not found</h1>");
                }

                if (!File.Exists(templatePath))
                {
                    return Html("<h1>Template was not found</h1>");
                }

                var template = File.ReadAllText(templatePath);
                var templateEngine = new TemplateEngine.TemplateEngine();
                var htmlContent = templateEngine.Render(template, film);
                return Html(htmlContent);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occured: {e.Message}");
                return Html($"<h1>Error occured: {e.Message}</h1>");
            }
        }
    }
}
