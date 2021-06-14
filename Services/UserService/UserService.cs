using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

//Context
using Angeloid.DataContext;

//Models
using Angeloid.Models;

namespace Angeloid.Services
{
    public class UserService : IUserService
    {
        private Context _context;
        public UserService(Context context)
        {
            _context = context;
        }

        public async Task<List<User>> ListAllUser()
        {
            var users = await (
                from user in _context.Users
                select new User
                {
                    UserId = user.UserId,
                    FacebookId = user.FacebookId,
                    UserName = user.UserName,
                    Email = user.Email,
                    Gender = user.Gender,
                    Avatar = user.Avatar,
                    IsAdmin = user.IsAdmin,
                }
            ).ToListAsync();
            return users;
        }

        public async Task<User> GetUserById(int userId)
        {
            var user = await _context.Users
                                .Where(u => u.UserId == userId)
                                .Select(
                                    u => new User
                                    {
                                        UserId = u.UserId,
                                        UserName = u.UserName,
                                        Email = u.Email,
                                        Gender = u.Gender,
                                        Avatar = u.Avatar,
                                        Fullname = u.Fullname,
                                        IsAdmin = u.IsAdmin
                                    }
                                )
                                .FirstOrDefaultAsync();
            return user;
        }

        private async Task<User> GetUser(int userId) {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User> GetUserByEmail(string email) {
            return await _context.Users
                                .Where(u => u.Email == email)
                                .Select(
                                    u => new User
                                    {
                                        UserId = u.UserId,
                                        Email = u.Email,
                                    }
                                )
                                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsEmailExist(User user)
        {
            // Find the email from db, if it exists, return true
            var existingEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            return existingEmail != null && existingEmail.UserId != user.UserId;
        }

        public async Task<int> UpdateUserInfo(User user, int userId)
        {
            var existingUser = await GetUser(userId);
            if (existingUser == null)
            {
                return 0;
            }

            existingUser.Email = user.Email;
            existingUser.Gender = user.Gender;
            existingUser.Fullname = user.Fullname;
            var rs = await _context.SaveChangesAsync();
            return rs;
        }

        public async Task<int> UpdateUserAvatar(User user, int userId)
        {
            var existingUser = await GetUser(userId);
            if (existingUser == null)
            {
                return 0;
            }
            existingUser.Avatar = user.Avatar;
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteUserById(int userId) {
            var existingUser = await GetUser(userId);
            if (existingUser == null)
            {
                return 0;
            }
            _context.Remove(existingUser);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateUserPassword(UserPassword user, int userId) {
            var existingUser = await GetUser(userId);

            if (existingUser == null || existingUser.Password != user.OldPassword)
            {
                return 0;
            }
            existingUser.Password = user.NewPassword;
            return await _context.SaveChangesAsync();
        }

        public async Task<int> ResetUserPassword(UserPassword user, int userId) {
            var existingUser = await GetUser(userId);
            if (existingUser == null)
            {
                return 0;
            }
            
            existingUser.Password = user.NewPassword;
            var rs = await _context.SaveChangesAsync();
            return rs;
        }
    }
}