using System;
using System.Threading.Tasks;

namespace Angeloid.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string guid);
    }
}