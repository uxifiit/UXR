using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UXR.Attributes;

namespace UXR
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LoginRequiredAsDefaultAttribute());

            filters.Add(new LogActionFilterAttribute());
        }
    }
}
