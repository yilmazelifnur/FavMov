using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FavMoviesC
{
    internal class Movies
    {
        public int Id { get; set; }
        public string MovieName { get; set; }
        public string  Director { get; set; }
        public string Actors  { get; set; }
        public int ProductionYear { get; set; }
        public string Genre { get; set; }
        public decimal ImdbRating { get; set; }

    }
    
}
