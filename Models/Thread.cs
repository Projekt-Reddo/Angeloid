using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Angeloid.Models
{
    public partial class Thread
    {
        public int ThreadId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public byte[] Image { get; set; }

        // Relationship to User
        // [IgnoreDataMember]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}