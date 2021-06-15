using System.Collections.Generic;
using System.Threading.Tasks;

//Models
using Angeloid.Models;

namespace Angeloid.Services
{
    public interface ITagService
    {
        Task<List<Tag>> ListAllTags();
        Task<int> insertAnimeTag(List<Tag> tagList, int AnimeId);
        Task<List<Tag>> getTagListFromAnime(Anime anime);
    }
}