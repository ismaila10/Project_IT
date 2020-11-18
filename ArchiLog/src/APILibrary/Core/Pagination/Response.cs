using System;
using System.Collections.Generic;
using System.Text;

namespace APILibrary.Core.Pagination
{
    class Response<T>
    {
        public Response() { }
        public Response(T response)
        {
            Data = response;
        }

        public T Data { get; set; }

        public int? PageNumber { get; set; }

        public int PageSize { get; set; }
        public string NextPage { get; set; }
        public string PreviousPage { get; set; }



    }
}
