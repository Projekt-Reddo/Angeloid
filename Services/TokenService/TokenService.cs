using System;
using System.Collections.Generic;

//Models
using Angeloid.Models;

namespace Angeloid.Services
{
    public class TokenService : ITokenService
    {
        private Dictionary<int, Token> _tokenList;
        public TokenService()
        {
            this._tokenList = new Dictionary<int, Token>();
        }
        public string createToken(int userId)
        {
            removeExpired();
            // Guid: random hash string
            string newGuid = Guid.NewGuid().ToString();
            // Expire time: 30 min
            DateTime expireTime = DateTime.Now.AddMinutes(30);

            Token newToken = new Token(newGuid, expireTime);

            if (this._tokenList.ContainsKey(userId))
            {
                this._tokenList.Remove(userId);
            }
            this._tokenList.Add(userId, newToken);
            return newGuid;
        }

        public int getUserIdByToken(string token)
        {
            //Traverse through the Dictionaries
            foreach (KeyValuePair<int, Token> entry in this._tokenList)
            {
                //Check if token exist
                if (entry.Value.TokenName == token)
                {
                    // If expire time < now remove the token
                    if (DateTime.Compare(entry.Value.Exp, DateTime.Now) < 0)
                    {
                        this._tokenList.Remove(entry.Key);
                        return 0;
                    }
                    return entry.Key;
                }
            }
            return 0;
        }

        private void removeExpired()
        {
            foreach (KeyValuePair<int, Token> entry in this._tokenList)
            {
                if (DateTime.Compare(entry.Value.Exp, DateTime.Now) < 0)
                {
                    this._tokenList.Remove(entry.Key);
                }
            }
        }

        public void removeToken(int userId)
        {
            this._tokenList.Remove(userId);
        }
    }
}