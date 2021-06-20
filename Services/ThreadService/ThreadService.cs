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

        public async Task<int> DeleteThreadById(int threadId)
        {
            var thread = await _context.Threads.FirstOrDefaultAsync(t => t.ThreadId == threadId);

            if (thread == null) {
                return 0;
            }

            _context.Threads.Remove(thread);
            var rowDeleted = await _context.SaveChangesAsync();

            return rowDeleted;
        }

        public async Task<Thread> GetThreadById(int threadId)
        {
            var thread = await _context.Threads.Where(t => t.ThreadId == threadId)
                            .Select(t => new Thread
                            {
                                ThreadId = t.ThreadId,
                                Title = t.Title,
                                Content = t.Content,
                                Image = t.Image,
                                UserId = t.UserId,
                                User = t.User
                            }).FirstOrDefaultAsync();

            return thread;
        }

        public async Task<List<Thread>> ListAllThread()
        {
            var threads = await _context.Threads
                            .Select(t => new Thread {
                                ThreadId = t.ThreadId,
                                Title = t.Title,
                                Content = t.Content,
                                Image = t.Image,
                                UserId = t.UserId,
                                User = t.User
                            })
                            .ToListAsync();
            
            return threads;
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