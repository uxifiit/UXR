using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXI.CQRS.Commands;
using UXR.Models.Entities;

namespace UXR.Studies.Models.Commands
{
    public class CreateSessionTemplateCommand : ICommand
    {
        public string Name { get; set; }

        public string Definition { get; set; }

        public ApplicationUser Author { get; set; }
    }


    public class DeleteSessionTemplateCommand : ICommand
    {
        public SessionTemplate Template { get; set; }
    }
}
