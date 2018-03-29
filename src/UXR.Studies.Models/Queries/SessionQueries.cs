using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Studies.Models.Queries
{
    public static class SessionQueries
    {
        public static IQueryable<Session> FilterByName(this IQueryable<Session> query, string name)
        {
            return query.Where(n => n.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }


        public static IQueryable<Session> FilterById(this IQueryable<Session> query, int sessionId)
        {
            return query.Where(session => session.Id == sessionId);
        }
    }
}
