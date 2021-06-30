using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Angeloid.Models
{
    public partial class SearchThread
    {
        public string searchString { get; set; }
    }
}