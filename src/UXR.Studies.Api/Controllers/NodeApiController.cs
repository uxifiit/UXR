using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using UXI.CQRS;
using UXR.Studies.Api.Entities;
using UXR.Studies.Api.Entities.Nodes;
using UXR.Studies.Models;
using UXR.Studies.Models.Commands;
using UXR.Studies.Models.Queries;

namespace UXR.Studies.Api.Controllers
{
    /// <summary>
    /// WebApi controller for nodes.
    /// </summary>
    [RoutePrefix(ApiRoutes.Node.PREFIX)]
    public class NodeApiController : ApiController
    {
        private static readonly IMapper Mapper;

        static NodeApiController()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<NodeStatus, NodeStatusInfo>()
                   .ForMember(u => u.LastUpdateAt, s => s.ResolveUsing(u => u.UpdatedAt ?? u.CreatedAt ?? DateTime.Now));

                cfg.CreateMap<Node, NodeIdInfo>();
            });

            Mapper = config.CreateMapper();
        }


        private readonly StudiesDatabase _database;
        private readonly CommandDispatcher _dispatcher;

        public NodeApiController(StudiesDatabase database, CommandDispatcher dispatcher)
        {
            _database = database;
            _dispatcher = dispatcher;
        }


        [HttpGet]
        [Route(ApiRoutes.Node.ACTION_LIST)]
        [ResponseType(typeof(List<NodeStatusInfo>))]
        public IHttpActionResult Index()
        {
            var nodes = _database.NodeStates
                                 .OrderBy(n => n.NodeId)
                                 .AsDbQuery()
                                 .Include(u => u.Node)
                                 .ToList();

            return Ok(nodes.Select(Mapper.Map<NodeStatusInfo>).ToArray());
        }


        // Test call with:
        //curl -H "Content-Type: application/json" -X POST -d "{\"Name\":\"1\",\"IsRecording\":true,\"SessionId\":\"0\"}" "http://localhost:15185/api/studies/node"
        [HttpPost]
        [Route(ApiRoutes.Node.ACTION_UPDATE)]
        [ResponseType(typeof(NodeIdInfo))]
        public IHttpActionResult Update([FromBody] NodeStatusUpdate status)
        {
            if (status != null)
            {
                string nodeName = status.Name;
                var node = _database.Nodes
                                    .FilterByName(nodeName)
                                    .SingleOrDefault();

                if (node != null)
                {
                    var command = new UpdateNodeStatusCommand()
                    {
                        Node = node,
                        IsRecording = status.Recording != null,
                        Session = status.Recording?.SessionName
                    };

                    _dispatcher.Dispatch(command);

                    return Ok(Mapper.Map<NodeIdInfo>(node));
                }

                return NotFound();
            }

            return BadRequest();
        }
    }
}
