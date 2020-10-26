using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TvMazeScraper.Models
{
    public class Show
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public List<Person> Cast { get; set; }

        [IgnoreDataMember]
        public DateTime CreatedAt { get; set; }
    }
}