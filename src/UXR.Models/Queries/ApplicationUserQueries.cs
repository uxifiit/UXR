using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXR.Models.Entities;

namespace UXR.Models.Queries
{
    public static class ApplicationUserQueries
    {
        public static IQueryable<ApplicationUser> FilterById(this IQueryable<ApplicationUser> query, string id)
        {
            return query.Where(u => u.Id == id);
        }
    }
}
