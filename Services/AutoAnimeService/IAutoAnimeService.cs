using System.Collections.Generic;
using System.Threading.Tasks;

//Models
using Angeloid.Models;

namespace Angeloid.Services
{
    public interface IAutoAnimeService
    {
        Task<int> AutoAddAnime();
    }
}