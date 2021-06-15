using System.Collections.Generic;
using System.Threading.Tasks;

//Models
using Angeloid.Models;

namespace Angeloid.Services
{
    public interface ILogInOutService
    {
        Task<User> Login(User user);
        Task<User> Logout(User user);
    }
}