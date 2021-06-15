using System.Collections.Generic;
using System.Threading.Tasks;

//Models
using Angeloid.Models;


namespace Angeloid.Services
{
    public interface ISeiyuuService
    {
        Task<int> isExistBySeiyuuName(Seiyuu inputSeiyuu);
        Task<int> insertSeiyuu(Seiyuu inputSeiyuu);
    }
}