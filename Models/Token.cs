using System;

namespace Angeloid.Models
{
    public partial class Token
    {
        public Token(string tokenName, DateTime exp)
        {
            TokenName = tokenName;
            Exp = exp;
        }
        public string TokenName { get; set; }
        public DateTime Exp { get; set; }
    }
}