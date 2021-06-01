using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Angeloid.Models
{
    public partial class Studio
    {
        public int StudioId { get; set; }
        public string StudioName { get; set; }

        //Relashion to Anime
        [IgnoreDataMember]
        public virtual ICollection<Anime> Animes { get; set; }
    }
}