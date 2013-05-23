using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YouConf.Data;

namespace YouConf.Controllers
{
    public class PascalsPyramidController : Controller
    {
        //
        // GET: /PascalsPyramid/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(PascalsPyramidModel model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new YouConfDbContext())
                {
                    //Run it once to warm it up, that way we're actually testing the proc, and not the connection to SQL and Entity framework etc
                    var nthRow = db.Database.SqlQuery<string>("exec GetPascalTriangleNthRow @RowNumber", new SqlParameter("@RowNumber", model.RowNumber)).First();

                    // Create new stopwatch
                    Stopwatch stopwatch = new Stopwatch();

                    // Begin timing
                    stopwatch.Start();

                    for (int i = 0; i < 10; i++)
                    {
                       db.Database.SqlQuery<string>("exec GetPascalTriangleNthRow @RowNumber", new SqlParameter("@RowNumber", model.RowNumber)).First();
                    }

                    var elapsed = stopwatch.Elapsed;
                    ViewBag.TotalTimeElapsed = elapsed;
                    ViewBag.NthRow = nthRow;
                    return View(model);
                }
            }

            return View(model);
        }

    }

    public class PascalsPyramidModel
    {
        [Range(1, 67, ErrorMessage = "The maximum row number you can enter without an arithmetic overflow exception is 67")]
        public int RowNumber { get; set; }
    }
}
