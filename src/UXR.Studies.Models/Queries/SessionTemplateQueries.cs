using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Studies.Models.Queries
{
    public static class SessionTemplateQueries
    {
        public static IQueryable<SessionTemplate> FilterById(this IQueryable<SessionTemplate> query, int templateId)
        {
            return query.Where(template => template.Id == templateId);
        }
    }
}
