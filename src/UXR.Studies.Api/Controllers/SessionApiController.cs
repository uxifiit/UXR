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
    [RoutePrefix(ApiRoutes.Session.PREFIX)]
    public class SessionApiController : ApiController
    {

        private readonly StudiesDatabase _database;
        private readonly Common.ITimeProvider _timeProvider;

        private static readonly IMapper Mapper;

        static SessionApiController()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SessionApiProfile>();
            });

            Mapper = config.CreateMapper();
        }


        public SessionApiController(StudiesDatabase database, ITimeProvider timeProvider)
        {
            _database = database;
            _timeProvider = timeProvider;
        }

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


        [HttpGet]
        [Route(ApiRoutes.Session.ACTION_SESSIONS_NOW)]
        [ResponseType(typeof(List<SessionInfo>))]
        public IHttpActionResult ActiveSessions()
        {
            var timeNow = _timeProvider.CurrentTime;

            var activeSessions = _database.Sessions
                                          .Where(s => (timeNow.CompareTo(s.StartTime) >= 0)
                                                      && (timeNow.CompareTo((DateTime)DbFunctions.AddMilliseconds(s.StartTime, DbFunctions.DiffMilliseconds(TimeSpan.Zero, s.Length))) <= 0))
                                          .AsDbQuery()
                                          .Include(s => s.Project)
                                          .ToList();

            var sessionInfoList = activeSessions.Select(Mapper.Map<SessionInfo>)
                                                .ToList();

            return Ok(sessionInfoList);
        }
    }
}
