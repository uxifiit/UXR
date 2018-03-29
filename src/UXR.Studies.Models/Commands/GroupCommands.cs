using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXI.CQRS.Commands;

namespace UXR.Studies.Models.Commands
{
    public class CreateGroupCommand : ICommand

    {
        public string Name { get; set; }
        //public IEnumerable<string> Nodes { get; set; }
    }

    public class EditGroupCommand : ICommand
    {
        public Group Group { get; set; }
        public string Name { get; set; }
        //public IEnumerable<Node> Nodes { get; set; }
    }

    public class DeleteGroupCommand : ICommand
    {
        public Group Group { get; set; }
    }
}
