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
    public class SeiyuuService : ISeiyuuService
    {
        private Context _context;
        public SeiyuuService(Context context)
        {
            _context = context;
        }

        public async Task<int> insertSeiyuu(Seiyuu inputSeiyuu)
        {
            _context.Seiyuus.Add(
                        new Seiyuu
                        {
                            SeiyuuName = inputSeiyuu.SeiyuuName,
                            SeiyuuImage = inputSeiyuu.SeiyuuImage
                        }
                    );

            var rowInserted = await _context.SaveChangesAsync();

            return rowInserted;
        }

        //Return seiyuu id if seiyuu is exited else return 0
        public async Task<int> isExistBySeiyuuName(Seiyuu inputSeiyuu)
        {
            var existSeiyuu = await _context.Seiyuus
                            .FirstOrDefaultAsync(se => se.SeiyuuName == inputSeiyuu.SeiyuuName);

            if (existSeiyuu != null) return existSeiyuu.SeiyuuId;

            return 0;
        }
    }
}