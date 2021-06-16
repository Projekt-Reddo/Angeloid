using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

//Models
using Angeloid.Models;
using Angeloid.DataContext;
using Microsoft.EntityFrameworkCore;

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
                                       CharacterId = ch.CharacterId,
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

        public async Task<int> removeAnimeFK(int animeId)
        {
            //remove all chacter's AnimeId foreign key
            _context.Characters
                    .Where(ch => ch.AnimeId == animeId)
                    .ToList()
                    .ForEach(ch => ch.AnimeId = null);
            var rowsAffected = await _context.SaveChangesAsync();
            return rowsAffected;
        }

        public async Task<int> updateCharacter(List<Character> characters, int animeId)
        {
            var rowsAffected = 0;
            foreach (var character in characters)
            {
                // get existed Character by update anime chars id
                var existChar = await _context.Characters
                            .FirstOrDefaultAsync(ch => ch.CharacterId == character.CharacterId);

                var existSeiyuu = await _context.Seiyuus
                            .FirstOrDefaultAsync(se => se.SeiyuuName == character.Seiyuu.SeiyuuName);

                /// if seiyuu is not exist in db -> insert (new seiyuu)
                if (existSeiyuu == null)
                {
                    // add new seiyuu in db
                    _context.Seiyuus.Add(
                        new Seiyuu
                        {
                            SeiyuuName = character.Seiyuu.SeiyuuName,
                            SeiyuuImage = character.Seiyuu.SeiyuuImage
                        }
                    );
                    rowsAffected += await _context.SaveChangesAsync();
                }
                else
                {// update dbseiyuu information
                    existSeiyuu.SeiyuuName = character.Seiyuu.SeiyuuName;
                    existSeiyuu.SeiyuuImage = character.Seiyuu.SeiyuuImage;
                    rowsAffected += await _context.SaveChangesAsync();
                }

                // if character is not exitst in db -> insert (new character)
                if (existChar == null)
                {
                    var dbSeiyuu = await(from se in _context.Seiyuus
                                         where se.SeiyuuName == character.Seiyuu.SeiyuuName
                                         select new Seiyuu
                                         {
                                             SeiyuuId = se.SeiyuuId
                                         }).FirstOrDefaultAsync();
                    // add new character in db
                    _context.Characters.Add(
                        new Character
                        {
                            CharacterName = character.CharacterName,
                            CharacterRole = character.CharacterRole,
                            CharacterImage = character.CharacterImage,
                            AnimeId = animeId,
                            SeiyuuId = dbSeiyuu.SeiyuuId
                        }
                    );
                    rowsAffected += await _context.SaveChangesAsync();
                }
                else
                {
                    // if character is existed in db 
                    // get new seiyuu just updated 
                    var dbSeiyuu = await(from se in _context.Seiyuus
                                         where se.SeiyuuName == character.Seiyuu.SeiyuuName
                                         select new Seiyuu
                                         {
                                             SeiyuuId = se.SeiyuuId
                                         }).FirstOrDefaultAsync();
                    // change animeId to animeId
                    _context.Characters
                    .Where(ch => ch.CharacterId == character.CharacterId)
                    .ToList()
                    .ForEach(ch =>
                    {
                        ch.AnimeId = animeId;
                        ch.CharacterName = character.CharacterName;
                        ch.CharacterImage = character.CharacterImage;
                        ch.CharacterRole = character.CharacterRole;
                        ch.SeiyuuId = dbSeiyuu.SeiyuuId;
                    });
                    rowsAffected += await _context.SaveChangesAsync();
                }
            }
            return rowsAffected;
        }
    }
}