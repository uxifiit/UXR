using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXI.CQRS.Commands;
using UXR.Studies.Models;

namespace UXR.Studies.Models.Commands
{
    public class SessionCommandsHandler
        : ICommandHandler<CreateSessionCommand>
        , ICommandHandler<EditSessionCommand>
        , ICommandHandler<DeleteSessionCommand>
    {
        private readonly IStudiesDbContext _context;

        public SessionCommandsHandler(IStudiesDbContext context)
        {
            _context = context;
        }

        public void Handle(CreateSessionCommand command)
        {
            Session session = new Session()
            {
                Name = command.Name,
                StartTime = command.StartTime,
                Length = command.Length,
                Project = command.Project,
                Definition = command.Definition
            };

            _context.Sessions.Add(session);
            _context.SaveChanges();
            command.NewId = session.Id;

        }

        public void Handle(EditSessionCommand command)
        {
            if (command.Session.Name != command.Name)
                command.Session.Name = command.Name;

            if (command.Session.StartTime != command.StartTime)
                command.Session.StartTime = command.StartTime;

            if (command.Session.Length != command.Length)
                command.Session.Length = command.Length;

            if (command.Session.ProjectId != command.Project.Id)
                command.Session.Project = command.Project;

            if (command.Session.Definition != command.Definition)
                command.Session.Definition = command.Definition;
        }

        public void Handle(DeleteSessionCommand command)
        {
            _context.Sessions.Remove(command.Session);
        }

    }
}
