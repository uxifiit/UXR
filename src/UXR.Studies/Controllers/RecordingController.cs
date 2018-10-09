using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using UXI.Common.Web.Extensions;
using UXI.CQRS;
using UXR.Studies.Files;
using UXR.Studies.ViewModels;
using UXR.Studies.Models;
using AutoMapper;
using UXR.Models;
using UXR.Studies.Models.Queries;
using UXR.Studies.ViewModels.Recordings;

namespace UXR.Studies.Controllers
{
    [RouteArea(Routes.AREA_NAME, AreaPrefix = Routes.AREA_NAME)]
    [RoutePrefix(Routes.Recording.PREFIX)]
    [Authorize(Roles = UserRoles.APPROVED)]
    public class RecordingController : Controller
    {
        private static readonly IMapper Mapper;

        public static readonly string ControllerName = nameof(RecordingController).Replace("Controller", "");

        static RecordingController()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Recording, RecordingViewModel>();
            });

            Mapper = config.CreateMapper();
        }


        private readonly StudiesDatabase _database;
        private readonly CommandDispatcher _dispatcher;
        private readonly RecordingFilesManager _recordings;


        public RecordingController(StudiesDatabase database, CommandDispatcher dispatcher, RecordingFilesManager recordings)
        {
            _database = database;
            _dispatcher = dispatcher;
            _recordings = recordings;
        }


        [Route(Routes.Recording.ACTION_ASSIGN_SESSION)]
        public ActionResult AssignSession()
        {
            var projects = _database.Projects
                        .OrderBy(p => p.CreatedAt)
                        .ToList();

            List<RecordingViewModel> recordings = _recordings.GetUnassignedRecordings()
                                                             .Select(Mapper.Map<RecordingViewModel>)
                                                             .ToList();

            return View(new SessionAssigningViewModel(projects, recordings));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route(Routes.Recording.ACTION_ASSIGN_SESSION)]
        public ActionResult AssignSession(SessionAssigningViewModel assign)
        {
            Request.ThrowIfDifferentReferrer();

            if (ModelState.IsValid)
            {
                var session = _database.Sessions
                                       .Where(s => s.Id == assign.SessionId)
                                       .AsDbQuery()
                                       .Include(s => s.Project.Owner)
                                       .SingleOrDefault();

                if (session != null && session.ProjectId == assign.ProjectId)
                {
                    var selectedRecordings = assign.Recordings
                                                   .Zip(assign.RecordingSelections, 
                                                        (recording, isSelected) => isSelected ? recording : null)
                                                   .Where(r => r != null);

                    foreach (var recording in selectedRecordings)
                    {
                        _recordings.AssignRecordingToSession(session, recording.NodeName, recording.StartTime);
                    }

                    return RedirectToAction(nameof(SessionController.Details), SessionController.ControllerName, new { sessionId = assign.SessionId });
                }

                ModelState.AddModelError(nameof(assign.SessionId), "Session was not selected.");

                return View(assign);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
    }
}
