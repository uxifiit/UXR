using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Studies.Models.Queries
{
    public static class ProjectQueries
    {
        public static IQueryable<Project> FilterByUserRights(this IQueryable<Project> projects, string userId, bool isAdmin)
        {
            return isAdmin
                 ? projects
                 : projects.Where(p => p.Owner.Id == userId);
        }
    }
}
