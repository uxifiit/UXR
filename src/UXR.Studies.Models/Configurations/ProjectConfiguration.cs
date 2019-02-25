using System.Data.Entity.ModelConfiguration;


namespace UXR.Studies.Models.Configurations
{
    public class ProjectConfiguration : EntityTypeConfiguration<Project>
    {
        public ProjectConfiguration()
        {
            this.HasMany(p => p.Sessions)
                .WithRequired(pss => pss.Project)
                .WillCascadeOnDelete();
        }
    }

    public class NodeStatusConfiguration : EntityTypeConfiguration<NodeStatus>
    {
        public NodeStatusConfiguration()
        {
            this.HasRequired(u => u.Node)
                .WithOptional(n => n.Status)
                .WillCascadeOnDelete();
        }
    }


    public class GroupConfiguration : EntityTypeConfiguration<Group>
    {
        public GroupConfiguration()
        {
        }
    }


    public class NodeConfiguration : EntityTypeConfiguration<Node>
    {
        public NodeConfiguration()
        {
            this.HasRequired(n => n.Group)
                .WithMany(g => g.Nodes)
                .WillCascadeOnDelete();
        }
    }
}
