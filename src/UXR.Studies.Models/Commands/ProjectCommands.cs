using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXI.CQRS.Commands;
using UXR.Studies.Models;
using UXR.Models.Entities;

namespace UXR.Studies.Models.Commands
{
    public class CreateProjectCommand : ICommand
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ApplicationUser Owner { get; set; }

        public string SessionDefinitionTemplate { get; set; }

        public int NewId { get; set; }
    }


    public class EditProjectCommand : ICommand
    {
        public Project Project { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string SessionDefinitionTemplate { get; set; }
    }


    public class DeleteProjectCommand : ICommand
    {
        public Project Project { get; set; }
    }
}
