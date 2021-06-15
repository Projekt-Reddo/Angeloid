using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

//Models
using Angeloid.Models;
using Angeloid.DataContext;

namespace Angeloid.Services
{
    public class CharacterService : ICharacterService
    {
        private Context _context;
        private ISeiyuuService _seiyuuService;
        public CharacterService(Context context, ISeiyuuService seiyuuService)
        {
            _context = context;
            _seiyuuService = seiyuuService;
        }

        public async Task<List<Character>> getCharacterListFromAnime(Anime anime)
        {
            var inputCharacters = (from ch in anime.Characters
                                    select new Character
                                    {
                                        CharacterName = ch.CharacterName,
                                        CharacterRole = ch.CharacterRole,
                                        CharacterImage = ch.CharacterImage,
                                        Seiyuu = ch.Seiyuu
                                    }).ToList();

            return inputCharacters;
        }

        public async Task<int> insertCharacter(Character character, int AnimeId, int SeiyuuId)
        {
            _context.Characters.Add(
                    new Character
                    {
                        CharacterName = character.CharacterName,
                        CharacterRole = character.CharacterRole,
                        CharacterImage = character.CharacterImage,
                        AnimeId = AnimeId,
                        SeiyuuId = SeiyuuId
                    }
                );

            var rowInserted = await _context.SaveChangesAsync();

            return rowInserted;
        }

        public async Task<int> insertListCharacter(List<Character> inputCharacters, int AnimeId)
        {
            var rowInserted = 0;
            // insert characters and seiyuu info
            foreach (var character in inputCharacters)
            {
                // if seiyuu is not exist in db -> insert (new seiyuu)
                if (await _seiyuuService.isExistBySeiyuuName(character.Seiyuu) == 0)
                {
                    // add new seiyuu to db
                    await _seiyuuService.insertSeiyuu(character.Seiyuu);
                }

                //Get seiyuuId from inserted seiyuu
                var insertedSeiyuuId = await _seiyuuService.isExistBySeiyuuName(character.Seiyuu);

                // add new character to db
                await insertCharacter(character, AnimeId, insertedSeiyuuId);
            }

            return rowInserted;
        }
    }
}