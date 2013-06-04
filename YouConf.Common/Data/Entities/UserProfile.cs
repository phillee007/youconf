using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace YouConf.Common.Data.Entities
{
    [Table("UserProfile")]
    public class UserProfile
    {
        public UserProfile()
        {
            ConferencesAdministering = new List<Conference>();
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        [MaxLength(250)]
        public string Email { get; set; }
        public virtual IList<Conference> ConferencesAdministering { get; set; }
    }
}