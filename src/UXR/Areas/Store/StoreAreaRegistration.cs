using System.Web.Mvc;
using PLUS.PackageStore;

namespace UXR.Areas.Store
{
    public class StoreAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return Routes.AreaName;
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                name: $"{AreaName}_default",
                url: $"{AreaName}/{{controller}}/{{action}}/{{id}}",
                defaults: new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
