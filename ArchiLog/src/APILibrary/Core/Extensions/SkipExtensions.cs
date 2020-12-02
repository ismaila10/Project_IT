using APILibrary.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APILibrary.Core.Extensions
{
    public static class SkipExtensions
    {
        public static IQueryable<TModel> Skip<TModel>(this IQueryable<TModel> source, int offset, int limit) where TModel : ModelBase
        {
            
            return source.Skip(offset).Take(limit);
        }
    }
}
