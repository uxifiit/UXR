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
using Microsoft.AspNet.Identity;
using UXR.Models.Entities;

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
                cfg.CreateMap<Session, SelectSessionViewModel>();
                cfg.CreateMap<Project, SelectProjectSessionViewModel>();
            });

            Mapper = config.CreateMapper();
        }


        private readonly StudiesDatabase _database;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CommandDispatcher _dispatcher;
        private readonly RecordingFilesManager _recordings;


        public RecordingController(StudiesDatabase database, UserManager<ApplicationUser> userManager, CommandDispatcher dispatcher, RecordingFilesManager recordings)
        {
            _database = database;
            _userManager = userManager;
            _dispatcher = dispatcher;
            _recordings = recordings;
        }


        [Route(Routes.Recording.ACTION_ASSIGN_SESSION)]
        public ActionResult AssignSession()
        {
            var currentUser = _userManager.FindById(User.Identity.GetUserId());

            List<RecordingViewModel> recordings = _recordings.GetUnassignedRecordings()
                                                             .OrderBy(r => r.StartTime)
                                                             .Select(Mapper.Map<RecordingViewModel>)
                                                             .ToList();

            var assign = new SessionAssigningViewModel(recordings.Select(r => new SelectableRecordingViewModel(r)));

            assign.ResetProjectSelection(GetProjectSessionSelection(currentUser.Id, User.IsInRole(UserRoles.ADMIN)));

            return View(assign);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route(Routes.Recording.ACTION_ASSIGN_SESSION)]
        public ActionResult AssignSession(SessionAssigningViewModel assign)
        {
            Request.ThrowIfDifferentReferrer();

            var currentUser = _userManager.FindById(User.Identity.GetUserId());

            if (ModelState.IsValid
                && assign.Recordings != null
                && assign.Recordings.Any(r => r.IsSelected))
            {
                var session = _database.Sessions
                                       .Where(s => s.Id == assign.SessionId
                                                   && s.ProjectId == assign.ProjectId)
                                       .AsDbQuery()
                                       .Include(s => s.Project.Owner)
                                       .SingleOrDefault();

                if (session != null 
                    && (session.Project.Owner.Id == currentUser.Id || User.IsInRole(UserRoles.ADMIN)))
                {
                    var selectedRecordings = assign.Recordings.Where(r => r.IsSelected);

                    foreach (var recording in selectedRecordings)
                    {
                        _recordings.AssignRecordingToSession(session, recording.NodeName, recording.StartTime);
                    }

                    return RedirectToAction(nameof(SessionController.Details), SessionController.ControllerName, new { sessionId = assign.SessionId });
                }

                ModelState.AddModelError(nameof(assign.SessionId), "Session was not selected.");
            }

            assign.ResetProjectSelection(GetProjectSessionSelection(currentUser.Id, User.IsInRole(UserRoles.ADMIN)));

            return View(assign);
        }


        private List<SelectProjectSessionViewModel> GetProjectSessionSelection(string userId, bool isAdmin)
        {
            return _database.Projects
                            .FilterByUserRights(userId, isAdmin)
                            .OrderBy(p => p.Name)
                            .AsDbQuery()
                            .Include(p => p.Sessions)
                            .Select(Mapper.Map<SelectProjectSessionViewModel>)
                            .ToList();
        }
    }
}
