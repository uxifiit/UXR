using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXR.Studies.Models;

namespace UXR.Studies
{
    public class StudiesDatabase
    {
        private readonly IStudiesDbContext _context;

        public StudiesDatabase(IStudiesDbContext context)
        {
            _context = context;
        }

        public IQueryable<Project> Projects => _context.Projects;

        public IQueryable<Session> Sessions => _context.Sessions;

        public IQueryable<SessionTemplate> SessionTemplates => _context.SessionTemplates;

        public IQueryable<Node> Nodes => _context.Nodes;

        public IQueryable<Group> Groups => _context.Groups;

        public IQueryable<NodeStatus> NodeStates => _context.NodeStates;
    }
}
