using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace UXR.Models
{
    public interface IIdentityDbContext<TUser>
        where TUser : IdentityUser
    {
        IDbSet<IdentityRole> Roles { get; set; }
        IDbSet<TUser> Users { get; set; }
    }
}
