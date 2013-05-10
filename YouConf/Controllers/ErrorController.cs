using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YouConf.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult ServerError()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

    }
}
