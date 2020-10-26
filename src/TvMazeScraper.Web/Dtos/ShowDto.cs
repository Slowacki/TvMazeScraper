using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TvMazeScraper.Web.Dtos
{
    public class ShowDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public List<PersonDto> Cast { get; set; }

        [IgnoreDataMember]
        public DateTime CreatedAt { get; set; }
    }
}