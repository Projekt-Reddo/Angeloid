using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Angeloid.Models
{
    public partial class EmailConfig
    {
        public string MailAddress { get; set; }
        public string MailPassword { get; set; }
        public int MailPort { get; set; }
    }
}