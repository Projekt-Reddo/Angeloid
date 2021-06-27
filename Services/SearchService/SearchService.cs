using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using Angeloid.DataContext;
using Angeloid.Models;

namespace Angeloid.Services
{
    public class SearchService : ISearchService
    {
        private Context _context;
        public SearchService(Context context)
        {
            _context = context;
        }
        public async Task<List<Anime>> Search(Search search)
        {
            // Find anime by Name, Year & SeasonName
            var animeList = await this._context.Animes
                                    .Select(ani => new Anime {
                                        AnimeId = ani.AnimeId,
                                        AnimeName = ani.AnimeName,
                                        Tags = ani.Tags,
                                        Studio = ani.Studio,
                                        Thumbnail = ani.Thumbnail,
                                        Season = ani.Season,
                                        Episode = ani.Episode
                                    })
                                    .Where(ani => ani.AnimeName.Contains(search.AnimeName) && ani.Season.Year.Contains(search.Year) && ani.Season.SeasonName.Contains(search.SeasonName))
                                    .ToListAsync();

            // Filter by Episode only when Search.Episode is 1 (Oneshot) or 2 (Series)
            if (Int32.Parse(search.Episode) > 0) {
                animeList = filterByEpisode(animeList, search);
            }

            // If have Tags, then do filter
            if (search.Tags.Count != 0) {
                animeList = filterByTags(animeList, search);
            }

            return animeList;
        }

        private List<Anime> filterByEpisode(List<Anime> animeList, Search search) {
            if (Int32.Parse(search.Episode) == 1) {
                // Find anime with Episode == 1
                return animeList.Where(ani => ani.Episode == "1").ToList();
            } 
            // Find anime with Episode != 1 and NOT NULL
            return animeList.Where(ani => ani.Episode != null && ani.Episode.ToLower() != "null" && ani.Episode != "1").ToList();
        }

        private List<Anime> filterByTags(List<Anime> animeList, Search search) {
            foreach (var item in search.Tags) {
                animeList = (from ani in animeList
                            from tag in ani.Tags
                            where tag.TagId == item.TagId
                            select ani).ToList();
            }
            return animeList;
        }
    }
}