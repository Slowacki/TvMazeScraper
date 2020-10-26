using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvMazeScraper.Models;
using TvMazeScraper.Services;

namespace TvMazeScraper.Web.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ShowsController : ControllerBase
    {
        private readonly IShowsService _showsService;

        public ShowsController(IShowsService showsService)
        {
            _showsService = showsService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Show>>> Get([FromQuery] int page = 0)
        {
            var shows = await _showsService.GetAsync(page);

            return shows.ToList();
        }
    }
}