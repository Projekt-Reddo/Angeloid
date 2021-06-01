using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Angeloid.Models
{
    public partial class User
    {
        public int UserId { get; set; }
        public string FacebookId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public byte[] Avatar { get; set; }
        public bool IsAdmin { get; set; }

        //Relationship to Thread
        [IgnoreDataMember]
        public virtual ICollection<Thread> Threads { get; set; }

        //Relationship to Review
        [IgnoreDataMember]
        public ICollection<Anime> ReviewAnimes { get; set; }
        [IgnoreDataMember]
        public virtual ICollection<Review> Reviews { get; set; }

        //Relationship to Favorite
        public ICollection<Anime> FavoriteAnimes { get; set; }
        [IgnoreDataMember]
        public ICollection<Favorite> Favorites { get; set; }
    }
}