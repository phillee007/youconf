using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YouConf.Common.Data.Entities;
using YouConf.Common.Data;

namespace YouConf.Controllers
{
    public class SpeakerController : BaseController
    {
        public IYouConfDbContext YouConfDbContext { get; set; }

        public SpeakerController(IYouConfDbContext youConfDbContext)
        {
            if (youConfDbContext == null)
            {
                throw new ArgumentNullException("youConfDbContext");
            }
            YouConfDbContext = youConfDbContext;
        }

        //
        // GET: /Speaker/

        public ActionResult Index(string conferenceHashTag)
        {
            var conference = YouConfDbContext.Conferences
                .FirstOrDefault(x => x.HashTag == conferenceHashTag);

            if (conference == null)
            {
                return HttpNotFound();
            }
            return View(conference.Speakers);
        }


        //
        // GET: /Speaker/Create

        public ActionResult Add(string id)
        {
            var conference = YouConfDbContext.Conferences
                .FirstOrDefault(x => x.HashTag == id);
            var speaker = new Speaker()
            {
                ConferenceId = conference.Id
            };

            ModelState.Remove("id");
            ViewBag.ConferenceHashTag = id;
            return View(speaker);
        }

        //
        // POST: /Speaker/Add

        [HttpPost]
        public ActionResult Add(string conferenceHashTag, Speaker speaker)
        {
            if (ModelState.IsValid)
            {
                YouConfDbContext.Speakers.Add(speaker);
                YouConfDbContext.SaveChanges();
                UpdateConferenceInSolrIndex(speaker.ConferenceId, Common.Messaging.SolrIndexAction.Update);

                return RedirectToAction("Details", "Conference", new { hashTag = conferenceHashTag });
            }

            ViewBag.ConferenceId = conferenceHashTag;
            return View(speaker);
        }

        //
        // GET: /Speaker/Edit/5

        public ActionResult Edit(int id)
        {
            var speaker = YouConfDbContext.Speakers
                .FirstOrDefault(x => x.Id == id);
            if (speaker == null)
            {
                return HttpNotFound();
            }

            return View(speaker);
        }

        //
        // POST: /Speaker/Edit/5

        [HttpPost]
        public ActionResult Edit(Speaker speaker)
        {
            if (ModelState.IsValid)
            {
                var currentSpeaker = YouConfDbContext.Speakers
                    .FirstOrDefault(x => x.Id == speaker.Id);
                if (currentSpeaker == null)
                {
                    return HttpNotFound();
                }

                Mapper.Map(speaker, currentSpeaker);
                YouConfDbContext.SaveChanges();
                UpdateConferenceInSolrIndex(currentSpeaker.ConferenceId, Common.Messaging.SolrIndexAction.Update);

                return RedirectToAction("Details", "Conference", new { hashTag = currentSpeaker.Conference.HashTag });
            }

            ViewBag.ConferenceId = speaker.ConferenceId;
            return View(speaker);
        }

        //
        // GET: /Speaker/Delete/5

        public ActionResult Delete(int id)
        {
            var currentSpeaker = YouConfDbContext.Speakers
                .FirstOrDefault(x => x.Id == id);
            if (currentSpeaker == null)
            {
                return HttpNotFound();
            }

            return View(currentSpeaker);
        }

        //
        // POST: /Speaker/Delete/5

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            if (ModelState.IsValid)
            {
                var currentSpeaker = YouConfDbContext.Speakers
                    .Include(x => x.Conference)
                    .FirstOrDefault(x => x.Id == id);
                if (currentSpeaker == null)
                {
                    return HttpNotFound();
                }

                var conferenceHashTag = currentSpeaker.Conference.HashTag;

                currentSpeaker.Presentations.Clear();
                YouConfDbContext.Speakers.Remove(currentSpeaker);
                YouConfDbContext.SaveChanges();
                UpdateConferenceInSolrIndex(currentSpeaker.ConferenceId, Common.Messaging.SolrIndexAction.Update);

                return RedirectToAction("Details", "Conference", new { hashTag = conferenceHashTag });
            }

            return View();
        }
    }
}
