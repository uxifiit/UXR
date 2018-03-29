using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXR.Studies.Models.Queries
{
    public static class CommonQueries
    {
        public static IQueryable<T> Page<T>(this IQueryable<T> query, int page, int pagesize)
        {
            return query.Skip((page - 1) * pagesize).Take(pagesize);
        }

        /// <summary>
        /// Attempts to cast an <see cref="IQueryable{T}"/> query into a <see cref="DbQuery{TResult}"/>, so the consumer may use its <see cref="DbQuery{TResult}.Include(string)"/> methods.  
        /// </summary>
        /// <typeparam name="T">Type of the data in the data source.</typeparam>
        /// <param name="query">Query to convert.</param>
        /// <returns>reference to the instance of <see cref="DbQuery{TResult}"/> if the query is implemented with this type; otherwise, null.</returns>
        public static DbQuery<T> AsDbQuery<T>(this IQueryable<T> query)
        {
            return query as DbQuery<T>;
        }
    }
}
