using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Models
{
    public class ForceRecreateMainDbInitializer : DropCreateDatabaseAlways<UXRDbContext>
    {
        public List<IPartialDbInitializer<UXRDbContext>> PartialInitializers { get; set; }

        protected override void Seed(UXRDbContext context)
        {
            foreach (IPartialDbInitializer<UXRDbContext> initializer in PartialInitializers)
            {
                initializer.Seed(context);
            }

            context.SaveChanges();
        }
    }
}
