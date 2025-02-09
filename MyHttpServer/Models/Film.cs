using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.Models
{
    //Title, Year, Duration, Genre, Country, Director, Cast, Description, Rating, Poster
    public class Film
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public int Duration { get; set; }
        public string Genre { get; set; }
        public string Country { get; set; }
        public string Director { get; set; }
        public string Cast { get; set; }
        public string Description { get; set; }
        public double Rating { get; set; }
        public string PosterURL { get; set; }
        public string ExtendedDescription { get; set; }
    }
}
