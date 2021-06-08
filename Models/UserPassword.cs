using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Angeloid.Models
{
    public partial class UserPassword
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}