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
            AvailableToPublic = true;
        }

        [Required]
        public string HashTag { get; set; }
        [Required]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Full Description")]
        public string Description { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]  
        public string Abstract { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Start Date")]
        [DisplayFormat(NullDisplayText = "", DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "End Date")]
        [DisplayFormat(NullDisplayText = "", DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        [Required]
        [UIHint("TimeZone"), Display(Name = "Time Zone")]
        public string TimeZoneId { get; set; }
        [Display(Name = "Hangout Id")]
        public string HangoutId { get; set; }
        [Display(Name = "Twitter Widget Id")]
        public long TwitterWidgetId { get; set; }
        [Display(Name = "Available to public")]
        public bool AvailableToPublic { get; set; }
        public IList<Presentation> Presentations { get; set; }
        public IList<Speaker> Speakers { get; set; }
    }

    public class Presentation{
        public Presentation()
        {
            Speakers = new List<Speaker>();
        }
        [Required]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]  
        public string Abstract { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Start Time")]
        [DisplayFormat(NullDisplayText = "", DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; }
        [Required]
        [Display(Name = "Duration (minutes)")]
        public int Duration { get; set; }
        [Display(Name = "YouTube Video Id")]
        public string YouTubeVideoId { get; set; }
        [Display(Name="Speaker/s")]
        public IList<Speaker> Speakers { get; set; }
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
        [Display(Name = "Avatar Url")]
        public string AvatarUrl { get; set; }
    }

}