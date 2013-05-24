//using FluentAssertions;
//using FluentAssertions.Mvc;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Data.Common;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web.Mvc;
//using YouConf.Controllers;
//using YouConf.Data;
//using YouConf.Data.Entities;

//namespace YouConf.Tests.Controllers
//{
//    [TestClass]
//    public class ConferenceControllerTests
//    {
//        private YouConfDbContext _context;

//        /// <summary>
//        /// Thanks to for the idea to use Effort
//        /// http://www.codeproject.com/Articles/460175/Two-strategies-for-testing-Entity-Framework-Effort
//        /// </summary>
//        [TestInitialize]
//        public void SetupTest()
//        {
//            // create a new DbConnection using Effort
//            DbConnection connection = Effort.DbConnectionFactory.CreateTransient();

//            // use the same DbConnection object to create the context object the test will use.
//            _context = new YouConfDbContext(connection);
//            //_context.Configuration.ValidateOnSaveEnabled = false;
//        }

//        [TestCleanup]
//        public void Cleanup()
//        {
//            _context.Dispose();
//        }

//        [TestMethod]
//        public void All_Should_ReturnOnlyPublicConferences()
//        {
//            //Setup a stub repository to return three public conferences and one private
//            _context.Conferences.Add(new Conference(){ AvailableToPublic = true});
//            _context.Conferences.Add(new Conference(){ AvailableToPublic = true});
//            _context.Conferences.Add(new Conference(){ AvailableToPublic = true});
//            _context.Conferences.Add(new Conference(){ AvailableToPublic = false});
//            _context.SaveChanges();

//            var conferenceController = new ConferenceController(_context);

//            var result = conferenceController.All()
//                .As<ViewResult>();

//            result.Model
//                .As<IEnumerable<Conference>>()
//                .Should().HaveCount(3);

//        }

//        [TestMethod]
//        public void Details_WithInvalidHashTag_Should_ReturnHttpNotFoundResult()
//        {
//            var conferenceController = new ConferenceController(_context);

//            var result = conferenceController.Details("thisisinvalid")
//                .As<HttpNotFoundResult>();
//        }


//        [TestMethod]
//        public void Details_WithValidHashTag_Should_ReturnCorrectConference()
//        {
//            var stubRepository = new Mock<IYouConfDataContext>();
//            var stubConference = new Conference() { HashTag = "abcde" };
//            stubRepository
//                .Setup(x => x.GetConference("abcde"))
//                .Returns(stubConference);

//            var conferenceController = new ConferenceController(_context);

//            var result = conferenceController.Details("abcde")
//                .As<ViewResult>();

//            result.Model
//                .As<Conference>()
//                .Should()
//                .Be(stubConference);
//        }

//        [TestMethod]
//        public void Create_WithAlreadyUsedHashTag_Should_ReturnViewAndAddModelError()
//        {
//            var stubRepository = new Mock<IYouConfDataContext>();
//            var stubConference = new Conference() { HashTag = "abcde" };
//            stubRepository
//                .Setup(x => x.GetConference("abcde"))
//                .Returns(stubConference);

//            var conferenceController = new ConferenceController(_context);

//            var newConference = new Conference() { HashTag = "abcde" };
//            var result = conferenceController.Create(newConference)
//                .As<ViewResult>();

//            result.ViewData.ModelState["HashTag"]
//                .Errors
//                .Count
//                .Should()
//                .Be(1);
//        }
//    }
//}
