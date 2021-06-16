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
    public class TagService : ITagService
    {
        private Context _context;
        public TagService(Context context)
        {
            _context = context;
        }

        public async Task<List<Tag>> getTagListFromAnime(Anime anime)
        {
            var inputTags = (from tg in anime.Tags
                             select new Tag
                             {
                                 TagId = tg.TagId
                             }).ToList();

            return inputTags;
        }

        public async Task<int> insertAnimeTag(List<Tag> tagList, int AnimeId)
        {
            var rowInserted = 0;

            foreach (var tag in tagList)
            {
                _context.AnimeTags.Add(
                    new AnimeTag
                    {
                        AnimeId = AnimeId,
                        TagId = tag.TagId
                    }
                );

                rowInserted += await _context.SaveChangesAsync();
            }

            return rowInserted;
        }

        public async Task<List<Tag>> ListAllTags()
        {
            var tags = await (
                from tag in _context.Tags
                select new Tag
                {
                    TagId = tag.TagId,
                    TagName = tag.TagName
                }
            ).ToListAsync();

            return tags;
        }

        public async Task<int> removeAnimeTag(int animeId)
        {
            // load animeTag model
            var animeTagList = await _context.AnimeTags
                            .Where(at => at.AnimeId == animeId)
                            .ToListAsync();
            // remove all row in AnimeTag db that AnimeId is equal updateAnimeId
            foreach (var animeTag in animeTagList)
            {
                _context.AnimeTags.Remove(animeTag);
            }
            // save change
            return await _context.SaveChangesAsync();
        }
    }
}