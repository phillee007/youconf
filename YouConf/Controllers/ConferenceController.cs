using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YouConf.Data;
using YouConf.Data.Entities;

namespace YouConf.Controllers
{
    public class ConferenceController : Controller
    {
        public IYouConfDataContext YouConfDataContext { get; set; }

        public ConferenceController(IYouConfDataContext youConfDataContext)
        {
            if (youConfDataContext == null)
            {
                throw new ArgumentNullException("youConfDataContext");
            }
            YouConfDataContext = youConfDataContext;
        }
        //
        // GET: /Conference/

        public ActionResult Index()
        {
            var conferences = YouConfDataContext.GetAllConferences();
            return View(conferences);
        }

        //
        // GET: /Conference/Details/5

        public ActionResult Details(string hashTag)
        {
            var conference = YouConfDataContext.GetConference(hashTag);
            if (conference == null)
            {
                return HttpNotFound();
            }
            return View(conference);
        }

        //
        // GET: /Conference/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Conference/Create

        [HttpPost]
        public ActionResult Create(Conference conference)
        {
            if (!ModelState.IsValid)
            {
                return View(conference);
            }

            YouConfDataContext.UpsertConference(conference);
            return RedirectToAction("Index");
        }

        //
        // GET: /Conference/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Conference/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Conference/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Conference/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
