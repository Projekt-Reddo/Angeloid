using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Angeloid.Models
{
    public partial class Character
    {
        public int CharacterId { get; set; }
        public string CharacterName { get; set; }
        public string CharacterRole { get; set; }
        public byte[] CharacterImage { get; set; }

        // Relationship to Seiyuu
        [IgnoreDataMember]
        public int? SeiyuuId { get; set; } = null!;
        public Seiyuu Seiyuu { get; set; }

        //Relationship to Anime
        [IgnoreDataMember]
        public int? AnimeId { get; set; } = null!;
        public Anime Anime { get; set; }
    }
}