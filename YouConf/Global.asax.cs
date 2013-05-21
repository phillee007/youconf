using AutoMapper;
using Microsoft.AspNet.SignalR;
using Microsoft.WindowsAzure;
using Ninject;
using Ninject.Web.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using YouConf.Data;
using YouConf.Data.Entities;

namespace YouConf
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            //SignalR
            var serviceBusConnectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            GlobalHost.DependencyResolver.UseServiceBus(serviceBusConnectionString, "YouConf");
            RouteTable.Routes.MapHubs();

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            ConfigureAutoMapper();

            //Tell Entity Framework to automatically update our database to the latest version on app startup
            Database.SetInitializer(new System.Data.Entity.MigrateDatabaseToLatestVersion<YouConfDbContext, YouConf.Migrations.Configuration>());
        }

        private static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<Speaker, Speaker>()
                .ForMember(x => x.Presentations, x => x.Ignore())
                .ForMember(x => x.Conference, x => x.Ignore());
            Mapper.CreateMap<Presentation, Presentation>()
                .ForMember(x => x.Speakers, x => x.Ignore())
                .ForMember(x => x.Conference, x => x.Ignore());
            Mapper.CreateMap<Conference, Conference>()
                .ForMember(x => x.Presentations, x => x.Ignore())
                .ForMember(x => x.Speakers, x => x.Ignore())
                .ForMember(x => x.Administrators, x => x.Ignore());
        }
    }
}