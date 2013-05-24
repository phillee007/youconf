namespace YouConf.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using YouConf.Data;

    internal sealed class YouConfTestInitializer : DbMigrationsConfiguration<YouConfDbContext>
    {
        public YouConfTestInitializer()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(YouConf.Data.YouConfDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
