using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Angeloid.Models;

namespace Angeloid.Services
{
    public interface ISearchService
    {
        Task<List<Anime>> Search(Anime anime);
    }
}