using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

//Models
using Angeloid.Models;
using Angeloid.DataContext;

namespace Angeloid.Services
{
    public class SeasonService : ISeasonService
    {
        private Context _context;
        public SeasonService(Context context) {
            _context = context;
        }

        public async Task<int> GetSeasonId(Season inputSeason)
        {

            var dbSeason = await (from season in _context.Seasons
                            where season.SeasonName == inputSeason.SeasonName && season.Year == inputSeason.Year
                            select new Season
                            {
                                SeasonId = season.SeasonId
                            }).FirstOrDefaultAsync();

            return dbSeason.SeasonId;
        }

        public async Task<List<string>> ListAllSeasonYear()
        {
            var seasons = await _context.Seasons
                                            .OrderBy(x => x.Year)
                                            .Select(x => x.Year)
                                            .Distinct()
                                            .ToListAsync();
            
            return seasons;
        }
    }
}