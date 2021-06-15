using System.Collections.Generic;
using System.Threading.Tasks;

//Models
using Angeloid.Models;

namespace Angeloid.Services
{
    public interface ISeasonService
    {
        Task<List<string>> ListAllSeasonYear();
        Task<int> GetSeasonId(Season inputSeason);
    }
}