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

        public ActionResult All()
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
            if (ModelState.IsValid)
            {
                YouConfDataContext.UpsertConference(conference.HashTag, conference);
                return RedirectToAction("Details", new { hashTag = conference.HashTag });
            }
            return View(conference);
        }

        //
        // GET: /Conference/Edit/5

        public ActionResult Edit(string hashTag)
        {
            var conference = YouConfDataContext.GetConference(hashTag);
            if (conference == null)
            {
                return HttpNotFound();
            }
            return View(conference);
        }

        //
        // POST: /Conference/Edit/5

        [HttpPost]
        public ActionResult Edit(string id, Conference conference)
        {

            if (ModelState.IsValid)
            {
                var existingConference = YouConfDataContext.GetConference(id);
                if (conference == null)
                {
                    return HttpNotFound();
                }

                //Could use Automapper or similar to map the new conference details onto the old so we don't lose any child properties e.g. Speakers/Presentations.
                //We'll do it manually for now
                conference.Speakers = existingConference.Speakers;
                conference.Presentations = existingConference.Presentations;

                YouConfDataContext.UpsertConference(id, conference);

                return RedirectToAction("Details", new { hashTag = conference.HashTag });
            }

            return View(conference);
        }

        //
        // GET: /Conference/Delete/5

        public ActionResult Delete(string hashTag)
        {
            var conference = YouConfDataContext.GetConference(hashTag);
            if (conference == null)
            {
                return HttpNotFound();
            }

            return View(conference);
        }

        //
        // POST: /Conference/Delete/5

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirm(string hashTag)
        {
            var conference = YouConfDataContext.GetConference(hashTag);
            if (conference == null)
            {
                return HttpNotFound();
            }

            YouConfDataContext.DeleteConference(hashTag);

            return RedirectToAction("All");
        }

        public ActionResult Live(string hashTag)
        {
            var conference = YouConfDataContext.GetConference(hashTag);
            if (conference == null)
            {
                return HttpNotFound();
            }
            return View(conference);
        }
    }
}
