using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using Angeloid.DataContext;
using Angeloid.Models;

namespace Angeloid.Services.SearchService
{
    public class SearchService
    {
        private Context _context;
        public SearchService(Context context)
        {
            _context = context;
        }
        public async Task<List<Anime>> Search(Anime searchAnime)
        {
            var animeList = await (from anime in _context.Animes
                                   where anime.AnimeName.Contains(searchAnime.AnimeName)
                                   select new Anime
                                   {
                                       AnimeId = anime.AnimeId,
                                       AnimeName = anime.AnimeName,
                                       Season = anime.Season,
                                       Thumbnail = anime.Thumbnail,
                                       Studio = anime.Studio,
                                       Episode = anime.Episode,
                                       Tags = (from tag in anime.Tags
                                               orderby tag.TagId ascending
                                               select new Tag
                                               {
                                                   TagId = tag.TagId,
                                                   TagName = tag.TagName
                                               }).Take(3).ToList(),
                                   }).ToListAsync();
            return animeList;
        }
    }
}