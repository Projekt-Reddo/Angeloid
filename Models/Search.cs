using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Angeloid.Models
{
    public partial class Search
    {
        public string AnimeName { get; set; }
        public string Year { get; set; }
        public string SeasonName { get; set; }
        public string Episode { get; set; }
        public ICollection<Tag> Tags { get; set; }
    }
}