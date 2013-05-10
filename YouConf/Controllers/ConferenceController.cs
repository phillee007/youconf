using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YouConf.Data;
using YouConf.Data.Entities;
using YouConf.SignalRHubs;

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
            var conferences = YouConfDataContext
                .GetAllConferences()
                .Where(x => x.AvailableToPublic)
                .ToList();
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
            var model = new Conference();
            return View(model);
        }

        //
        // POST: /Conference/Create

        [HttpPost]
        public ActionResult Create(Conference conference)
        {
            if (!IsConferenceHashTagAvailable(conference.HashTag))
            {
                ModelState.AddModelError("HashTag", "Unfortunately that hashtag is not available.");
            }

            if (ModelState.IsValid)
            {
                var conferenceTimeZone = TimeZoneInfo.FindSystemTimeZoneById(conference.TimeZoneId);
                conference.StartDate = TimeZoneInfo.ConvertTimeToUtc(conference.StartDate, conferenceTimeZone);
                conference.EndDate = TimeZoneInfo.ConvertTimeToUtc(conference.EndDate, conferenceTimeZone);

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
            //If the user has changed the conference hashtag we have to make sure that the new one hasn't already been taken
            if (id != conference.HashTag && !IsConferenceHashTagAvailable(conference.HashTag))
            {
                ModelState.AddModelError("HashTag", "Unfortunately that hashtag is not available.");
            }

            if (ModelState.IsValid)
            {
                var existingConference = YouConfDataContext.GetConference(id);
                if (conference == null)
                {
                    return HttpNotFound();
                }

                var conferenceTimeZone = TimeZoneInfo.FindSystemTimeZoneById(conference.TimeZoneId);
                conference.StartDate = TimeZoneInfo.ConvertTimeToUtc(conference.StartDate, conferenceTimeZone);
                conference.EndDate = TimeZoneInfo.ConvertTimeToUtc(conference.EndDate, conferenceTimeZone);

                //Could use Automapper or similar to map the new conference details onto the old so we don't lose any child properties e.g. Speakers/Presentations.
                //We'll do it manually for now
                conference.Speakers = existingConference.Speakers;
                conference.Presentations = existingConference.Presentations;

                YouConfDataContext.UpsertConference(id, conference);


                if (existingConference.HangoutId != conference.HangoutId)
                {
                    //User has changed the conference hangout id, so notify any listeners/viewers out there if they're watching (e.g. during the live conference streaming)
                    var context = GlobalHost.ConnectionManager.GetHubContext<YouConfHub>();
                    context.Clients.Group(conference.HashTag).updateConferenceVideoUrl(conference.HangoutId);
                }

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

        public ActionResult Lookup(string conferenceHashTag)
        {
            return Json(IsConferenceHashTagAvailable(conferenceHashTag), JsonRequestBehavior.AllowGet);
        }

        private bool IsConferenceHashTagAvailable(string hashTag)
        {
            var conference = YouConfDataContext.GetConference(hashTag);
            return conference == null;
        }
    }
}
