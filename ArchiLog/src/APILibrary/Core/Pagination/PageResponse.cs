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
        public PageResponse(T data,int MaxCollectionSize, HttpRequest request)
        {
            Data = data;
            //evalute the value of Total Page to display
            Total_Page = MaxCollectionSize / Page_Maxsize < 1 ? 1 : (MaxCollectionSize / Page_Maxsize);

        }

        public int Get_MaxpageSize() { return Page_Maxsize; }
        private int Page_Maxsize = 50;
        
        public T Data { get; set; }
        public int Total_Page { private set; get; }
        public string rel_First { get; set; }
        public string rel_Next { get; set; }
        public string rel_Previous { get; set; }
        public string rel_Last { get; set; }
    }
}