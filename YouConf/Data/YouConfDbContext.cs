using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
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

       public YouConfDbContext(string connectionString)
           : base(connectionString)
       {
       }

       public YouConfDbContext(DbConnection connection)
           : base(connection, true)
       {
       }


        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Conference> Conferences { get; set; }
        public DbSet<Speaker> Speakers { get; set; }
        public DbSet<Presentation> Presentations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
   }
}