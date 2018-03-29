using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using UXR.Common;
using UXR.Studies.Api.Entities;
using UXR.Studies.Api.Entities.Sessions;
using UXR.Studies.Api.MapperProfiles;
using UXR.Studies.Models;
using UXR.Studies.Models.Queries;

namespace UXR.Studies.Api.Controllers
{
    [RoutePrefix(ApiRoutes.Status.PREFIX)]
    public class StudiesStatusApiController : ApiController
    {
        [HttpGet]
        [Route(ApiRoutes.Status.ACTION_INDEX)]
        [ResponseType(typeof(StatusInfo))]
        public IHttpActionResult Index()
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            var status = new StatusInfo()
            {
                ApiVersion = version.ToString()
            };

            return Ok(status);
        }
    }
}
