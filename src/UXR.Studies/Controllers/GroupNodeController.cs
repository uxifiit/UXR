using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using PagedList;
using System.Net;
using UXI.Common.Web.Extensions;
using UXI.CQRS;
using UXR.Studies.Models;
using UXR.Studies.Models.Queries;
using UXR.Studies.Models.Commands;
using UXR.Studies.ViewModels.GroupNodes;
using UXR.Models;

namespace UXR.Studies.Controllers
{
    [RouteArea(Routes.AREA_NAME, AreaPrefix = Routes.AREA_NAME)]
    [RoutePrefix(Routes.Group.Node.PREFIX)]
    [Authorize(Roles = UserRoles.ADMIN)]
    public class GroupNodeController : Controller
    {
        private static readonly IMapper Mapper;

        public static readonly string ControllerName = nameof(GroupNodeController).Replace("Controller", "");

        static GroupNodeController()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Group, GroupNodesViewModel>().ForMember(g => g.Nodes, g => g.Ignore());
                cfg.CreateMap<Node, GroupNodeViewModel>();
            });

            Mapper = config.CreateMapper();
        }


        private readonly CommandDispatcher _dispatcher;
        private readonly StudiesDatabase _database;

        private const int MIN_PAGE_SIZE = 20;

        public GroupNodeController(StudiesDatabase database, CommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            _database = database;
        }

        [HttpGet]
        [Route("")]
        [Route(Routes.ACTION_INDEX)]
        public ActionResult Index(int groupId, int? limit, int? page)
        {
            int pageSize = Math.Max(limit ?? MIN_PAGE_SIZE, MIN_PAGE_SIZE);
            int pageNumber = Math.Max(page ?? 1, 1);

            var group = _database.Groups
                                 .FilterById(groupId)
                                 .SingleOrDefault();

            if (group != null)
            {
                var nodes = group.Nodes
                                 .AsQueryable()
                                 .OrderBy(n => n.Name)
                                 .Page(pageNumber, pageSize)
                                 .ToList();

                var viewModel = Mapper.Map<GroupNodesViewModel>(group);
                viewModel.Nodes = new PagedList<GroupNodeViewModel>(nodes.Select(Mapper.Map<GroupNodeViewModel>), pageNumber, pageSize);

                return View(viewModel);
            }

            return HttpNotFound();
        }

        [HttpGet]
        [Route(Routes.ACTION_CREATE)]
        public ActionResult Create(int groupId)
        {
            var group = _database.Groups
                                 .FilterById(groupId)
                                 .SingleOrDefault();
            if (group != null)
            {
                return View(new CreateNodeViewModel() { GroupName = group.Name });
            }

            return HttpNotFound();
        }


        [HttpPost]
        [Route(Routes.ACTION_CREATE)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int groupId, CreateNodeViewModel node)
        {
            Request.ThrowIfDifferentReferrer();

            var group = _database.Groups
                                 .FilterById(groupId)
                                 .SingleOrDefault();
            if (group != null)
            {
                if (ModelState.IsValid)
                {
                    List<string> nodeNames = node.Name
                                                 .Split(new string[] { Environment.NewLine, ";", "," }, StringSplitOptions.RemoveEmptyEntries)
                                                 .Select(n => n.Trim())
                                                 .Distinct(StringComparer.InvariantCultureIgnoreCase)
                                                 .ToList();
                    if (nodeNames.Any())
                    {
                        var existingNodeNames = _database.Nodes
                                                         .FilterByNames(nodeNames)
                                                         .Select(n => n.Name)
                                                         .ToList();

                        if (existingNodeNames.Any() == false)
                        {
                            var command = new CreateNodesCommand()
                            {
                                Names = nodeNames,
                                Group = group
                            };

                            _dispatcher.Dispatch(command);

                            return RedirectToAction(nameof(Index));
                        }

                        string existingNames = String.Join(", ", existingNodeNames);

                        ModelState.AddModelError(nameof(node.Name), $"Nodes with these names already exists: {existingNames}");
                    }
                }

                node.GroupName = group.Name;
                return View(node);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }


        [HttpPost]
        [Route(Routes.Group.Node.ACTION_DELETE)]
        public ActionResult Delete(int groupId, int nodeId)
        {
            Request.ThrowIfDifferentReferrer();

            var node = _database.Nodes
                                .SingleOrDefault(n => n.GroupId == groupId
                                                      && n.Id == nodeId);

            if (node != null)
            {
                var command = new DeleteNodeCommand()
                {
                    Node = node
                };

                _dispatcher.Dispatch(command);

                return RedirectToAction(nameof(Index));
            }

            return HttpNotFound();
        }
    }
}
