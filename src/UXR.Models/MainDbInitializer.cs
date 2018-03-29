using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Models
{
    public class MainDbInitializer : IDatabaseInitializer<UXRDbContext>
    {
        private IDatabaseInitializer<UXRDbContext> _migrationsInitializer = new MigrateDatabaseToLatestVersion<UXRDbContext, Migrations.Configuration>();

        public List<IPartialDbInitializer<UXRDbContext>> PartialInitializers { get; set; }

        public void InitializeDatabase(UXRDbContext context)
        {
            _migrationsInitializer.InitializeDatabase(context);

            Seed(context);
        }

        private void Seed(UXRDbContext context)
        {
            foreach (IPartialDbInitializer<UXRDbContext> initializer in PartialInitializers)
            {
                initializer.Seed(context);
            }

            context.SaveChanges();
        }
    }
}
