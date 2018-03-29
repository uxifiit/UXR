using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Studies.Models.Queries
{
    public static class ProjectQueries
    {
        public static IQueryable<Project> FilterByName(this IQueryable<Project> query, string name)
        {
            return query.Where(project => project.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }


        public static IQueryable<Project> FilterById(this IQueryable<Project> query, int projectId)
        {
            return query.Where(project => project.Id == projectId);
        }
    }
}
