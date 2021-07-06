using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Newtonsoft;
using Newtonsoft.Json.Linq;

//Models
using Angeloid.Models;
using Angeloid.DataContext;
using Angeloid.Controllers;


namespace Angeloid.Services
{
    public class AutoAnimeService : IAutoAnimeService
    {
        private Context _context;
        private readonly HttpClient _client;
        private string _api;
        public AutoAnimeService(Context context)
        {
            _context = context;
            _client = new HttpClient();
            _api = "https://api.jikan.moe/v3/";
        }

        public async Task<int> AutoAddAnime() {
            try {
                // Get this DateTime.Now Anime Season
                Season season = getSeason();
                // Fetch API
                var json = await fetch(_api + $"season/{season.Year}/{season.SeasonName.ToLower()}");

                if (json == null) {
                    return 0;
                }

                // Get List of animeId
                List<string> animeIdList = new List<string>();
                JArray animeJsonList = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(json["anime"].ToString());
                foreach (JObject item in animeJsonList) {
                    animeIdList.Add(item["mal_id"].ToString());
                    if (animeIdList.Count == 36) break;
                }

                await handleAddListAnime(animeIdList, season);

                return 1;
            } catch (Exception ex) {
                return 0;
            }
        }

        private Season getSeason() {
            Season season = new Season();
            season.Year = DateTime.Now.Year.ToString();
            season.SeasonName = Helper.getSeasonInText(DateTime.Now);

            return season;
        }

        private async Task handleAddListAnime(List<string> animeIdList, Season season) {
            foreach (string str in animeIdList) {
                await handleAddAnimeToDb(str, season);
                await Task.Delay(5000);
            }
        }

        private async Task handleAddAnimeToDb(string animeIdUrl, Season season) {
            try {
                // Fetch API
                var json = await fetch(_api + $"anime/{animeIdUrl}");

                if (json == null) {
                    return;
                }

                // If anime exist, return;
                string animeName = json["title"].ToString();
                Anime animeFromDb = await this._context.Animes.FirstOrDefaultAsync(a => a.AnimeName == animeName);
                if(animeFromDb != null) {
                    return;
                }

                // Anime information
                Anime anime = new Anime();
                anime.AnimeName = json["title"].ToString();
                anime.Content = json["synopsis"].ToString();
                anime.Thumbnail = await fetchImage(json["image_url"].ToString());
                anime.Status = json["status"].ToString();
                anime.Trailer = json["trailer_url"].ToString();
                anime.View = 0;
                anime.EpisodeDuration = json["duration"].ToString();
                // Avoid episode null
                try {
                    anime.Episode = json["episodes"].ToString();
                    int testAnimeEpisode = Int32.Parse(anime.Episode);
                } catch (Exception ex) {
                    anime.Episode = "0";
                }
                // Avoid startDay null
                try {
                    anime.StartDay = json["aired"]["string"].ToString().Split(" to")[0];
                } catch (Exception ex) {
                    anime.StartDay = "";
                }
                anime.Web = json["url"].ToString();
                // Avoid no studio
                try {
                    anime.StudioId = (int) json["studios"][0]["mal_id"];
                } catch (Exception ex) {
                    anime.StudioId = 9;
                }

                // Check Season exist, if not then add to db
                Season seasonFromDb = await this._context.Seasons
                    .FirstOrDefaultAsync(s => s.SeasonName == season.SeasonName && s.Year == season.Year);
                if (seasonFromDb == null) {
                    await this._context.Seasons.AddAsync(season);
                    await this._context.SaveChangesAsync();
                    seasonFromDb = season;
                }
                anime.SeasonId = seasonFromDb.SeasonId;

                // Add Anime to Db
                await this._context.Animes.AddAsync(anime);
                await this._context.SaveChangesAsync();

                // Not added -> Id = 0
                if (anime.AnimeId == 0) {
                    return;
                }

                JArray tagList = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(json["genres"].ToString());
                if (tagList.Count != 0) {
                    foreach (JObject tag in tagList) {
                        await handleAddAnimeTagToDb(tag["mal_id"].ToString(), anime.AnimeId);
                    }
                }

                await Task.Delay(5000);
                await handleAddCharacterList(animeIdUrl, anime.AnimeId);

                System.Console.WriteLine(anime.AnimeId);
            } catch (Exception ex) {
                System.Console.WriteLine(ex);
                return;
            }
        }

