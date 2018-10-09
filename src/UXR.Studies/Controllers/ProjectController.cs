using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UXR.Studies.Models;
using UXR.Studies.ViewModels;
using UXR.Models;
using PagedList;
using UXI.Common.Web.Extensions;
using UXI.CQRS;
using UXR.Studies.Files;
using System.IO;
using UXR.Models.Entities;
using Microsoft.AspNet.Identity;
using UXR.Studies.Models.Queries;
using UXR.Studies.ViewModels.Projects;
using UXR.Studies.Models.Commands;
using UXR.Studies.ViewModels.Users;
using UXR.Studies.ViewModels.SessionTemplates;
using UXR.Studies.ViewModels.Sessions;
using UXR.Studies.Controllers.Validation;
using UXR.Studies.Extensions;

namespace UXR.Studies.Controllers
{
    [RouteArea(Routes.AREA_NAME, AreaPrefix = Routes.AREA_NAME)]
    [RoutePrefix(Routes.Project.PREFIX)]
    [Authorize(Roles = UserRoles.APPROVED)]
    public class ProjectController : Controller
    {
        private static readonly IMapper Mapper;

        public static readonly string ControllerName = nameof(ProjectController).Replace("Controller", "");

        static ProjectController()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApplicationUser, UserNameViewModel>();

                cfg.CreateMap<Project, ProjectViewModel>();

                cfg.CreateMap<Project, CreateProjectViewModel>();
                cfg.CreateMap<CreateProjectViewModelPost, CreateProjectViewModel>();

                cfg.CreateMap<Project, EditProjectViewModel>()
                   .ForMember(e => e.OriginalName, e => e.MapFrom(p => p.Name));
                cfg.CreateMap<EditProjectViewModelPost, EditProjectViewModel>()
                   .ForMember(e => e.OriginalName, e => e.Ignore());

                cfg.CreateMap<Project, ProjectDetailsViewModel>();

                cfg.CreateMap<SessionTemplate, SessionTemplateViewModel>();

