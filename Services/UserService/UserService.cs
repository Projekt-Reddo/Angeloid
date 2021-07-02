using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;

//Context
using Angeloid.DataContext;

//Models
using Angeloid.Models;

namespace Angeloid.Services
{
    public class UserService : IUserService, ILogInOutService
    {
        private Context _context;
        private const string fileName = "UserLogin.txt";
        public UserService(Context context)
        {
            _context = context;
        }

        public async Task<List<User>> ListAllUser()
        {
            var users = await (
                from user in _context.Users
                where user.IsAdmin == false
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

        public async Task<List<User>> ListTopUser()
        {
            var users = await (
                from u in _context.Users
                orderby u.Threads.Count() descending
                select u
            ).Take(9).ToListAsync();
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

        private async Task<User> GetUser(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User> GetUserByEmail(string email)
        {
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

        private async Task<User> GetUserByFacebookId(string facebookId)
        {
            return await _context.Users
                                .Where(u => u.FacebookId == facebookId)
                                .Select(
                                    u => new User
                                    {
                                        UserId = u.UserId,
                                        IsAdmin = u.IsAdmin,
                                        Avatar = u.Avatar
                                    }
                                )
                                .FirstOrDefaultAsync();
        }

        private async Task<bool> IsUserNameExist(User user)
        {
            // Find the username from db, if it exists, return true.
            var existingUsername = await _context.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);
            return existingUsername != null && existingUsername.UserId != user.UserId;
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
            if (
                existingUser.Email == user.Email &&
                existingUser.Gender == user.Gender &&
                existingUser.Fullname == user.Fullname
            )
            {
                return 1;
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
            if (existingUser.Avatar == user.Avatar)
            {
                return 1;
            }

            existingUser.Avatar = user.Avatar;
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteUserById(int userId)
        {
            var existingUser = await GetUser(userId);
            if (existingUser == null)
            {
                return 0;
            }
            _context.Remove(existingUser);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateUserPassword(UserPassword user, int userId)
        {
            var existingUser = await GetUser(userId);

            if (existingUser == null || existingUser.Password != user.OldPassword)
            {
                return 0;
            }
            existingUser.Password = user.NewPassword;
            return await _context.SaveChangesAsync();
        }

        public async Task<int> ResetUserPassword(UserPassword user, int userId)
        {
            var existingUser = await GetUser(userId);
            if (existingUser == null)
            {
                return 0;
            }
            if (existingUser.Password == user.NewPassword)
            {
                return 1;
            }
            existingUser.Password = user.NewPassword;
            var rs = await _context.SaveChangesAsync();
            return rs;
        }

        public async Task<int> Register(User user)
        {
            var rowInserted = 0;
            if (await IsUserNameExist(user) || await IsEmailExist(user))
            {
                return 0;
            }
            _context.Users.Add(user);
            rowInserted += await _context.SaveChangesAsync();
            return rowInserted;
        }

        public async Task<User> Login(User user)
        {
            var _user = await _context.Users
                        .Where(u => u.UserName == user.UserName && u.Password == user.Password)
                        .Select(
                            u => new User
                            {
                                UserId = u.UserId,
                                Avatar = u.Avatar,
                                IsAdmin = u.IsAdmin
                            }
                        ).FirstOrDefaultAsync();
            AddLoginToFile();
            return _user;
        }

        public async Task<User> FacebookLogin(User user)
        {
            if (await IsEmailExist(user))
            {
                var ExistUser = await GetUserEmail(user.Email);
                ExistUser.FacebookId = user.FacebookId;
                AddLoginToFile();
                return ExistUser;
            }
            var FacebookUser = await GetUserByFacebookId(user.FacebookId);
            if (FacebookUser != null)
            {
                AddLoginToFile();
                return FacebookUser;
            }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            FacebookUser = await GetUserByFacebookId(user.FacebookId);
            AddLoginToFile();
            return FacebookUser;
        }

        public Task<User> Logout(User user)
        {
            return null;
        }

        private async Task<User> GetUserEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        // Count and Add time to file when any user login to web
        private void AddLoginToFile()
        {
            DateTime current = DateTime.Now;
            string[] lines = File.ReadAllLines(fileName);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] == current.Year.ToString())
                {
                    for (int j = 1; j < 13; j++)
                    {
                        if (j == current.Month)
                        {
                            lines[i + j] = (int.Parse(lines[i + j]) + 1).ToString();
                        }
                    }
                    break;
                }
            }
            File.WriteAllLines(fileName, lines);
        }

        // List all Login Time Of User in current year
        public List<string> ReadLoginFromFile()
        {
            DateTime current = DateTime.Now;
            string[] lines = File.ReadAllLines(fileName);
            List<string> listView = new List<string>();
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] == current.Year.ToString())
                {
                    for (int j = 1; j < 13; j++)
                    {
                        listView.Add(lines[i + j]);
                    }
                }
            }
            return listView;
        }
    }
}