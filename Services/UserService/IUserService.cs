using System.Collections.Generic;
using System.Threading.Tasks;

//Models
using Angeloid.Models;

namespace Angeloid.Services
{
    public interface IUserService
    {
        Task<List<User>> ListAllUser();
        Task<User> GetUserById(int userId);
        Task<User> GetUserByEmail(string email);
        Task<bool> IsEmailExist(User user);
        Task<int> UpdateUserInfo(User user, int userId);
        Task<int> UpdateUserAvatar(User user, int userId);
        Task<int> DeleteUserById(int userId);
        Task<int> UpdateUserPassword(UserPassword user, int userId);
        Task<int> ResetUserPassword(UserPassword user, int userId);
    }
}