using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using UXR.Studies.Models;
using UXR.Studies.Models.Configurations;
using Microsoft.AspNet.Identity.EntityFramework;
using UXR.Models.Entities;
using System.Threading.Tasks;

namespace UXR.Models
{
    public class UXRDbContext : IdentityDbContext<ApplicationUser>, IIdentityDbContext<ApplicationUser>, IStudiesDbContext
    {
        public UXRDbContext() : this("UXRDbContext") { }
        public UXRDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
          
        }

        public DbSet<Node> Nodes { get { return Set<Node>(); } }
        public DbSet<Group> Groups { get { return Set<Group>(); } }
        public DbSet<Project> Projects { get { return Set<Project>(); } }
        public DbSet<Session> Sessions { get { return Set<Session>(); } }
        public DbSet<SessionTemplate> SessionTemplates { get { return Set<SessionTemplate>(); } }
        public DbSet<NodeStatus> NodeStates { get { return Set<NodeStatus>(); } }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new NodeStatusConfiguration());
            modelBuilder.Configurations.Add(new ProjectConfiguration());
            modelBuilder.Configurations.Add(new NodeConfiguration());
        }

        public override int SaveChanges()
        {
            AddTimestamps();

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync()
        {
            AddTimestamps();

            return await base.SaveChangesAsync();
        }

        private void AddTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity)
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            var now = DateTime.UtcNow; // current datetime

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = now;
                }

                entity.UpdatedAt = now;
            }
        }
    }
}
