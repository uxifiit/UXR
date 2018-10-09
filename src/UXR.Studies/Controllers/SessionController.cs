using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UXI.Common.Web.Extensions;
using UXI.CQRS;
using UXR.Studies;
using UXR.Studies.Models;
using UXR.Studies.ViewModels;
using UXR.Models;
using UXR.Studies.Files;
using System.Net.Http;
using System.IO;
using Microsoft.AspNet.Identity;
using UXR.Models.Entities;
using UXR.Studies.Extensions;
using UXR.Common;
using UXR.Studies.Models.Queries;
using UXR.Studies.ViewModels.Sessions;
using UXR.Studies.ViewModels.Recordings;
using UXR.Studies.Models.Commands;
using UXR.Studies.ViewModels.Projects;
using Ionic.Zip;
using System.Net.Http.Headers;
using System.IO.Pipes;
using System.Threading.Tasks;
using UXR.Studies.ViewModels.Users;
using UXR.Studies.Controllers.Validation;

namespace UXR.Studies.Controllers
{
    [RouteArea(Routes.AREA_NAME, AreaPrefix = Routes.AREA_NAME)]
    [RoutePrefix(Routes.Session.PREFIX)]
    [Authorize(Roles = UserRoles.APPROVED)]
    public class SessionController : Controller
    {
        public static readonly string ControllerName = nameof(SessionController).Replace("Controller", "");

        private readonly StudiesDatabase _database;
        private readonly CommandDispatcher _dispatcher;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RecordingFilesManager _recordings;
        private readonly ZipHelper _zip;
        private readonly ITimeProvider _timeProvider;


        private static readonly IMapper Mapper;

        static SessionController()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApplicationUser, UserNameViewModel>();

                cfg.CreateMap<Recording, RecordingViewModel>();
                cfg.CreateMap<Project, SelectProjectViewModel>();

                cfg.CreateMap<Session, SessionViewModel>();
                cfg.CreateMap<Session, SessionDetailsViewModel>();
                cfg.CreateMap<Session, EditSessionViewModel>()
                   .ForMember(e => e.OriginalName, e => e.MapFrom(s => s.Name))
                   .ForMember(e => e.UseProjectDefinitionTemplate, e => e.ResolveUsing((s, v) => String.IsNullOrEmpty(s.Definition)));
                cfg.CreateMap<EditSessionViewModelPost, EditSessionViewModel>()
                   .ForMember(e => e.OriginalName, e => e.Ignore());

                cfg.CreateMap<CreateSessionViewModelPost, CreateSessionViewModel>();

