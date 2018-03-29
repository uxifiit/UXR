using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using System.Data.Entity;
using UXR.Models;
using UXR.Studies;
using Elmah.Contrib.WebApi;
using UXR.Common;

namespace UXR
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
#if STAGING
            Database.SetInitializer(new ForceRecreateMainDbInitializer()
            {
                PartialInitializers = new List<IPartialDbInitializer<UXRDbContext>>
                {
                    new UsersDbInitializer(),
                    new StudiesDbInitializer(),
                //    new TestingDataDbInitializer(new TimeProvider())
                }
            });
#else
            Database.SetInitializer(new MainDbInitializer()
            {
                PartialInitializers = new List<IPartialDbInitializer<UXRDbContext>>
                {
                    new UsersDbInitializer(),
                    new StudiesDbInitializer()
                }
            });
#endif
            // Code that runs on application startup
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.Filters.Add(new ElmahHandleErrorApiAttribute());
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ModelBinders.Binders.Add(typeof(DateTime), new DateTimeBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new DateTimeBinder());

            AreaRegistration.RegisterAllAreas();

        }
    }
}
