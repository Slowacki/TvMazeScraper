using AutoMapper;
using TvMazeScraper.Models;
using TvMazeScraper.Web.Dtos;

namespace TvMazeScraper.Web.Configuration.AutoMapper
{
    public class ShowsProfile : Profile
    {
        public ShowsProfile()
        {
            CreateMap<Show, ShowDto>();
            CreateMap<Person, PersonDto>();
        }
    }
}