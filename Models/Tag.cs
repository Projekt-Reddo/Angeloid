using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Angeloid.Models
{
    public partial class Tag
    {
        public int TagId { get; set; }
        public string TagName { get; set; }
        public string TagDescription { get; set; }

        //Relationship to Anime by AnimeTag
        [IgnoreDataMember]
        public virtual ICollection<Anime> Animes { get; set; }
        [IgnoreDataMember]
        public virtual ICollection<AnimeTag> AnimeTags { get; set; }
    }

    public partial class AnimeTag
    {
        public int AnimeId { get; set; }
        public Anime Anime { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}