                cfg.CreateMap<Session, CalendarSessionViewModel>()
                   .ForMember(s => s.EndTime, s => s.MapFrom(session => session.StartTime + session.Length));
            });

            Mapper = config.CreateMapper();
        }


        public SessionController
        (
            StudiesDatabase database, 
            CommandDispatcher dispatcher, 
            UserManager<ApplicationUser> userManager, 
            RecordingFilesManager recordings,
            ZipHelper zip,
            ITimeProvider timeProvider
        )
        {
            _database = database;
            _dispatcher = dispatcher;
            _userManager = userManager;
            _recordings = recordings;
            _zip = zip;
            _timeProvider = timeProvider;
        }


        // GET: Studies/Session/Create
        [Route(Routes.Session.ACTION_CREATE)]
        public ActionResult Create(int? projectId)
        {
            if (projectId.HasValue)
            {
                var project = _database.Projects
                                       .Where(p => p.Id == projectId)
                                       .SingleOrDefault();

                var currentUser = _userManager.FindById(User.Identity.GetUserId());

                if (project != null)
                {
                    if (project.Owner == currentUser || User.IsInRole(UserRoles.ADMIN))
                    {
                        var session = new CreateSessionViewModel()
                        {
                            Name = "",
                            StartTime = _timeProvider.CurrentTime.Ceiling(TimeSpan.FromHours(1)),
                            Length = TimeSpan.FromHours(1),
                            UseProjectDefinitionTemplate = true,
                            Definition = project.SessionDefinitionTemplate ?? "",
                            ProjectId = project.Id
                        };

                        return View(session);
                    }
                }

                return HttpNotFound();
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        private const string forbiddenCharactersMessage = "Session name cannot contain \" < > | : * ? \\ /";
        private const string sessionNameInUseMessage = "Session with this name already exists in this project (case insensitive)";
        private const string projectIsInaccessibleMessage = "Project inaccessible";


        // POST: Studies/Session/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route(Routes.Session.ACTION_CREATE)]
        public ActionResult Create(int? projectId, CreateSessionViewModelPost create)
        {
            Request.ThrowIfDifferentReferrer();

            if (projectId.HasValue)
            {
                var project = _database.Projects
                                       .FilterById(projectId.Value)
                                       .SingleOrDefault();

                var currentUser = _userManager.FindById(User.Identity.GetUserId());

                if (CheckProjectAccessRights(nameof(projectId), project, currentUser))
                {
                    if (ModelState.IsValid
                        && PathNameValidation.CheckContainsNoForbiddenPathCharacters(ModelState, nameof(create.Name), create.Name)
                        && CheckSessionNameIsAlreadyInUse(nameof(create.Name), project, create.Name)
                        && (create.UseProjectDefinitionTemplate 
                            || SessionDefinitionValidation.CheckDefinitionTemplateJson(ModelState, nameof(create.Definition), create.Definition)))
                    {
                        var command = new CreateSessionCommand()
                        {
                            Name = create.Name,
                            StartTime = create.StartTime,
                            Length = create.Length,
                            Definition = create.UseProjectDefinitionTemplate ? null : create.Definition,
                            Project = project,
                        };

                        _dispatcher.Dispatch(command);

                        return RedirectToAction(nameof(ProjectController.Details), ProjectController.ControllerName, new { projectId = projectId });
                    }

                    var newCreate = Mapper.Map<CreateSessionViewModel>(create);

                    newCreate.ProjectId = project.Id;

                    return View(newCreate);
                }

                return HttpNotFound();
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }


        private bool CheckSessionNameIsAlreadyInUse(string propertyName, Project project, string sessionName, int? excludeId = null)
        {
            var query = project.Sessions
                               .AsQueryable()
                               .FilterByName(sessionName);
            if (excludeId.HasValue)
            {
                query = query.Where(s => s.Id != excludeId.Value);
            }

            bool nameAlreadyUsed = query.Any();
            if (nameAlreadyUsed)
            {
                ModelState.AddModelError(propertyName, sessionNameInUseMessage);
                return false;
            }

            return true;
        }

        private bool CheckProjectAccessRights(string propertyName, Project project, ApplicationUser user)
        {
            if (project != null && user != null
                && (project.Owner == user || User.IsInRole(UserRoles.ADMIN)))
            {
                return true;
            }

            ModelState.AddModelError(propertyName, projectIsInaccessibleMessage);
            return false;
        }


        [Route(Routes.Session.ACTION_DETAILS)]
        public ActionResult Details(int? sessionId)
        {
            if (sessionId.HasValue)
            {
                var session = _database.Sessions
                                       .FilterById(sessionId.Value)
                                       .AsDbQuery()
                                       .Include(s => s.Project)
                                       .SingleOrDefault();

                var currentUser = _userManager.FindById(User.Identity.GetUserId());

                if (session != null
                    && (session.Project.Owner == currentUser || User.IsInRole(UserRoles.ADMIN)))
                {
                    var details = Mapper.Map<SessionDetailsViewModel>(session);

                    details.Recordings = _recordings.GetSessionRecordings(session)
                                                    .Select(Mapper.Map<RecordingViewModel>)
                                                    .ToList();

                    return View(details);
                }

                return HttpNotFound();
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }


        // GET: Studies/Session/Edit/5
        [Route(Routes.Session.ACTION_EDIT)]
        public ActionResult Edit(int? sessionId)
        {
            if (sessionId.HasValue)
            {
                var session = _database.Sessions
                                       .FilterById(sessionId.Value)
                                       .AsDbQuery()
                                       .Include(s => s.Project)
                                       .SingleOrDefault();

                var currentUser = _userManager.FindById(User.Identity.GetUserId());

                if (session != null
                    && (session.Project.Owner == currentUser || User.IsInRole(UserRoles.ADMIN)))
                {
                    var edit = Mapper.Map<EditSessionViewModel>(session);

                    var projects = GetSelectProjectList(currentUser.Id);
                    
                    if (edit.UseProjectDefinitionTemplate)
                    {
                        edit.Definition = session.Project.SessionDefinitionTemplate;
                    }

                    edit.PopulateProjectSelectList(projects);

                    return View(edit);
                }

                return HttpNotFound();
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }


        private List<SelectProjectViewModel> GetSelectProjectList(string userId)
        {
            IQueryable<Project> projectsQuery;

            if (User.IsInRole(UserRoles.ADMIN))
            {
                projectsQuery = _database.Projects;
            }
            else
            {
                projectsQuery = _database.Projects
                                         .Where(p => p.Owner.Id == userId);
            }

            return projectsQuery.OrderBy(p => p.CreatedAt)
                                .Select(Mapper.Map<SelectProjectViewModel>)
                                .ToList();
        }


        // TODO ! REVIEW

        // POST: Studies/Session/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route(Routes.Session.ACTION_EDIT)]
        public ActionResult Edit(int sessionId, EditSessionViewModelPost edit)
        {
            Request.ThrowIfDifferentReferrer();

            var session = _database.Sessions
                                   .FilterById(sessionId)
                                   .AsDbQuery()
                                   .Include(s => s.Project.Owner)
                                   .SingleOrDefault();

            var currentUser = _userManager.FindById(User.Identity.GetUserId());

            if (session?.Project != null && currentUser != null
                && (session.Project.Owner == currentUser || User.IsInRole(UserRoles.ADMIN)))
            {
                Project project;
                if (edit.ProjectId != session.ProjectId)
                {
                    project = _database.Projects
                                       .FilterById(edit.ProjectId)
                                       .AsDbQuery()
                                       .Include(p => p.Sessions)
                                       .Include(p => p.Owner)
                                       .SingleOrDefault();
                }
                else
                {
                    project = session.Project;
                }

                if (ModelState.IsValid
                    && CheckProjectAccessRights(nameof(edit.ProjectId), project, currentUser)
                    && PathNameValidation.CheckContainsNoForbiddenPathCharacters(ModelState, nameof(edit.Name), edit.Name)
                    && CheckSessionNameIsAlreadyInUse(nameof(edit.Name), project, edit.Name, session.Id)
                    && (edit.UseProjectDefinitionTemplate
                        || SessionDefinitionValidation.CheckDefinitionTemplateJson(ModelState, nameof(edit.Definition), edit.Definition)))
                {
                    var nameBeforeEdit = session.Name;
                    var projectBeforeEdit = session.Project;

                    var command = new EditSessionCommand()
                    {
                        Session = session,
                        Project = project,
                        Name = edit.Name,
                        StartTime = edit.StartTime,
                        Length = edit.Length,
                        Definition = edit.UseProjectDefinitionTemplate ? null : edit.Definition
                    };

                    _dispatcher.Dispatch(command);

                    if (session.Project != projectBeforeEdit || session.Name != nameBeforeEdit)
                    {
                        _recordings.UpdateSessionDataLocation(session, projectBeforeEdit.Owner.Email, projectBeforeEdit.Name, nameBeforeEdit);
                    }

                    return RedirectToAction(nameof(ProjectController.Details), ProjectController.ControllerName, new { projectId = session.ProjectId });
                }
                else
                {
                    // return again
                    // Prevent changing of the headline in the view using the name from viewmodel

                    var newEdit = Mapper.Map<EditSessionViewModel>(edit);

                    newEdit.OriginalName = session.Name;

                    var projects = GetSelectProjectList(currentUser.Id);
                    newEdit.PopulateProjectSelectList(projects);

                    //List<RecordingViewModel> recordings = _recordings.GetSessionRecordings(session)
                    //                                                 .Select(Mapper.Map<RecordingViewModel>)
                    //                                                 .ToList();

                    //edit.SelectableRecordings = new SelectableRecordingsViewModel(recordings);

                    return View(newEdit);
                }
            }

            return HttpNotFound();  
        }


        // GET: Studies/Session/Delete/5
        [Route(Routes.Session.ACTION_DELETE)]
        public ActionResult Delete(int? sessionId)
        {
            if (sessionId.HasValue)
            {
                var session = _database.Sessions
                                       .FilterById(sessionId.Value)
                                       .AsDbQuery()
                                       .Include(s => s.Project)
                                       .SingleOrDefault();

                var currentUser = _userManager.FindById(User.Identity.GetUserId());

                if (session != null 
                    && (session.Project.Owner == currentUser || User.IsInRole(UserRoles.ADMIN)))
                {
                    return View(Mapper.Map<SessionViewModel>(session));
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return HttpNotFound();
        }


        // POST: Studies/Session/Delete/5
        [HttpPost, ActionName(Routes.Session.ACTION_DELETE)]
        [ValidateAntiForgeryToken]
        [Route(Routes.Session.ACTION_DELETE)]
        public ActionResult DeleteConfirmed(int sessionId)
        {
            Request.ThrowIfDifferentReferrer();

            var session = _database.Sessions
                                   .FilterById(sessionId)
                                   .AsDbQuery()
                                   .Include(s => s.Project)
                                   .SingleOrDefault();

            var currentUser = _userManager.FindById(User.Identity.GetUserId());
            
            int projectId;

            if (session != null)
            {
                if (session.Project.Owner == currentUser || User.IsInRole(UserRoles.ADMIN))
                {
                    projectId = session.ProjectId;

                    var command = new DeleteSessionCommand()
                    {
                        Session = session,
                    };

                    _recordings.DeleteSessionData(session);

                    _dispatcher.Dispatch(command);

                    return RedirectToAction(nameof(ProjectController.Details), ProjectController.ControllerName, new { projectId = projectId });
                }
            }
            return HttpNotFound();
        }


        [Route(Routes.Session.ACTION_CALENDAR)]
        public ActionResult Calendar()
        {
            var today = _timeProvider.CurrentTime.Date;
            var tomorrow = today.AddDays(1);

            var sessions = _database.Sessions
                                    .Where(s => s.StartTime >= today && s.StartTime < tomorrow)
                                    .OrderBy(s => s.StartTime)
                                    .AsDbQuery()
                                    .Include(s => s.Project.Owner)
                                    .ToList();

            var viewModel = new CalendarViewModel()
            {
                Sessions = sessions.Select(Mapper.Map<CalendarSessionViewModel>).ToList()
            };

            return PartialView(viewModel);
        }


        [HttpGet]
        [Route(Routes.Session.ACTION_DOWNLOAD)]
        public ActionResult Download(int? sessionId)
        {
            if (sessionId.HasValue)
            {
                var session = _database.Sessions
                                       .FilterById(sessionId.Value)
                                       .AsDbQuery()
                                       .Include(s => s.Project)
                                       .SingleOrDefault();

                var currentUser = _userManager.FindById(User.Identity.GetUserId());

                if (session != null
                    && (session.Project.Owner == currentUser || User.IsInRole(UserRoles.ADMIN)))
                {
                    var sessionRecordings = _recordings.GetSessionRecordings(session);

                    if (sessionRecordings.Any())
                    {
                        string filename = $"{session.Project.Name} - {session.Name}.zip";

                        return this.StreamFileResult(filename, "application/octet-stream", stream =>
                        {
                            _zip.ZipRecordingFiles(sessionRecordings, stream, ZipHelper.RecordingFilesPathDepth.Node);
                        });
                        
                    }

                    return HttpNotFound();
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest); // redirect to details, add details, not just edit
        }
    }
}
