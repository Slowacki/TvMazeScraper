using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TvMazeScraper.Services;
using TvMazeScraper.Web.Dtos;

namespace TvMazeScraper.Web.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ShowsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IShowsService _showsService;

        public ShowsController(IShowsService showsService,
            IMapper mapper)
        {
            _showsService = showsService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShowDto>>> Get([FromQuery] int page = 0)
        {
            var shows = await _showsService.GetAsync(page);

            return Ok(_mapper.Map<IEnumerable<ShowDto>>(shows));
        }
    }
}