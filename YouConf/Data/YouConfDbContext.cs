using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using YouConf.Data.Entities;

namespace YouConf.Data
{
   public class YouConfDbContext : DbContext, IYouConfDbContext
    {
       public YouConfDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Conference> Conferences { get; set; }
        public DbSet<Speaker> Speakers { get; set; }
        public DbSet<Presentation> Presentations { get; set; }
   }
}