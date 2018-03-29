using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXI.CQRS.Commands;
using UXR.Common;
using UXR.Studies.Models;

namespace UXR.Studies.Models.Commands
{
    public class NodeStatusCommandsHandler
        : ICommandHandler<UpdateNodeStatusCommand>
    {
        private readonly IStudiesDbContext _context;
        private readonly ITimeProvider _timeProvider;

        public NodeStatusCommandsHandler(IStudiesDbContext context, ITimeProvider timeProvider)
        {
            _context = context;
            _timeProvider = timeProvider;
        }

        public void Handle(UpdateNodeStatusCommand command)
        {
            var state = _context.NodeStates
                                .SingleOrDefault(n => n.NodeId == command.Node.Id);

            if (state == null)
            {
                var create = new NodeStatus()
                {
                    Node = command.Node,
                    IsRecording = command.IsRecording,
                    Session = command.Session
                };
                _context.NodeStates.Add(create);
            }
            else
            {
                state.IsRecording = command.IsRecording;
                state.Session = command.Session;
            }
        }
    }
}
