using System.Collections.Generic;

namespace TvMazeScraper.Web.Dtos
{
    public class ShowDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public List<PersonDto> Cast { get; set; }
    }
}