using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YouConf.Data;
using YouConf.Data.Entities;

namespace YouConf.Controllers
{
    public class PresentationController : Controller
    {
        public IYouConfDataContext YouConfDataContext { get; set; }

        public PresentationController(IYouConfDataContext youConfDataContext)
        {
            if (youConfDataContext == null)
            {
                throw new ArgumentNullException("youConfDataContext");
            }
            YouConfDataContext = youConfDataContext;
        }

        //
        // GET: /Presentation/Create

        public ActionResult Add(string id)
        {
            var conference = YouConfDataContext.GetConference(id);
            if (conference == null)
            {
                return HttpNotFound();
            }

            ModelState.Remove("id");
            var presentation = new Presentation()
            {
                Id = DateTime.Now.Ticks
            };

            ViewBag.Conference = conference;
            ViewBag.Speakers = conference.Speakers;

            return View(presentation);
        }

        //
        // POST: /Presentation/Add

        [HttpPost]
        public ActionResult Add(string conferenceHashTag, long[] speakerIds, Presentation presentation)
        {
            var conference = YouConfDataContext.GetConference(conferenceHashTag);
            if (conference == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                //Convert the start time to UTC before storing
                presentation.StartTime = ConvertToUtc(presentation.StartTime, conference.TimeZoneId);

                PopulateSpeakers(conference, presentation, speakerIds);
                conference.Presentations.Add(presentation);
                YouConfDataContext.UpsertConference(conference.HashTag, conference);
                return RedirectToAction("Details", "Conference", new { hashTag = conferenceHashTag });
            }

            ViewBag.Conference = conference;
            ViewBag.Speakers = conference.Speakers;
            return View(presentation);
        }

        private void PopulateSpeakers(Conference conference, Presentation presentation, long[] speakerIds)
        {
            presentation.Speakers.Clear();

            //Could look at creating a custom model binder here, but doing it simple for now....
            if (speakerIds == null)
                return;

            foreach (var speakerId in speakerIds)
            {
                var speaker = conference.Speakers.First(x => x.Id == speakerId);
                presentation.Speakers.Add(speaker);
            }
        }

        //
        // GET: /Presentation/Edit/5

        public ActionResult Edit(long id, string conferenceHashTag)
        {
            var conference = YouConfDataContext.GetConference(conferenceHashTag);
            if (conference == null)
            {
                return HttpNotFound();
            }

            var presentation = conference.Presentations.FirstOrDefault(x => x.Id == id);
            if (presentation == null)
            {
                return HttpNotFound();
            }

            ViewBag.Conference = conference;
            ViewBag.Speakers = conference.Speakers;
            return View(presentation);
        }

        //
        // POST: /Presentation/Edit/5

        [HttpPost]
        public ActionResult Edit(string conferenceHashTag, long[] speakerIds, Presentation presentation)
        {
            var conference = YouConfDataContext.GetConference(conferenceHashTag);
            if (conference == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                var currentPresentation = conference.Presentations.FirstOrDefault(x => x.Id == presentation.Id);
                if (currentPresentation == null)
                {
                    return HttpNotFound();
                }

                //Convert the start time to UTC before storing
                presentation.StartTime = ConvertToUtc(presentation.StartTime, conference.TimeZoneId);

                PopulateSpeakers(conference, presentation, speakerIds);
                //Overwrite the old Presentation details with the new
                conference.Presentations[conference.Presentations.IndexOf(currentPresentation)] = presentation;

                YouConfDataContext.UpsertConference(conferenceHashTag, conference);
                return RedirectToAction("Details", "Conference", new { hashTag = conferenceHashTag });
            }

            ViewBag.Conference = conference;
            ViewBag.Speakers = conference.Speakers;
            return View(presentation);
        }

        //
        // GET: /Presentation/Delete/5

        public ActionResult Delete(long id, string conferenceHashTag)
        {
            var conference = YouConfDataContext.GetConference(conferenceHashTag);
            if (conference == null)
            {
                return HttpNotFound();
            }

            var currentPresentation = conference.Presentations.FirstOrDefault(x => x.Id == id);
            if (currentPresentation == null)
            {
                return HttpNotFound();
            }

            ViewBag.ConferenceId = conferenceHashTag;

            return View(currentPresentation);
        }

        //
        // POST: /Presentation/Delete/5

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirm(long id, string conferenceHashTag)
        {
            if (ModelState.IsValid)
            {
                var conference = YouConfDataContext.GetConference(conferenceHashTag);
                if (conference == null)
                {
                    return HttpNotFound();
                }

                var currentPresentation = conference.Presentations.FirstOrDefault(x => x.Id == id);
                if (currentPresentation == null)
                {
                    return HttpNotFound();
                }

                //Remove the Presentation
                conference.Presentations.Remove(currentPresentation);
                YouConfDataContext.UpsertConference(conferenceHashTag, conference);

                return RedirectToAction("Details", "Conference", new { hashTag = conferenceHashTag });
            }

            ViewBag.ConferenceId = conferenceHashTag;
            return View();
        }

        private DateTime ConvertToUtc(DateTime dateTime, string timeZoneId)
        {
            DateTime startTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
            var conferenceTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeToUtc(startTime, conferenceTimeZone);
        }
    }
}
