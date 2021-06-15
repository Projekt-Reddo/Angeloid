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
    public class StudioService : IStudioService
    {
        private Context _context;
        public StudioService(Context context)
        {
            _context = context;
        }

        public async Task<List<Studio>> ListAllStudio()
        {
            var studios = await(
                from studio in _context.Studios
                where studio.StudioName != ""
                select new Studio
                {
                    StudioId = studio.StudioId,
                    StudioName = studio.StudioName
                }
            ).ToListAsync();

            return studios;
        }
    }
}