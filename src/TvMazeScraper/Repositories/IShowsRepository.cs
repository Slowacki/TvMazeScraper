using System.Collections.Generic;
using System.Threading.Tasks;
using TvMazeScraper.Models;

namespace TvMazeScraper.Repositories
{
    public interface IShowsRepository
    {
        Task<IEnumerable<Show>> GetAsync(int page = 0);

        Task InsertAsync(IEnumerable<Show> shows);
    }
}