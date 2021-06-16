using System.Collections.Generic;
using System.Threading.Tasks;
using Angeloid.Models;

namespace Angeloid.Services
{
    public interface IThreadService
    {
        Task<List<Thread>> ListThreadFirst();
        Task<List<Thread>> LoadMore(int loadId);
        Task<int> AddNewThread(Thread thread);
    }
}