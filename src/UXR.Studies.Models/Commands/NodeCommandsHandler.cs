using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXI.Common.Extensions;
using UXI.CQRS.Commands;

namespace UXR.Studies.Models.Commands
{
    public class NodeCommandsHandler
      : ICommandHandler<CreateNodesCommand>
      , ICommandHandler<DeleteNodeCommand>
    {
        private readonly IStudiesDbContext _context;

        public NodeCommandsHandler(IStudiesDbContext context)
        {
            _context = context;
        }

        public void Handle(DeleteNodeCommand command)
        {
            command.Node.ThrowIfNull(nameof(command.Node));

            _context.Nodes.Remove(command.Node);
        }

        public void Handle(CreateNodesCommand command)
        {
            var nodeNames = command?.Names?
                                    .Select(n => n.Trim())
                                    .Distinct(StringComparer.InvariantCultureIgnoreCase)
                                    .ToList();

            nodeNames.ThrowIfNull(l => l.Any() == false, nameof(command.Names));
            command.Group.ThrowIfNull(nameof(command.Group));

            foreach (var name in nodeNames)
            {
                var node = new Node()
                {
                    Name = name,
                    Group = command.Group
                };
                _context.Nodes.Add(node);
            }
        }
    }
}
