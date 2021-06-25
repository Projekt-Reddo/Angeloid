using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Angeloid.Models
{
    public partial class Anime
    {
        public int AnimeId { get; set; }
        [Required]
        public string AnimeName { get; set; }
        public string Content { get; set; }
        public byte[] Thumbnail { get; set; }
        public string Status { get; set; }
        public byte[] Wallpaper { get; set; }
        public string Trailer { get; set; }
        public int View { get; set; }
        public string EpisodeDuration { get; set; }
        public string Episode { get; set; }
        public string StartDay { get; set; }
        public string Web { get; set; }

        // Relationship to Character
        public virtual ICollection<Character> Characters { get; set; }

        // Relationship to Season
        // [IgnoreDataMember]
        public int? SeasonId { get; set; } = null!;
        [Required]
        public Season Season { get; set; }

        // Relationship to Studio
        // [IgnoreDataMember]
        public int? StudioId { get; set; } = null!;
        public Studio Studio { get; set; }

        //Relationship to Tag
        public ICollection<Tag> Tags { get; set; }
        [IgnoreDataMember]
        public ICollection<AnimeTag> AnimeTags { get; set; }

        // Relationship to Review
        public ICollection<User> UsersReview { get; set; }
        [IgnoreDataMember]
        public virtual ICollection<Review> Reviews { get; set; }

        // Relationship to Favorite
        public ICollection<User> UsersFavorite { get; set; }
        [IgnoreDataMember]
        public ICollection<Favorite> Favorites { get; set; }
    }
}