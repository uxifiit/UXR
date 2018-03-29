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
    public class ProjectCommandsHandler
        : ICommandHandler<CreateProjectCommand>
        , ICommandHandler<EditProjectCommand>
        , ICommandHandler<DeleteProjectCommand>
    {
        private readonly IStudiesDbContext _context;

        public ProjectCommandsHandler(IStudiesDbContext context)
        {
            _context = context;
        }

        public void Handle(CreateProjectCommand command)
        {
            Project project = new Project()
            {
                Name = command.Name,
                Description = command.Description ?? String.Empty,
                SessionDefinitionTemplate = command.SessionDefinitionTemplate ?? String.Empty
            };

            _context.Projects.Add(project);

            project.Owner = command.Owner;

            _context.SaveChanges();

            command.NewId = project.Id;
        }

        public void Handle(EditProjectCommand command)
        {
            // TODO - Check correctness of input
            string newName = command.Name.Trim();
            if (command.Project.Name?.Trim() != newName)
            {
                command.Project.Name = newName;
            }

            string newDescription = command.Description?.Trim() ?? String.Empty;
            if (command.Project.Description?.Trim() != newDescription)
            {
                command.Project.Description = newDescription;
            }

            string newTemplate = command.SessionDefinitionTemplate?.Trim() ?? String.Empty;
            if (command.Project.SessionDefinitionTemplate?.Trim() != newTemplate)
            {
                command.Project.SessionDefinitionTemplate = newTemplate;
            }
        }

        public void Handle(DeleteProjectCommand command)
        {
            _context.Projects.Remove(command.Project);
        }
    }
}
