using FluentAssertions;
using FluentAssertions.Mvc;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using YouConf.Data;
using YouConf.Migrations;
using YouConf.Data.Entities;
using System.Collections.Generic;
using YouConf.Controllers;
using System.Web.Mvc;
using System.Data.Entity.Migrations;
using YouConf.Tests.Integration.Extensions;
using Moq;
using System.Web;
using System.Security.Principal;
using System.Web.Routing;

namespace YouConf.Tests.Integration
{
    [TestClass]
    public class ConferenceControllerTests : TestBase
    {
        [TestMethod]
        public void All_Should_ReturnOnlyPublicConferences()
        {
            //Three public conferences, one private
            _context.Conferences.Add(new Conference() { HashTag = "test", Name = "test", Abstract = "test", StartDate = DateTime.Now, EndDate = DateTime.Now, TimeZoneId = "test", AvailableToPublic = true });
            _context.Conferences.Add(new Conference() { HashTag = "test", Name = "test", Abstract = "test", StartDate = DateTime.Now, EndDate = DateTime.Now, TimeZoneId = "test", AvailableToPublic = true });
            _context.Conferences.Add(new Conference() { HashTag = "test", Name = "test", Abstract = "test", StartDate = DateTime.Now, EndDate = DateTime.Now, TimeZoneId = "test", AvailableToPublic = true });
            _context.Conferences.Add(new Conference() { HashTag = "test", Name = "test", Abstract = "test", StartDate = DateTime.Now, EndDate = DateTime.Now, TimeZoneId = "test", AvailableToPublic = false });
            _context.SaveChangesWithErrors();

            var conferenceController = new ConferenceController(_context);

            var result = conferenceController.All()
                .As<ViewResult>();

            result.Model
                .As<IEnumerable<Conference>>()
                .Should().HaveCount(3);
        }

        [TestMethod]
        public void Details_WithInvalidHashTag_Should_ReturnHttpNotFoundResult()
        {
            var conferenceController = new ConferenceController(_context);

            var result = conferenceController.Details("thisisinvalid")
                .As<HttpNotFoundResult>();
        }


        [TestMethod]
        public void Details_WithValidHashTag_Should_ReturnCorrectConference()
        {
            var stubConference = new Conference() { HashTag = "abcde", Name = "test", Abstract = "test", StartDate = DateTime.Now, EndDate = DateTime.Now, TimeZoneId = "test", AvailableToPublic = true };
            _context.Conferences.Add(stubConference);
            _context.Conferences.Add(new Conference() { HashTag = "test", Name = "test", Abstract = "test", StartDate = DateTime.Now, EndDate = DateTime.Now, TimeZoneId = "test", AvailableToPublic = false });
            _context.SaveChangesWithErrors(); ;

            var conferenceController = new ConferenceController(_context);
            conferenceController.ControllerContext = TestHelper.MockContext(conferenceController, "TestUser");

            var result = conferenceController.Details("abcde")
                .As<ViewResult>();

            result.Model
                .As<Conference>()
                .Should()
                .Be(stubConference);
        }

        [TestMethod]
        public void Create_WithAlreadyUsedHashTag_Should_ReturnViewAndAddModelError()
        {
            _context.Conferences.Add(new Conference() { HashTag = "abcde", Name = "test", Abstract = "test", StartDate = DateTime.Now, EndDate = DateTime.Now, TimeZoneId = "test", AvailableToPublic = false });
            _context.SaveChangesWithErrors();

            var conferenceController = new ConferenceController(_context);

            var newConference = new Conference() { HashTag = "abcde" };
            var result = conferenceController.Create(newConference)
                .As<ViewResult>();

            result.ViewData.ModelState["HashTag"]
                .Errors
                .Count
                .Should()
                .Be(1);
        }       
    }
}
