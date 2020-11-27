using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APILibrary.Core.Pagination
{
    class PageResponse<T>
    {
        public PageResponse() { }
        public PageResponse(T data,string[] Tab,int MaxCollectionSize,HttpRequest request)
        {
            Data = data;
            var start = Int16.Parse(Tab[0]);
            var Endindex = Int16.Parse(Tab[1]);

            Rel_First = $"{request.Path}/{request.QueryString}";

            if (start - Page_Maxsize < 0)
                Rel_Previous = "0";

            if( Endindex + Page_Maxsize < MaxCollectionSize)
               Rel_Next= $"?range=[{Endindex},{Endindex +Page_Maxsize}]";

        }

        public int Get_MaxpageSize() { return Page_Maxsize; }

        private readonly int Page_Maxsize = 50;
        public T Data{get;set;}
        public string Rel_First { get; set; }
        public string Rel_Next { get; set; }
        public string Rel_Previous { get; set; }
        public string Rel_Last { get; set; }
    }
}
