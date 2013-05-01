using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YouConf.Data;
using YouConf.Data.Entities;

namespace YouConf.Controllers
{
    public class SpeakerController : Controller
    {
        public IYouConfDataContext YouConfDataContext { get; set; }

        public SpeakerController(IYouConfDataContext youConfDataContext)
        {
            if (youConfDataContext == null)
            {
                throw new ArgumentNullException("youConfDataContext");
            }
            YouConfDataContext = youConfDataContext;
        }

        //
        // GET: /Speaker/

        public ActionResult Index(string id)
        {
            var conference = YouConfDataContext.GetConference(id);
            if (conference == null)
            {
                return HttpNotFound();
            }
            return View(conference.Speakers);
        }

        //
        // GET: /Speaker/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Speaker/Create

        public ActionResult Add(string id)
        {
            var speaker = new Speaker();
            ViewBag.ConferenceId = id;
            return View(speaker);
        }

        //
        // POST: /Speaker/Add

        [HttpPost]
        public ActionResult Add(string conferenceHashTag, Speaker speaker)
        {
            if (ModelState.IsValid)
            {
                var conference = YouConfDataContext.GetConference(conferenceHashTag);
                if (conference == null)
                {
                    return HttpNotFound();
                }

                conference.Speakers.Add(speaker);
                YouConfDataContext.UpsertConference(conference);
                return RedirectToAction("Details", "Conference", new { hashTag = conferenceHashTag });
            }

            return View(speaker);
        }

        //
        // GET: /Speaker/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Speaker/Edit/5

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
        // GET: /Speaker/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Speaker/Delete/5

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
