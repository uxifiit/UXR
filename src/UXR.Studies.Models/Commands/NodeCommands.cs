using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXI.CQRS.Commands;

namespace UXR.Studies.Models.Commands
{
    public class CreateNodesCommand : ICommand
    {
        public List<string> Names { get; set; }
        public Group Group { get; set; }
    }
    public class DeleteNodeCommand : ICommand
    {
        public Node Node { get; set; }
    }
}