        private async Task handleAddAnimeTagToDb(string tagIdStr, int animeId) {
            try {
                int tagId = Int32.Parse(tagIdStr);
                AnimeTag animeTag = new AnimeTag();
                animeTag.AnimeId = animeId;
                animeTag.TagId = tagId;

                await this._context.AnimeTags.AddAsync(animeTag);
                await this._context.SaveChangesAsync();
            } catch (Exception ex) {
                System.Console.WriteLine(ex);
                return;
            }
        }

        private async Task handleAddCharacterList(string animeIdUrl, int animeId) {
            try {
                // Fetch API
                var json = await fetch(_api + $"anime/{animeIdUrl}/characters_staff");

                if (json == null) {
                    return;
                }

                JArray characterList = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(json["characters"].ToString());

                if (characterList.Count == 0) {
                    return;
                }

                foreach (JObject character in characterList) {
                    await handleAddCharacterToDb(character, animeId);
                }
            } catch (Exception ex) {
                System.Console.WriteLine(ex);
                return;
            }
        }

        private async Task handleAddCharacterToDb(JObject characterJson, int animeId) {
            try {
                Seiyuu seiyuu = new Seiyuu();
                JArray seiyuuList = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(characterJson["voice_actors"].ToString());
                if (seiyuuList.Count != 0) {
                    foreach (JObject sei in seiyuuList)
                    {
                        if (sei["language"].ToString() == "Japanese") {
                            seiyuu.SeiyuuName = sei["name"].ToString().Replace(",", "");
                            seiyuu.SeiyuuImage = await fetchImage(sei["image_url"].ToString());
                        }
                    }
                }

                // Check seiyuu exist
                Seiyuu seiyuuFromDb = await this._context.Seiyuus.FirstOrDefaultAsync(s => s.SeiyuuName == seiyuu.SeiyuuName);
                if (seiyuuFromDb == null) {
                    await this._context.Seiyuus.AddAsync(seiyuu);
                    await this._context.SaveChangesAsync();
                    seiyuuFromDb = seiyuu;
                }

                // Check character exist
                string characterName = characterJson["name"].ToString().Replace(",", "");
                Character characterFromDb = await this._context.Characters.FirstOrDefaultAsync(c => c.CharacterName == characterName);
                if (characterFromDb == null) {
                    Character character = new Character();
                    character.CharacterName = characterName;
                    character.CharacterRole = characterJson["role"].ToString();
                    character.CharacterImage = await fetchImage(characterJson["image_url"].ToString());
                    character.SeiyuuId = seiyuuFromDb.SeiyuuId;
                    character.AnimeId = animeId;
                    await this._context.Characters.AddAsync(character);
                    await this._context.SaveChangesAsync();
                }
            } catch (Exception ex) {
                System.Console.WriteLine(ex);
                return;
            }
        }

        // Fetch API, return Json String
        private async Task<Newtonsoft.Json.Linq.JObject> fetch(string url) {
            try {
                // Fetch API
                HttpResponseMessage res = await _client.GetAsync(url);
                HttpContent content = res.Content;
                // Parse Json
                var data = await content.ReadAsStringAsync();
                if (data == null) return null;

                var json = JObject.Parse(data);

                return json;
            } catch (Exception ex) {
                return null;
            }
        }

        // Fetch Image, return buffer array
        private async Task<byte[]> fetchImage(string url) {
            try {
                // Fetch API
                HttpResponseMessage res = await _client.GetAsync(url);
                HttpContent content = res.Content;
                // Parse Json
                var data = await content.ReadAsByteArrayAsync();
                if (data == null) return null;

                return data;
            } catch (Exception ex) {
                return null;
            }
        }
    }
}