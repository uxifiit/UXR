using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXI.CQRS.Commands;
using UXR.Studies.Models;

namespace UXR.Studies.Models.Commands
{
    public abstract class SessionCommandBase : ICommand
    {
        public string Name { get; set; }

        public DateTime StartTime { get; set; }

        public TimeSpan Length { get; set; }

        public string Definition { get; set; }
    }

    public class CreateSessionCommand : SessionCommandBase
    {
        public Project Project { get; set; }

        public int NewId { get; set; }
    }

    public class EditSessionCommand : SessionCommandBase
    {
        public Session Session { get; set; }
        public Project Project { get; set; }
    }

    public class DeleteSessionCommand : ICommand
    {
        public Session Session { get; set; }
    }

}
