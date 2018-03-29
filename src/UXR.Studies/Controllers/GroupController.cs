using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using System.Data.Entity.Core.Objects;
using UXI.Common.Web.Extensions;
using UXI.CQRS;
using UXR.Studies.Models;
using UXR.Studies.ViewModels.Groups;
using UXR.Studies.Models.Commands;
using UXR.Studies.Models.Queries;
using UXR.Models;

namespace UXR.Studies.Controllers
{
    [RouteArea(Routes.AREA_NAME, AreaPrefix = Routes.AREA_NAME)]
    [RoutePrefix(Routes.Group.PREFIX)]
    [Authorize(Roles = UserRoles.ADMIN)]
    public class GroupController : Controller
    {
        private static readonly IMapper Mapper;

        public static readonly string ControllerName = nameof(GroupController).Replace("Controller", "");

        static GroupController()
        {
            var config = new MapperConfiguration(cfg =>
            {
                // Model -> ViewModel
                cfg.CreateMap<Group, GroupViewModel>();
                cfg.CreateMap<Group, EditGroupViewModel>();
                cfg.CreateMap<Group, DeleteGroupViewModel>();
            });

            Mapper = config.CreateMapper();
        }


        private readonly CommandDispatcher _dispatcher;
        private readonly StudiesDatabase _database;

        private const int MIN_PAGE_SIZE = 20;

        public GroupController(StudiesDatabase database, CommandDispatcher dispatcher)
        {
            _database = database;
            _dispatcher = dispatcher;
        }



        [HttpGet]
        [Route("")]
        [Route(Routes.ACTION_INDEX)]
        public ActionResult Index()
        {
            var groups = _database.Groups
                                  .OrderBy(g => g.Name)
                                  .AsDbQuery()
                                  .Include(nameof(Group.Nodes))
                                  .ToList();

            return View(groups.Select(c => Mapper.Map<GroupViewModel>(c)).ToList());
        }


        [HttpGet]
        [Route(Routes.ACTION_CREATE)]
        public ActionResult Create()
        {
            return View(new CreateGroupViewModel());
        }


        [HttpPost]
        [Route(Routes.ACTION_CREATE)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateGroupViewModel group)
        {
            Request.ThrowIfDifferentReferrer();

            if (ModelState.IsValid)
            {
                string groupName = group.Name;
                bool exists = _database.Groups
                                       .FilterByName(groupName)
                                       .Any();

                if (exists == false)
                {
                    var command = new CreateGroupCommand() { Name = group.Name };
                    _dispatcher.Dispatch(command);

                    var createdGroup = _database.Groups
                                                .FilterByName(groupName)
                                                .SingleOrDefault();

                    return RedirectToAction(nameof(GroupNodeController.Index), GroupNodeController.ControllerName, new { groupId = createdGroup.Id });
                }

                ModelState.AddModelError(nameof(group.Name), "A group with this name already exists.");
            }

            return View(group);
        }


        // TODO Query
        [HttpGet]
        [Route(Routes.Group.ACTION_EDIT)]
        public ActionResult Edit(int groupId)
        {
            var group = _database.Groups
                                 .FilterById(groupId)
                                 .SingleOrDefault();

            if (group != null)
            {
                return View(Mapper.Map<EditGroupViewModel>(group));
            }

            return HttpNotFound();
        }


        // TODO Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route(Routes.Group.ACTION_EDIT)]
        public ActionResult Edit(int groupId, EditGroupViewModel edit)
        {
            Request.ThrowIfDifferentReferrer();

            if (ModelState.IsValid)
            {
                var group = _database.Groups
                                     .FilterById(groupId)
                                     .SingleOrDefault();

                if (group != null)
                {
                    var command = new EditGroupCommand()
                    {
                        Group = group,
                        Name = edit.Name
                    };
                    _dispatcher.Dispatch(command);

                    return RedirectToAction(nameof(Index));
                }
                return HttpNotFound();
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }


       

        [HttpGet]
        [Route(Routes.Group.ACTION_DELETE)]
        public ActionResult Delete(int groupId)
        {
            var group = _database.Groups
                                 .FilterById(groupId)
                                 .SingleOrDefault();
            if (group != null)
            {
                return View(Mapper.Map<DeleteGroupViewModel>(group));    // TODO
            }

            return HttpNotFound();
        }


        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        [Route(Routes.Group.ACTION_DELETE)]
        public ActionResult DeleteConfirmed(int groupId)
        {
            Request.ThrowIfDifferentReferrer();

            var group = _database.Groups
                                 .FilterById(groupId)
                                 .SingleOrDefault();
            if (group != null)
            {
                var command = new DeleteGroupCommand()
                {
                    Group = group
                };

                _dispatcher.Dispatch(command);

                return RedirectToAction(nameof(Index));
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
    }
}
