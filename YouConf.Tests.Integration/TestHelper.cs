using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace YouConf.Tests.Integration
{
    public static class TestHelper
    {
        public static ControllerContext MockContext(Controller controller, string username)
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var principal = MockUser(username);

            context.SetupGet(c => c.Request).Returns(request.Object);
            context.SetupGet(c => c.Response).Returns(response.Object);
            context.SetupGet(c => c.Session).Returns(session.Object);
            context.SetupGet(c => c.Server).Returns(server.Object);
            context.SetupGet(c => c.User).Returns(principal.Object);

            return new ControllerContext(context.Object, new RouteData(), controller);
        }

        public static ControllerContext MockContext(Controller controller)
        {
            return MockContext(controller, null);
        }

        public static Mock<IPrincipal> MockUser(string userName)
        {
            var mock = new Mock<IPrincipal>();
            mock.SetupGet(i => i.Identity).Returns(GetIdentityMock(userName != null, userName).Object);
            mock.Setup(i => i.IsInRole(It.IsAny<string>())).Returns(false);
            return mock;
        }

        public static Mock<IIdentity> GetIdentityMock(bool isLoggedIn, string username)
        {
            var mock = new Mock<IIdentity>();

            mock.SetupGet(i => i.AuthenticationType).Returns(isLoggedIn ? "Mock Identity" : null);
            mock.SetupGet(i => i.IsAuthenticated).Returns(isLoggedIn);
            mock.SetupGet(i => i.Name).Returns(isLoggedIn ? username : null);

            return mock;
        }
    }
}
