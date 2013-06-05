using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YouConf.Common.Data;
using YouConf.Common.Messaging;

namespace YouConf.Controllers
{
    [Authorize(Roles="Administrators")]
    public class AdminController : BaseController
    {
         public IYouConfDbContext YouConfDbContext { get; set; }

        public AdminController(IYouConfDbContext youConfDbContext)
        {
            if (youConfDbContext == null)
            {
                throw new ArgumentNullException("youConfDbContext");
            }
            YouConfDbContext = youConfDbContext;
        }
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ReIndex(){
            var conferences = YouConfDbContext.Conferences.ToList();
            foreach (var conference in conferences)
            {
                UpdateConferenceInSolrIndex(conference.Id, SolrIndexAction.Update);
            }

            return View("Index");
        }

        [HttpPost]
        public ActionResult DeleteAll()
        {
            var conferences = YouConfDbContext.Conferences.ToList();
            foreach (var conference in conferences)
            {
                UpdateConferenceInSolrIndex(conference.Id, SolrIndexAction.Delete);
            }

            return View("Index");
        }
    }
}
