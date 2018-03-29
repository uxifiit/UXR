using System.Web.Mvc;
using UXR.Studies;

namespace UXR.Areas.Studies
{
    public class StudiesAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return Routes.AREA_NAME;
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                name: $"{AreaName}_default",
                url: $"{AreaName}/{{controller}}/{{action}}/{{id}}",
                defaults: new { action = Routes.ACTION_INDEX, id = UrlParameter.Optional }
            );
        }
    }
}
