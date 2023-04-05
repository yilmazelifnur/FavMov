using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FavMoviesC
{
    internal class UsersMovie : Movies
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public string MovieNames { get; set; }
        public decimal Rating { get; set; }

    }
}
