using APILibrary.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APILibrary.Core.Extensions
{
    public static class PaginationExtensions
    {
        public static IQueryable<TModel> Skips<TModel>(this IQueryable<TModel> query, int offset, int limit) where TModel : ModelBase
        {
            int Star = offset - 1;
            if (Star < 0)
                Star = 0;

            if (limit > query.Count())
                limit = query.Count();

            return query.Skip(Star).Take(limit - limit + 1);
        }
    }
}
