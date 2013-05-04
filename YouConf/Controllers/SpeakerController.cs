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
            var speaker = new Speaker()
            {
                Id = DateTime.Now.Ticks
            };
            //We want the id hidden field to be the SpeakerId, but if we don't remove it from ModelState here it will be the conference Id. See http://stackoverflow.com/questions/4710447/asp-net-mvc-html-hiddenfor-with-wrong-value
            ModelState.Remove("id");
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
                YouConfDataContext.UpsertConference(conference.HashTag, conference);
                return RedirectToAction("Details", "Conference", new { hashTag = conferenceHashTag });
            }

            ViewBag.ConferenceId = conferenceHashTag;
            return View(speaker);
        }

        //
        // GET: /Speaker/Edit/5

        public ActionResult Edit(long id, string conferenceHashTag)
        {
            var conference = YouConfDataContext.GetConference(conferenceHashTag);
            if (conference == null)
            {
                return HttpNotFound();
            }

            var speaker = conference.Speakers.FirstOrDefault(x => x.Id == id);
            if (speaker == null)
            {
                return HttpNotFound();
            }

            ViewBag.ConferenceId = conferenceHashTag;
            return View(speaker);
        }

        //
        // POST: /Speaker/Edit/5

        [HttpPost]
        public ActionResult Edit(long id, string conferenceHashTag, Speaker speaker)
        {
            if (ModelState.IsValid)
            {
                var conference = YouConfDataContext.GetConference(conferenceHashTag);
                if (conference == null)
                {
                    return HttpNotFound();
                }

                var currentSpeaker = conference.Speakers.FirstOrDefault(x => x.Id == id);
                if (currentSpeaker == null)
                {
                    return HttpNotFound();
                }

                //Overwrite the old speaker details with the new
                conference.Speakers[conference.Speakers.IndexOf(currentSpeaker)] = speaker;

                YouConfDataContext.UpsertConference(conferenceHashTag, conference);
                return RedirectToAction("Details", "Conference", new { hashTag = conferenceHashTag });
            }

            ViewBag.ConferenceId = conferenceHashTag;
            return View(speaker);
        }

        //
        // GET: /Speaker/Delete/5

        public ActionResult Delete(long id, string conferenceHashTag)
        {
            var conference = YouConfDataContext.GetConference(conferenceHashTag);
            if (conference == null)
            {
                return HttpNotFound();
            }

            var currentSpeaker = conference.Speakers.FirstOrDefault(x => x.Id == id);
            if (currentSpeaker == null)
            {
                return HttpNotFound();
            }

            ViewBag.ConferenceId = conferenceHashTag;

            return View(currentSpeaker);
        }

        //
        // POST: /Speaker/Delete/5

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

                var currentSpeaker = conference.Speakers.FirstOrDefault(x => x.Id == id);
                if (currentSpeaker == null)
                {
                    return HttpNotFound();
                }

                //Remove the speaker
                conference.Speakers.Remove(currentSpeaker);
                //Also remove them from any presentations...
                foreach (var presentation in conference.Presentations)
                {
                    var speaker = presentation.Speakers.FirstOrDefault(x => x.Id == currentSpeaker.Id);
                    presentation.Speakers.Remove(speaker);
                }

                YouConfDataContext.UpsertConference(conferenceHashTag, conference);

                return RedirectToAction("Details", "Conference", new { hashTag = conferenceHashTag });
            }

            ViewBag.ConferenceId = conferenceHashTag;
            return View();
        }
    }
}
