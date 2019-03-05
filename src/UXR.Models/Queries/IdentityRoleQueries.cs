using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace UXR.Models.Queries
{
    public static class IdentityRoleQueries
    {
        public static IQueryable<IdentityRole> FilterByName(this IQueryable<IdentityRole> query, string name)
        {
            return query.Where(r => r.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }


        public static IdentityRole GetOrDefault(this IQueryable<IdentityRole> query, string name)
        {
            return query.FilterByName(name).SingleOrDefault();
        }
    }
}