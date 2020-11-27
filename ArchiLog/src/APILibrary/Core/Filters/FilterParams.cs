using System;
using static APILibrary.Core.Filters.FilterUtility;

namespace APILibrary.Core.Filters
{
    public class FilterParams
    {
        public string ColumnName { get; set; } = string.Empty;
        public string FilterValue { get; set; } = string.Empty;
        public FilterOptions FilterOption { get; set; } = FilterOptions.Contains;
    }
}
