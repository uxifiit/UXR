using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXI.CQRS.Commands;
using UXR.Studies.Models;

namespace UXR.Studies.Models.Commands
{
    public class UpdateNodeStatusCommand : ICommand
    {
        public Node Node { get; set; }

        public bool IsRecording { get; set; }

        public string Session { get; set; }
    }
}
