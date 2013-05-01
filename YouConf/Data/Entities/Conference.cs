using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YouConf.Data.Entities
{

    public class AzureTableEntity : TableEntity
    {
        public string Entity { get; set; }
    }

    public class Conference
    {
        public Conference()
        {
            Presentations = new List<Presentation>();
            Speakers = new List<Speaker>();
        }

        public string HashTag { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<Presentation> Presentations { get; set; }
        public IList<Speaker> Speakers { get; set; }
        public string Abstract { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndTime { get; set; }
        public string TimeZone { get; set; }
    }

    public class Presentation{
        public Presentation()
        {
            Speakers = new List<Speaker>();
        }
        public string Name { get; set; }
        public string Abstract { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public string TimeZone { get; set; }
        public IEnumerable<Speaker> Speakers { get; set; }
    }
    public class Speaker{
        public string Name { get; set; }
        public string Bio { get; set; }
        public string Url { get; set; }
        public string Email { get; set; }
    }

}