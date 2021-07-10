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

            if (thread == null)
            {
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
                            .Select(t => new Thread
                            {
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
            var threads = await (
                from t in _context.Threads
                orderby t.ThreadId descending
                select new Thread
                {
                    ThreadId = t.ThreadId,
                    Title = t.Title,
                    Content = t.Content,
                    Image = t.Image,
                    UserId = t.UserId,
                    User = new User
                    {
                        UserId = t.UserId,
                        UserName = t.User.UserName,
                        Avatar = t.User.Avatar
                    }
                }
            ).Take(10).ToListAsync();
            return threads;
        }
        public async Task<List<Thread>> LoadMore(int loadId)
        {
            var threads = await (
                from t in _context.Threads
                where t.ThreadId < loadId
                orderby t.ThreadId descending
                select new Thread
                {
                    ThreadId = t.ThreadId,
                    Title = t.Title,
                    Content = t.Content,
                    Image = t.Image,
                    UserId = t.UserId,
                    User = new User
                    {
                        UserId = t.UserId,
                        UserName = t.User.UserName,
                        Avatar = t.User.Avatar
                    }
                }
            ).Take(10).ToListAsync();
            return threads;
        }
        public async Task<int> AddNewThread(Thread thread)
        {
            var rowInserted = 0;
            var existingUsername = await _context.Users.FirstOrDefaultAsync(u => u.UserId == thread.UserId);
            if (existingUsername == null)
            {
                return 0;
            }
            _context.Threads.Add(thread);
            rowInserted += await _context.SaveChangesAsync();
            return rowInserted;
        }

        // List all searched thread with text key
        public async Task<List<Thread>> SearchThread(SearchThread searchString)
        {
            // Query get all thread contain text key in database
            var threads = await _context.Threads
                            .Where(t => t.Title.ToLower().Contains(searchString.searchString.ToLower()))
                            .OrderByDescending(t => t.ThreadId)
                            .Select(t => new Thread
                            {
                                ThreadId = t.ThreadId,
                                Title = t.Title,
                                Content = t.Content,
                                Image = t.Image,
                                UserId = t.UserId,
                                User = new User
                                {
                                    UserId = t.UserId,
                                    UserName = t.User.UserName,
                                    Avatar = t.User.Avatar
                                }
                            })
                            .ToListAsync();
            return threads;
        }

    }
}