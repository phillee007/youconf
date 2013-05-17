using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using YouConf.Data.Entities;

namespace YouConf.Data
{
   public class YouConfDbContext : DbContext
    {
       public YouConfDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
   }
}