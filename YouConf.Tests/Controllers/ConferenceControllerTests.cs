using FluentAssertions;
using FluentAssertions.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using YouConf.Controllers;
using YouConf.Data;
using YouConf.Data.Entities;

namespace YouConf.Tests.Controllers
{
    [TestClass]
    public class ConferenceControllerTests
    {
        [TestMethod]
        public void All_Should_ReturnOnlyPublicConferences()
        {
            //Setup a stub repository to return three public conferences and one private
            var stubRepository = new Mock<IYouConfDataContext>();
            stubRepository
                .Setup(x => x.GetAllConferences())
                .Returns(new List<Conference>(){
                    new Conference(){ AvailableToPublic = true},
                    new Conference(){ AvailableToPublic = true},
                    new Conference(){ AvailableToPublic = true},
                    new Conference(){ AvailableToPublic = false}
                });

            var conferenceController = new ConferenceController(stubRepository.Object);

            var result = conferenceController.All()
                .As<ViewResult>();

            result.Model
                .As<IEnumerable<Conference>>()
                .Should().HaveCount(3);           

        }

        [TestMethod]
        public void Details_WithInvalidHashTag_Should_ReturnHttpNotFoundResult()
        {
            var stubRepository = new Mock<IYouConfDataContext>();
            var conferenceController = new ConferenceController(stubRepository.Object);

            var result = conferenceController.Details("thisisinvalid")
                .As<HttpNotFoundResult>();
        }


        [TestMethod]
        public void Details_WithValidHashTag_Should_ReturnCorrectConference()
        {
            var stubRepository = new Mock<IYouConfDataContext>();
            var stubConference = new Conference(){ HashTag = "abcde"};
            stubRepository
                .Setup(x => x.GetConference("abcde"))
                .Returns(stubConference);

            var conferenceController = new ConferenceController(stubRepository.Object);

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
            var stubRepository = new Mock<IYouConfDataContext>();
            var stubConference = new Conference() { HashTag = "abcde" };
            stubRepository
                .Setup(x => x.GetConference("abcde"))
                .Returns(stubConference);

            var conferenceController = new ConferenceController(stubRepository.Object);
            
            var newConference = new Conference(){ HashTag = "abcde"};
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
