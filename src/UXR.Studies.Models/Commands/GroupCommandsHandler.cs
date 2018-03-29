using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXI.Common.Extensions;
using UXI.CQRS.Commands;

namespace UXR.Studies.Models.Commands
{
    public class GroupCommandsHandler
       : ICommandHandler<CreateGroupCommand>
       , ICommandHandler<EditGroupCommand>
       , ICommandHandler<DeleteGroupCommand>
    {
        private readonly IStudiesDbContext _context;

        public GroupCommandsHandler(IStudiesDbContext context)
        {
            _context = context;
        }

        public void Handle(DeleteGroupCommand command)
        {
            command.Group.ThrowIfNull(nameof(command.Group));

            _context.Groups.Remove(command.Group);
        }

        public void Handle(CreateGroupCommand command)
        {
            command.Name.ThrowIf(String.IsNullOrWhiteSpace, nameof(command.Name));

            var group = new Group() { Name = command.Name };

            _context.Groups.Add(group);
        }

        public void Handle(EditGroupCommand command)
        {
            command.Group.ThrowIfNull(nameof(command.Group));

            var group = command.Group;

            if (group.Name.Equals(command.Name, StringComparison.CurrentCultureIgnoreCase) == false)
            {
                group.Name = command.Name;
            }
        }
    }
}
