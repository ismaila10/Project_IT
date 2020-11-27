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

            rel_First = $"{request.Path}/{request.QueryString}";

            if (start - Page_Maxsize < 0)
                rel_Previous = "0";

            if( Endindex + Page_Maxsize < MaxCollectionSize)
               rel_Next= $"?range=[{Endindex},{Endindex +Page_Maxsize}]";

        }
        //By Project_IT
        public int Get_MaxpageSize() { return Page_Maxsize; }

        private int Page_Maxsize = 50;
        public T Data{get;set;}
        public string rel_First { get; set; }
        public string rel_Next { get; set; }
        public string rel_Previous { get; set; }
        public string rel_Last { get; set; }
    }
}
