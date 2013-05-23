using AutoMapper;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using YouConf.Data;
using YouConf.Data.Entities;
using YouConf.SignalRHubs;

namespace YouConf.Controllers
{
    public class ConferenceController : Controller
    {
        public IYouConfDbContext YouConfDbContext { get; set; }

        public ConferenceController(IYouConfDbContext youConfDbContext)
        {
            if (youConfDbContext == null)
            {
                throw new ArgumentNullException("youConfDbContext");
            }
            YouConfDbContext = youConfDbContext;
        }
        //
        // GET: /Conference/All

        public ActionResult All()
        {
            var conferences = YouConfDbContext
                .Conferences
                .Where(x => x.AvailableToPublic)
                .OrderBy(x => x.StartDate)
                .ToList();
            ViewBag.Title = "All conferences";
            return View(conferences);
        }

        //
        // GET: /Conference/Manage
        [System.Web.Mvc.Authorize]
        public ActionResult Manage()
        {
            var conferences = YouConfDbContext.UserProfiles
                .Include(x => x.ConferencesAdministering)
                .FirstOrDefault(x => x.UserName == User.Identity.Name)
                .ConferencesAdministering;

            ViewBag.Title = "Manage your conferences";
            return View("All", conferences);
        }

        //
        // GET: /Conference/Details/5

        public ActionResult Details(string hashTag)
        {
            var conference = YouConfDbContext
                .Conferences
                .Include(x => x.Administrators)
                .FirstOrDefault(x => x.HashTag == hashTag);
            if (conference == null)
            {
                return HttpNotFound();
            }
            ViewBag.CurrentUserCanEdit = false;
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.CurrentUserCanEdit = IsCurrentUserAuthorizedToAdministerConference(conference);
            }
            return View(conference);
        }

        //
        // GET: /Conference/Create
        [System.Web.Mvc.Authorize]
        public ActionResult Create()
        {
            var model = new Conference()
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1)
            };
            return View(model);
        }

        //
        // POST: /Conference/Create

        [HttpPost]
        [System.Web.Mvc.Authorize]
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

                var currentUserProfile = YouConfDbContext.UserProfiles.FirstOrDefault(x => x.UserName == User.Identity.Name);
                conference.Administrators.Add(currentUserProfile);
                
                YouConfDbContext.Conferences.Add(conference);
                YouConfDbContext.SaveChanges();

                return RedirectToAction("Details", new { hashTag = conference.HashTag });
            }
            return View(conference);
        }

        //
        // GET: /Conference/Edit/5
        [System.Web.Mvc.Authorize]
        public ActionResult Edit(string hashTag)
        {
            var conference = YouConfDbContext.Conferences
                .FirstOrDefault(x => x.HashTag == hashTag);
            if (conference == null)
            {
                return HttpNotFound();
            }
            if (!IsCurrentUserAuthorizedToAdministerConference(conference))
            {
                return HttpUnauthorized();
            }
            return View(conference);
        }

        //
        // POST: /Conference/Edit/5

        [HttpPost]
        [System.Web.Mvc.Authorize]
        public ActionResult Edit(string currentHashTag, Conference conference)
        {
            //If the user has changed the conference hashtag we have to make sure that the new one hasn't already been taken
            if (currentHashTag != conference.HashTag && !IsConferenceHashTagAvailable(conference.HashTag))
            {
                ModelState.AddModelError("HashTag", "Unfortunately that hashtag is not available.");
            }

            if (ModelState.IsValid)
            {
                var existingConference = YouConfDbContext.Conferences
                .FirstOrDefault(x => x.Id == conference.Id);
                if (conference == null)
                {
                    return HttpNotFound();
                }
                if (!IsCurrentUserAuthorizedToAdministerConference(existingConference))
                {
                    return HttpUnauthorized();
                }

                var conferenceTimeZone = TimeZoneInfo.FindSystemTimeZoneById(conference.TimeZoneId);
                conference.StartDate = TimeZoneInfo.ConvertTimeToUtc(conference.StartDate, conferenceTimeZone);
                conference.EndDate = TimeZoneInfo.ConvertTimeToUtc(conference.EndDate, conferenceTimeZone);

                var hasHangoutIdUpdated = existingConference.HangoutId != conference.HangoutId;

                Mapper.Map(conference, existingConference);
                YouConfDbContext.SaveChanges();


                if (hasHangoutIdUpdated)
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
        [System.Web.Mvc.Authorize]
        public ActionResult Delete(string hashTag)
        {
            var conference = YouConfDbContext.Conferences
                .FirstOrDefault(x => x.HashTag == hashTag);
            if (conference == null)
            {
                return HttpNotFound();
            }

            return View(conference);
        }

        //
        // POST: /Conference/Delete/5

        [HttpPost]
        [System.Web.Mvc.Authorize]
        [ActionName("Delete")]
        public ActionResult DeleteConfirm(string hashTag)
        {
            var conference = YouConfDbContext.Conferences
                .FirstOrDefault(x => x.HashTag == hashTag);

            if (conference == null)
            {
                return HttpNotFound();
            }
            if (!IsCurrentUserAuthorizedToAdministerConference(conference))
            {
                return HttpUnauthorized();
            }

            YouConfDbContext.Conferences.Remove(conference);
            YouConfDbContext.SaveChanges();

            return RedirectToAction("All");
        }

        public ActionResult Live(string hashTag)
        {
            var conference = YouConfDbContext.Conferences
                .FirstOrDefault(x => x.HashTag == hashTag);

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
            var conference = YouConfDbContext.Conferences
                .FirstOrDefault(x => x.HashTag == hashTag);

            return conference == null;
        }

        private bool IsCurrentUserAuthorizedToAdministerConference(Conference conference)
        {
            var userProfile = YouConfDbContext.UserProfiles.FirstOrDefault(x => x.UserName == User.Identity.Name);
            return conference.Administrators.Contains(userProfile);

        }

        protected HttpUnauthorizedResult HttpUnauthorized()
        {
            throw new HttpException((int)HttpStatusCode.Unauthorized, "Current user is does not have permission to access this page");
        }
    }
}
