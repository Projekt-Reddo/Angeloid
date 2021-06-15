using System.Collections.Generic;
using System.Threading.Tasks;

//Models
using Angeloid.Models;

namespace Angeloid.Services
{
    public interface IAnimeService
    {
        Task<List<Anime>> ListAllAnime();
        Task<Anime> GetAnimes(int animeId);
        Task<int> InsertAnime(Anime anime);
        Task<int> DeleteAnime(int animeId);
        Task<int> UpdateAnime(Anime anime, int animeId);
        Task<int> isExistByAnimeName(Anime anime);
    }
}