using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouConf.Data;

namespace YouConf.Tests.Integration
{
    public class TestBase
    {
        protected YouConfDbContext _context;

        [TestInitialize]
        public void SetupTest()
        {
            //Database.SetInitializer<YouConfDbContext>(null);
            Database.SetInitializer(new DropCreateDatabaseAlways<YouConfDbContext>());

            using (var context = new YouConfDbContext())
            {
                context.Database.Initialize(true);
            }

            _context = new YouConfDbContext();
        }
    }
}
