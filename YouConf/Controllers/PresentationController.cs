using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YouConf.Common.Data;
using YouConf.Common.Data.Entities;

namespace YouConf.Controllers
{
    public class PresentationController : BaseController
    {
        public IYouConfDbContext YouConfDbContext { get; set; }

        public PresentationController(IYouConfDbContext youConfDbContext)
        {
            if (youConfDbContext == null)
            {
                throw new ArgumentNullException("youConfDbContext");
            }
            YouConfDbContext = youConfDbContext;
        }

        //
        // GET: /Presentation/Create

        public ActionResult Add(string id)
        {
            var conference = YouConfDbContext.Conferences
                .FirstOrDefault(x => x.HashTag == id);

            if (conference == null)
            {
                return HttpNotFound();
            }

            ModelState.Remove("id");
            var presentation = new Presentation()
            {
                StartTime = conference.StartDate
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
            var conference = YouConfDbContext.Conferences
                .Include(x => x.Speakers)
                .FirstOrDefault(x => x.HashTag == conferenceHashTag);

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
                YouConfDbContext.SaveChanges();

                UpdateConferenceInSolrIndex(conference.Id, Common.Messaging.SolrIndexAction.Update);

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

        public ActionResult Edit(int id)
        {
            var presentation = YouConfDbContext.Presentations
                .Include(x => x.Conference)
                .Include(x => x.Conference.Speakers)
                .FirstOrDefault(x => x.Id == id);
            if (presentation == null)
            {
                return HttpNotFound();
            }

            ViewBag.Conference = presentation.Conference;
            ViewBag.Speakers = presentation.Conference.Speakers;
            return View(presentation);
        }

        //
        // POST: /Presentation/Edit/5

        [HttpPost]
        public ActionResult Edit(long[] speakerIds, Presentation presentation)
        {
            var currentPresentation = YouConfDbContext.Presentations
                .Include(x => x.Conference)
                .Include(x => x.Conference.Speakers)
                .FirstOrDefault(x => x.Id == presentation.Id);
            if (currentPresentation == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                //Convert the start time to UTC before storing
                presentation.StartTime = ConvertToUtc(presentation.StartTime, currentPresentation.Conference.TimeZoneId);

                //Overwrite the old Presentation details with the new
                Mapper.Map(presentation, currentPresentation);
                PopulateSpeakers(currentPresentation.Conference, currentPresentation, speakerIds);

                YouConfDbContext.SaveChanges();
                UpdateConferenceInSolrIndex(currentPresentation.ConferenceId, Common.Messaging.SolrIndexAction.Update);

                return RedirectToAction("Details", "Conference", new { hashTag = currentPresentation.Conference.HashTag });
            }

            ViewBag.Conference = currentPresentation.Conference;
            ViewBag.Speakers = currentPresentation.Conference.Speakers;
            return View(presentation);
        }

        //
        // GET: /Presentation/Delete/5

        public ActionResult Delete(int id)
        {
            var currentPresentation = YouConfDbContext.Presentations
                .FirstOrDefault(x => x.Id == id);
            if (currentPresentation == null)
            {
                return HttpNotFound();
            }

            return View(currentPresentation);
        }

        //
        // POST: /Presentation/Delete/5

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            if (ModelState.IsValid)
            {
                var currentPresentation = YouConfDbContext.Presentations
                    .Include(x => x.Conference)
                    .FirstOrDefault(x => x.Id == id);
                if (currentPresentation == null)
                {
                    return HttpNotFound();
                }

                var conferenceHashTag = currentPresentation.Conference.HashTag;

                //Remove the Presentation
                currentPresentation.Speakers.Clear();
                YouConfDbContext.Presentations.Remove(currentPresentation);
                YouConfDbContext.SaveChanges();
                UpdateConferenceInSolrIndex(currentPresentation.ConferenceId, Common.Messaging.SolrIndexAction.Update);

                return RedirectToAction("Details", "Conference", new { hashTag = conferenceHashTag });
            }

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
