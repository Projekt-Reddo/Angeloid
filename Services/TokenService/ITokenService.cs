namespace Angeloid.Services
{
    public interface ITokenService
    {
        string createToken(int userId);
        int getUserIdByToken(string token);
        void removeToken(int userId);
    }
}