using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXI.CQRS;

namespace UXR.Studies.Models
{
    public interface IStudiesDbContext : IDbContext
    {
        DbSet<Project> Projects { get; }

        DbSet<Session> Sessions { get; }

        DbSet<SessionTemplate> SessionTemplates { get; }

        DbSet<Node> Nodes { get; }

        DbSet<Group> Groups { get; }

        DbSet<NodeStatus> NodeStates { get; }
    }
}
