using System.Collections.Generic;
using System.Threading.Tasks;

//Models
using Angeloid.Models;

namespace Angeloid.Services
{
    public interface IStudioService
    {
        Task<List<Studio>> ListAllStudio();
    }
}