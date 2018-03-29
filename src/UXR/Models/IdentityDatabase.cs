using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UXR.Models;
using UXR.Models.Entities;

namespace UXR.Models
{
    public class IdentityDatabase
    {
        private readonly IIdentityDbContext<ApplicationUser> _context;

        public IdentityDatabase(IIdentityDbContext<ApplicationUser> context)
        {
            _context = context;
        }
        
        public IQueryable<ApplicationUser> Users => _context.Users;

        public IQueryable<IdentityRole> Roles => _context.Roles;
    }
}
