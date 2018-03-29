using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using UXI.CQRS;
using UXR.Models;
using UXR.Models.Entities;
using UXR.Studies.Models;
using UXR.Studies.ViewModels.SessionTemplates;
using UXR.Studies.Models.Commands;
using UXR.Studies.Models.Queries;
using System.Net;
using UXI.Common.Web.Extensions;
using UXR.Studies.ViewModels.Users;

namespace UXR.Studies.Controllers
{
    [RouteArea(Routes.AREA_NAME, AreaPrefix = Routes.AREA_NAME)]
    [RoutePrefix(Routes.Session.Template.PREFIX)]
    [Authorize(Roles = UserRoles.ADMIN)]
    public class SessionTemplateController : Controller
    {
        private static readonly IMapper Mapper;

        public static readonly string ControllerName = nameof(SessionTemplateController).Replace("Controller", "");

        static SessionTemplateController()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApplicationUser, UserNameViewModel>();

                cfg.CreateMap<SessionTemplate, SessionTemplateViewModel>();
            });

            Mapper = config.CreateMapper();
        }


        private readonly StudiesDatabase _database;
        private readonly CommandDispatcher _dispatcher;
        private readonly UserManager<ApplicationUser> _userManager;

        public SessionTemplateController(StudiesDatabase database, CommandDispatcher dispatcher, UserManager<ApplicationUser> userManager)
        {
            _database = database;
            _dispatcher = dispatcher;
            _userManager = userManager;
        }


        [Route(Routes.ACTION_INDEX)]
        public ActionResult Index()
        {
            var templates = _database.SessionTemplates
                                     .OrderBy(t => t.Name)
                                     .AsDbQuery()
                                     .Include(t => t.Author)
                                     .Select(Mapper.Map<SessionTemplateViewModel>)
                                     .ToList();

            return View(templates);
        }


        [Route(Routes.ACTION_CREATE)]
        public ActionResult Create()
        {
            return View(new CreateSessionTemplateViewModel());
        }


        [HttpPost]
        [Route(Routes.ACTION_CREATE)]
        public ActionResult Create(CreateSessionTemplateViewModel create)
        {
            var currentUser = _userManager.FindById(User.Identity.GetUserId());

            if (ModelState.IsValid && currentUser != null
                && Validation.SessionDefinitionValidation.CheckDefinitionTemplateJson(ModelState, nameof(create.Definition), create.Definition))
            {
                var command = new CreateSessionTemplateCommand()
                {
                    Author = currentUser,
                    Name = create.Name,
                    Definition = create.Definition
                };

                _dispatcher.Dispatch(command);

                return RedirectToAction(nameof(Index));
            }

            return View(create);
        }

        [Route(Routes.Session.Template.ACTION_DELETE)]
        public ActionResult Delete(int? templateId)
        {
            if (templateId.HasValue)
            {
                var template = _database.SessionTemplates
                                        .FilterById(templateId.Value)
                                        .AsDbQuery()
                                        .Include(t => t.Author)
                                        .SingleOrDefault();

                if (template != null)
                {
                    return View(Mapper.Map<SessionTemplateViewModel>(template));
                }

                return HttpNotFound();
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [Route(Routes.Session.Template.ACTION_DELETE)]
        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int templateId)
        {
            Request.ThrowIfDifferentReferrer();

            var template = _database.SessionTemplates
                                    .FilterById(templateId)
                                    .AsDbQuery()
                                    .SingleOrDefault();

            if (template != null)
            {
                var command = new DeleteSessionTemplateCommand()
                {
                    Template = template,
                };

                _dispatcher.Dispatch(command);

                return RedirectToAction(nameof(Index));
            }

            return HttpNotFound();
        }
    }
}