                cfg.CreateMap<Session, SessionViewModel>();
            });

            Mapper = config.CreateMapper();
        }


        private readonly StudiesDatabase _database;
        private readonly CommandDispatcher _dispatcher;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RecordingFilesManager _recordings;
        private readonly ZipHelper _zip;

        public ProjectController
        (
            StudiesDatabase database, 
            CommandDispatcher dispatcher, 
            UserManager<ApplicationUser> userManager, 
            RecordingFilesManager recordings,
            ZipHelper zip
        )
        {
            _database = database;
            _dispatcher = dispatcher;
            _userManager = userManager;
            _recordings = recordings;
            _zip = zip;
        }

        private const int MIN_PAGE_SIZE = 20;

        // GET: Studies/Project
        [Route("")]
        [Route(Routes.ACTION_INDEX)]
        public ActionResult Index(int? limit, int? page)
        {
            var currentUser = _userManager.FindById(User.Identity.GetUserId());

            int pageSize = Math.Max(limit ?? MIN_PAGE_SIZE, MIN_PAGE_SIZE);
            int pageNumber = Math.Max(page ?? 1, 1);

            IQueryable<Project> projectsQuery;

            if (User.IsInRole(UserRoles.ADMIN))
            {
                projectsQuery = _database.Projects;
            }
            else
            {
                string userId = currentUser.Id;
                projectsQuery = _database.Projects
                                         .Where(p => p.Owner.Id == userId);
            }

            IEnumerable<Project> projects = Enumerable.Empty<Project>();

            int projectsCount = projectsQuery.Count();
            if (projectsCount > 0)
            {
                projects = projectsQuery.OrderBy(p => p.CreatedAt)
                                        .Page(pageNumber, pageSize)
                                        .ToList();
            }

            // Use StaticPageList to be able to map model to viewmodel, provide the totalItemCount to paging and not pull the entire projects table from the database
            return View(new StaticPagedList<ProjectViewModel>(projects.Select(Mapper.Map<ProjectViewModel>), pageNumber, pageSize, projectsCount));
        }


        // GET: Studies/Project/Create
        [Route(Routes.ACTION_CREATE)]
        public ActionResult Create()
        {
            var templates = GetSessionTemplatesList();

            return View(new CreateProjectViewModel() { SessionDefinitionTemplates = templates, SessionDefinitionTemplate = templates.FirstOrDefault()?.Definition });
        }


        private const string logInToCreateMessage = "You need to log in to create projects";
        private const string projectNameInUseMessage = "A user cannot own multiple projects with the same name (case insensitive)";

        // POST: Studies/Project/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route(Routes.ACTION_CREATE)]
        public ActionResult Create(CreateProjectViewModelPost create)
        {
            Request.ThrowIfDifferentReferrer();

            var currentUser = _userManager.FindById(User.Identity.GetUserId());

            if (ModelState.IsValid && currentUser != null
                && PathNameValidation.CheckContainsNoForbiddenPathCharacters(ModelState, nameof(create.Name), create.Name)
                && CheckProjectNameAlreadyInUse(nameof(create.Name), create.Name, currentUser)
                && SessionDefinitionValidation.CheckDefinitionTemplateJson(ModelState, nameof(create.SessionDefinitionTemplate), create.SessionDefinitionTemplate))
            { 
                var command = new CreateProjectCommand()
                {
                    Owner = currentUser,
                    SessionDefinitionTemplate = create.SessionDefinitionTemplate,
                    Name = create.Name,
                    Description = create.Description
                };

                _dispatcher.Dispatch(command);

                return RedirectToAction(nameof(Details), new { projectId = command.NewId });
            }

            var newCreate = Mapper.Map<CreateProjectViewModel>(create);
            newCreate.SessionDefinitionTemplates = GetSessionTemplatesList();

            return View(newCreate);
        }



        private bool CheckProjectNameAlreadyInUse(string property, string projectName, ApplicationUser user, int? excludeProjectId = null)
        {
            var query = _database.Projects
                                 .Where(p => p.Owner.Id == user.Id)
                                 .FilterByName(projectName);
            if (excludeProjectId.HasValue)
            {
                query = query.Where(p => p.Id != excludeProjectId.Value);
            }

            bool isNameAlreadyUsed = query.Any();

            if (isNameAlreadyUsed)
            {
                ModelState.AddModelError(property, projectNameInUseMessage);
                return false;
            }
            return true;
        }

       

        [Route(Routes.Project.ACTION_DETAILS)]
        public ActionResult Details(int? projectId)
        {
            if (projectId.HasValue)
            {
                Project project = _database.Projects
                                           .FilterById(projectId.Value)
                                           .AsDbQuery()
                                           .Include(p => p.Sessions)
                                           .Include(p => p.Owner)
                                           .SingleOrDefault();

                var currentUser = _userManager.FindById(User.Identity.GetUserId());

                if (project != null)
                {
                    if (project.Owner == currentUser || User.IsInRole(UserRoles.ADMIN))
                    {
                        var details = Mapper.Map<ProjectDetailsViewModel>(project);

                        foreach (var session in project.Sessions)
                        {
                            var sessionViewModel = Mapper.Map<SessionViewModel>(session);
                            sessionViewModel.RecordingsCount = _recordings.GetSessionRecordings(session).Count();

                            details.Sessions.Add(sessionViewModel);
                        }

                        return View(details);
                    }
                }

                return HttpNotFound();
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // GET: Studies/Project/Edit/5
        [Route(Routes.Project.ACTION_EDIT)]
        public ActionResult Edit(int? projectId)
        {
            if (projectId.HasValue)
            {
                Project project = _database.Projects
                                           .FilterById(projectId.Value)
                                           .AsDbQuery()
                                           .Include(p => p.Owner)
                                           .Include(p => p.Sessions)
                                           .SingleOrDefault();

                var currentUser = _userManager.FindById(User.Identity.GetUserId());

                if (project != null)
                {
                    if (project.Owner == currentUser || User.IsInRole(UserRoles.ADMIN))
                    {
                        var edit = Mapper.Map<EditProjectViewModel>(project);
                        edit.SessionDefinitionTemplates = GetSessionTemplatesList();

                        return View(edit);
                    }
                }

                return HttpNotFound();
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }


        // POST: Studies/Project/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route(Routes.Project.ACTION_EDIT)]
        public ActionResult Edit(int projectId, EditProjectViewModelPost edit)
        {
            Request.ThrowIfDifferentReferrer();

            var project = _database.Projects
                                   .FilterById(projectId)
                                   .AsDbQuery()
                                   .Include(p => p.Owner)
                                   .SingleOrDefault();

            var currentUser = _userManager.FindById(User.Identity.GetUserId());

            if (project != null && currentUser != null
                && (project.Owner == currentUser || User.IsInRole(UserRoles.ADMIN)))
            {
                if (ModelState.IsValid
                    && PathNameValidation.CheckContainsNoForbiddenPathCharacters(ModelState, nameof(edit.Name), edit.Name)
                    && CheckProjectNameAlreadyInUse(nameof(edit.Name), edit.Name, project.Owner, project.Id)
                    && SessionDefinitionValidation.CheckDefinitionTemplateJson(ModelState, nameof(edit.SessionDefinitionTemplate), edit.SessionDefinitionTemplate))
                {
                    string nameBeforeEdit = project.Name;

                    var command = new EditProjectCommand()
                    {
                        Project = project,
                        Name = edit.Name,
                        Description = edit.Description,
                        SessionDefinitionTemplate = edit.SessionDefinitionTemplate
                    };

                    _dispatcher.Dispatch(command);

                    if (project.Name != nameBeforeEdit)
                    {
                        _recordings.UpdateProjectDataLocation(project, nameBeforeEdit);
                    }

                    return RedirectToAction(nameof(Details), new { projectId = project.Id });
                }

                var newEdit = Mapper.Map<EditProjectViewModel>(edit);

                newEdit.OriginalName = project.Name;
                newEdit.SessionDefinitionTemplates = GetSessionTemplatesList();

                return View(newEdit);
            }

            return HttpNotFound();
        }


        // GET: Studies/Project/5/Delete/
        [Route(Routes.Project.ACTION_DELETE)]
        public ActionResult Delete(int? projectId)
        {
            if (projectId.HasValue)
            {
                var project = _database.Projects
                                       .FilterById(projectId.Value)
                                       .AsDbQuery()
                                       .Include(p => p.Owner)
                                       .SingleOrDefault();

                var currentUser = _userManager.FindById(User.Identity.GetUserId());

                if (project != null && (project.Owner == currentUser || User.IsInRole(UserRoles.ADMIN)))
                {
                    return View(Mapper.Map<ProjectViewModel>(project));
                }

                return HttpNotFound();
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }


        // POST: Studies/Project/5/Delete/
        [Route(Routes.Project.ACTION_DELETE)]
        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int projectId)
        {
            Request.ThrowIfDifferentReferrer();

            var project = _database.Projects
                                   .FilterById(projectId)
                                   .AsDbQuery()
                                   .Include(p => p.Owner)
                                   .SingleOrDefault();

            var currentUser = _userManager.FindById(User.Identity.GetUserId());

            if (project != null 
                && (project.Owner == currentUser || User.IsInRole(UserRoles.ADMIN)))
            {
                var command = new DeleteProjectCommand()
                {
                    Project = project,
                };

                _recordings.DeleteProjectData(project);

                _dispatcher.Dispatch(command);

                return RedirectToAction(nameof(Index));
            }

            return HttpNotFound();
        }


        private List<SessionTemplateViewModel> GetSessionTemplatesList()
        {
            return _database.SessionTemplates
                            .AsDbQuery()
                            .Include(p => p.Author)
                            .Select(Mapper.Map<SessionTemplateViewModel>)
                            .ToList();
        }


        [HttpGet]
        [Route(Routes.Project.ACTION_DOWNLOAD)]
        public ActionResult Download(int? projectId)
        {
            if (projectId.HasValue)
            {
                Request.ThrowIfDifferentReferrer();

                var project = _database.Projects
                                       .FilterById(projectId.Value)
                                       .AsDbQuery()
                                       .Include(p => p.Owner)
                                       .SingleOrDefault();

                var currentUser = _userManager.FindById(User.Identity.GetUserId());

                if (project != null
                    && (project.Owner == currentUser || User.IsInRole(UserRoles.ADMIN)))
                {
                    var projectRecordings = _recordings.GetProjectRecordings(project);

                    if (projectRecordings.Any())
                    {
                        string filename = $"{project.Name}.zip";

                        return this.StreamFileResult(filename, "application/octet-stream", stream =>
                        {
                            _zip.ZipRecordingFiles(projectRecordings, stream, ZipHelper.RecordingFilesPathDepth.Session_Node);
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
