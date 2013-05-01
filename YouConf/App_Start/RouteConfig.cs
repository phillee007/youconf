using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace YouConf
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ConferenceFriendlyUrl",
                url: "{hashTag}/{action}",
                defaults: new { controller = "Conference", action = "Details" },
                constraints: new { hashTag = new IsNotAControllerNameConstraint() }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }


    /// <summary>
    /// Thanks to http://stackoverflow.com/questions/10407365/mvc-routing-constraint-on-controller-names for the idea behind this.
    /// Modified the code to cache the list of controller names and enhance lookup speed using a HashSet rather than List
    /// </summary>
    public class IsNotAControllerNameConstraint : IRouteConstraint
    {
        private static HashSet<string> ControllerNames = GetControllerNames();

        private static List<Type> GetSubClasses<T>()
        {
            return Assembly.GetCallingAssembly().GetTypes().Where(
                type => type.IsSubclassOf(typeof(T))).ToList();
        }

        public static HashSet<string> GetControllerNames()
        {
            var controllerNames = new HashSet<string>();
            GetSubClasses<Controller>().ForEach(
                type => controllerNames.Add(type.Name));
            return controllerNames;
        }
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (values.ContainsKey(parameterName))
            {
                string stringValue = values[parameterName] as string;
                return !ControllerNames.Contains(stringValue + "Controller");
            }

            return true;
        }
    }
}