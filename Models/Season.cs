using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Angeloid.Models
{
    public partial class Season
    {
        public int SeasonId { get; set; }
        public string Year { get; set; }
        public string SeasonName { get; set; }

        //Relation to Anime
        [IgnoreDataMember]
        public virtual ICollection<Anime> Animes { get; set; }
    }
}