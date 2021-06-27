using System.ComponentModel.DataAnnotations;

namespace Angeloid.Models
{
    public class IsClickedModel
    {
        [Required]
        public int AnimeId;
        [Required]
        public int UserId;
        public bool Rated;
        public bool Favorite;
        public bool Reviewed;
    }
}