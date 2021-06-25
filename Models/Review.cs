using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Angeloid.Models
{
    public partial class Review
    {
        //Relationship to User
        public int UserId { get; set; }
        public User User { get; set; }

        //Relationship to Anime
        public int AnimeId { get; set; }
        public Anime Anime { get; set; }
        
        public string Content { get; set; }
        public int RateScore { get; set; }

    }
}