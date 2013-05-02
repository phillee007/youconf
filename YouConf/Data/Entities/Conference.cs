using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Required]
        public string HashTag { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]  
        public string Description { get; set; }
        public IList<Presentation> Presentations { get; set; }
        public IList<Speaker> Speakers { get; set; }
        [DataType(DataType.MultilineText)]  
        public string Abstract { get; set; }
        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Required]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        [Required]
        [UIHint("TimeZone"), Display(Name = "Time Zone")]
        public string TimeZoneId { get; set; }
        public string HangoutUrl { get; set; }
        public string TwitterFeed { get; set; }
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
        [Required]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]  
        public string Bio { get; set; }
        public string Url { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
    }

}