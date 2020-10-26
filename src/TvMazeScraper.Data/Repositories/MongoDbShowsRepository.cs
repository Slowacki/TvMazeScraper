using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using TvMazeScraper.Models;
using TvMazeScraper.Repositories;

namespace TvMazeScraper.Data.Repositories
{
    public class MongoDbShowsRepository : IShowsRepository
    {
        private readonly IMongoCollection<Show> _showCollection;

        public MongoDbShowsRepository(IMongoCollection<Show> showCollection)
        {
            _showCollection = showCollection;
        }

        public async Task<IEnumerable<Show>> GetAsync(int page = 0)
        {
            return (await _showCollection.FindAsync(s => s.Id >= page * 250 && s.Id < (page + 1) * 250)).ToList();
        }

        public async Task InsertAsync(IEnumerable<Show> shows)
        {
            try
            {
                await _showCollection.InsertManyAsync(shows);
            }
            catch (MongoBulkWriteException ex) when (ex.WriteErrors.All(er => er.Category == ServerErrorCategory.DuplicateKey))
            {
                // Log and forget
            }
        }
    }
}