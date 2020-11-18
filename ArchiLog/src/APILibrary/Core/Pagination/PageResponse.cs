using System;
using System.Collections.Generic;
using System.Text;

namespace APILibrary.Core.Pagination
{
    class PageResponse<T>
    {
        public PageResponse() { }
        public PageResponse(IEnumerable<dynamic> data)
        {
            Data = data;
        }

        public IEnumerable<dynamic> Data{get;set;}

        public int? PageNumber { get; set; }

        public int PageSize { get; set; }
        public string NextPage { get; set; }
        public string PreviousPage { get; set; }
    }
}
