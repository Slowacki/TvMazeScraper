using System.Collections.Generic;
using System.Threading.Tasks;
using TvMazeScraper.Models;

namespace TvMazeScraper.Services
{
    public interface IShowsService
    {
        Task<IEnumerable<Show>> GetAsync(int page = 0);
    }
}