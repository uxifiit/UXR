using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Studies.Models.Queries
{
    public static class GroupQueries
    {
        public static IQueryable<Group> FilterById(this IQueryable<Group> query, int groupId)
        {
            return query.Where(g => g.Id == groupId);
        }

        public static IQueryable<Group> FilterByName(this IQueryable<Group> query, string name)
        {
            return query.Where(g => g.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }

        public static IQueryable<Group> FilterByNames(this IQueryable<Group> query, IEnumerable<string> names)
        {
            return query.Where(g => names.Any(name => g.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)));
        }
    }
}
