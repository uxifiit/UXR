using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using UXR.Models;
using UXR.Studies.Models;
using UXR.Studies.Models.Queries;
using UXR.Studies.ViewModels.Dashboard;

namespace UXR.Studies.Controllers
{
    [RouteArea(Routes.AREA_NAME, AreaPrefix = Routes.AREA_NAME)]
    [RoutePrefix(Routes.Dashboard.PREFIX)]
    [Authorize(Roles = UserRoles.APPROVED)]
    public class DashboardController : Controller
    {
        private static readonly IMapper Mapper;

        public static readonly string ControllerName = nameof(DashboardController).Replace("Controller", "");

        static DashboardController()
        {
            var config = new MapperConfiguration(cfg => {
                // Model -> ViewModel
                cfg.CreateMap<NodeStatus, NodeStatusViewModel>()
                   .ForMember(s => s.UpdatedAt, s => s.ResolveUsing(n => n.UpdatedAt?.ToLocalTime() ?? n.CreatedAt?.ToLocalTime() ?? DateTime.MinValue));
            });

            Mapper = config.CreateMapper();
        }


        private readonly StudiesDatabase _database;

        public DashboardController(StudiesDatabase database)
        {
            _database = database;
        }


        [Route("")]
        [Route(Routes.ACTION_INDEX)]
        public ActionResult Index()
        {
            return View(GetDashboardViewModel());
        }


        [Route(Routes.Dashboard.ACTION_NODES)]
        public ActionResult NodeStatusBoard()
        {
            return PartialView(GetDashboardViewModel());
        }


        private DashboardViewModel GetDashboardViewModel()
        {
            var nodeStates = _database.NodeStates
                                      .AsDbQuery()
                                      .Include(n => n.Node.Group)
                                      .GroupBy(n => n.Node.Group)
                                      .ToList()
                                      .OrderBy(g => g.Key.Name)
                                      .Select(g => new NodeStatusGroupViewModel(g.Key.Name, g.OrderBy(n => n.Node.Name).Select(Mapper.Map<NodeStatusViewModel>)));

            return new DashboardViewModel(nodeStates);
        }
    }
}
