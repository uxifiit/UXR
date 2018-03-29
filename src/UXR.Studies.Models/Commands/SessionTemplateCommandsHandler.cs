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
    public class SessionTemplateCommandsHandler
        : ICommandHandler<CreateSessionTemplateCommand>
        , ICommandHandler<DeleteSessionTemplateCommand>
    {
        private readonly IStudiesDbContext _context;

        public SessionTemplateCommandsHandler(IStudiesDbContext context)
        {
            _context = context;
        }

        public void Handle(CreateSessionTemplateCommand command)
        {
            SessionTemplate template = new SessionTemplate()
            {
                Name = command.Name,
                Definition = command.Definition,
            };

            _context.SessionTemplates.Add(template);

            template.Author = command.Author;
        }

  
        public void Handle(DeleteSessionTemplateCommand command)
        {
            _context.SessionTemplates.Remove(command.Template);
        }
    }
}
