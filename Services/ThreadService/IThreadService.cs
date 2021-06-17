using System.Collections.Generic;
using System.Threading.Tasks;
using Angeloid.Models;

namespace Angeloid.Services
{
    public interface IThreadService
    {
        Task<List<Thread>> ListThreadFirst();
        Task<List<Thread>> LoadMore(int loadId);
        Task<Thread> GetThreadById(int userId);
        Task<List<Thread>> ListAllThread();
        Task<int> DeleteThreadById(int threadId);
    }
}