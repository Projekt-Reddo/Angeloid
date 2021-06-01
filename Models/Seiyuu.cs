using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Angeloid.Models
{
    public partial class Seiyuu
    {
        public int SeiyuuId { get; set; }
        public string SeiyuuName { get; set; }
        public byte[] SeiyuuImage { get; set; }

        //Relation to Character
        [IgnoreDataMember]
        public virtual ICollection<Character> Characters { get; set; }
    }
}