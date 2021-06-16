using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angeloid.DataContext;
using Angeloid.Models;
using Microsoft.EntityFrameworkCore;

namespace Angeloid.Services
{
    public class ThreadService : IThreadService
    {
        private Context _context;
        public ThreadService(Context context)
        {
            _context = context;
        }
        public async Task<List<Thread>> ListThreadFirst()
        {
            var threads = await _context.Threads
                                            .OrderByDescending(t => t.ThreadId).Include(t => t.User)
                                            .Take(10)
                                            .ToListAsync();
            return threads;
        }

        public async Task<List<Thread>> LoadMore(int loadId)
        {
             var threads = await _context.Threads
                            .Where(t=>t.ThreadId>=loadId-10 && t.ThreadId<loadId)
                            .Include(t=>t.User)
                            .OrderByDescending(t=>t.ThreadId)
                            .ToListAsync();
            return threads;
        }
        public async Task<int> AddNewThread(Thread thread)
        {
            var rowInserted = 0;
            _context.Threads.Add(thread);
            rowInserted += await _context.SaveChangesAsync();
            return rowInserted;
        }

    }
}