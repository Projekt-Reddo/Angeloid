using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Angeloid.Models
{
    public partial class Favorite
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int AnimeId { get; set; }
        public Anime Anime { get; set; }
    }
}