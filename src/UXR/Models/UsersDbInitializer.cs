using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXR.Models.Entities;
using System.Collections.Specialized;
using System.Configuration;

namespace UXR.Models
{
    public class UsersDbInitializer : IPartialDbInitializer<UXRDbContext>
    {
        public void Seed(UXRDbContext context)
        {
            var superAdminRole = context.Roles.FirstOrDefault(r => r.Name == UserRoles.SUPERADMIN);

            if (superAdminRole == null)
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                superAdminRole = new IdentityRole { Name = UserRoles.SUPERADMIN };

                manager.Create(superAdminRole);
            }

            if (!context.Roles.Any(r => r.Name == UserRoles.ADMIN))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = UserRoles.ADMIN };

                manager.Create(role);
            }

            if (!context.Roles.Any(r => r.Name == UserRoles.APPROVED))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = UserRoles.APPROVED };

                manager.Create(role);
            }

         
            if (superAdminRole.Users.Any() == false)
            {
                var deploymentConfig = ((NameValueCollection)ConfigurationManager.GetSection("DeploymentConfig"));
                
                string superAdminUserName = deploymentConfig["FirstUserName"];
                string superAdminDefaultPassword = deploymentConfig["FirstUserPassword"];

                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser { UserName = superAdminUserName, Email = superAdminUserName };

                manager.Create(user, superAdminDefaultPassword);
                manager.AddToRole(user.Id, UserRoles.SUPERADMIN);
                manager.AddToRole(user.Id, UserRoles.ADMIN);
                manager.AddToRole(user.Id, UserRoles.APPROVED);
            }
        }
    }
}
