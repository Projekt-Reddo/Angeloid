using System.Collections.Generic;
using System.Threading.Tasks;

//Models
using Angeloid.Models;

namespace Angeloid.Services
{
    public interface IHomePageService
    {
        Task<List<Anime>> ListThisSeasonAnime();
        Task<List<Anime>> ListNextSeasonAnime();
        Task<List<Anime>> ListAllTimePopularAnime();
    }
}