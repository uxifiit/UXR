//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web;
//using System.Web.Mvc;
//using AutoMapper;
//using PagedList;
//using System.Net;
//using UXI.Common.Web.Extensions;
//using UXI.CQRS;
//using UXR.Studies.Models;
//using UXR.Studies.ViewModels.Nodes;
//using UXR.Studies.Models.Queries;
//using UXR.Studies.Models.Commands;

//namespace UXR.Studies.Controllers
//{
//    [RouteArea(Routes.AREA_NAME, AreaPrefix = Routes.AREA_NAME)]
//    [RoutePrefix(Routes.Node.PREFIX)]
//    public class NodeController : Controller
//    {
//        private static readonly IMapper Mapper;

//        public static readonly string ControllerName = nameof(NodeController).Replace("Controller", "");

//        static NodeController()
//        {
//            var config = new MapperConfiguration(cfg =>
//            {
//                cfg.CreateMap<Node, NodeViewModel>();
//            });

//            Mapper = config.CreateMapper();
//        }


//        private readonly CommandDispatcher _dispatcher;
//        private readonly StudiesDatabase _database;

//        private const int MIN_PAGE_SIZE = 20;

//        public NodeController(StudiesDatabase database, CommandDispatcher dispatcher)
//        {
//            _dispatcher = dispatcher;
//            _database = database;
//        }
//    }
//}
