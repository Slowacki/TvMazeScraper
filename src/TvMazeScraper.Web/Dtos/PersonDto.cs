using System;

namespace TvMazeScraper.Web.Dtos
{
    public class PersonDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public DateTime? Birthday { get; set; }
    }
}