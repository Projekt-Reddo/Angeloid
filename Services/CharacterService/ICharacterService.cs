using System.Collections.Generic;
using System.Threading.Tasks;

//Models
using Angeloid.Models;


namespace Angeloid.Services
{
    public interface ICharacterService
    {
        Task<int> insertCharacter(Character character, int AnimeId, int SeiyuuId);
        Task<int> insertListCharacter(List<Character> inputCharacters, int AnimeId);
        Task<List<Character>> getCharacterListFromAnime(Anime anime);
        Task<int> removeAnimeFK(int animeId);
        Task<int> updateCharacter(List<Character> characters, int animeId);
    }
}