using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Studies.Models.Queries
{
    public static class NodeQueries
    {
        public static IQueryable<Node> FilterById(this IQueryable<Node> nodes, int nodeId)
        {
            return nodes.Where(node => node.Id == nodeId);
        }

        public static IQueryable<Node> FilterByName(this IQueryable<Node> query, string name)
        {
            return query.Where(n => n.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }

        public static IQueryable<Node> FilterByNames(this IQueryable<Node> query, IEnumerable<string> names)
        {
            return query.Where(n => names.Any(name => n.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)));
        }
    }
}
