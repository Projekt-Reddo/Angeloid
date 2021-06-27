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
    public class FavoriteService : IFavoriteService
    {
        private Context _context;
        private IAnimeService _animeService;
        private IUserService _userService;
        public FavoriteService(Context context, IAnimeService animeService, IUserService userService)
        {
            _context = context;
            _animeService = animeService;
            _userService = userService;
        }

        //Add new anime to user favorite list
        public async Task<int> AddToFavorite(Favorite favorite) {
            //Check if user is exist
            if (await _userService.GetUserById(favorite.UserId) == null)
            {
                return 0;
            }

            //Check if anime is exist
            if (await _animeService.isExistByAnimeId(favorite.AnimeId) == null)
            {
                return 0;
            }

            //Check duplicate favorite
            var existedFavorite = await _context.Favorites.FirstOrDefaultAsync(fa => fa.AnimeId == favorite.AnimeId && fa.UserId == favorite.UserId);
            if (existedFavorite != null) {
                return -1;
            }

            var rowsInserted = 0;

            //Add new anime and user to favorite
            _context.Favorites.Add(favorite);
            rowsInserted += await _context.SaveChangesAsync();

            return rowsInserted;
        }

        public async Task<List<Anime>> GetFavoriteList(int userId) {
            //Check if user is exist
            if (await _userService.GetUserById(userId) == null)
            {
                return null;
            }

            //Get list anime where approriate user id from Favorite table
            var favoriteList = await _context.Favorites.Where(fa => fa.UserId == userId)
                                .Select(fa => new Anime{
                                    AnimeId = fa.AnimeId,
                                    AnimeName = fa.Anime.AnimeName,
                                    Thumbnail = fa.Anime.Thumbnail
                                })
                                .ToListAsync();

            return favoriteList;
        }

        public async Task<int> DeleteFavorite(Favorite favorite) {
            //Check if user is exist
            if (await _userService.GetUserById(favorite.UserId) == null)
            {
                return 0;
            }

            //Check if anime is exist
            if (await _animeService.isExistByAnimeId(favorite.AnimeId) == null)
            {
                return 0;
            }

            var rowsDeleted = 0;

            //Check duplicate favorite
            var existedFavorite = await _context.Favorites.FirstOrDefaultAsync(fa => fa.AnimeId == favorite.AnimeId && fa.UserId == favorite.UserId);
            if (existedFavorite != null) {
                _context.Favorites.Remove(existedFavorite);
                rowsDeleted = await _context.SaveChangesAsync();
                return rowsDeleted;
            }

            return 0;
        }
    }
}