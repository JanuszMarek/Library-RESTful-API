using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library_RESTful_API.Helpers
{
    public class PaginationMetaData
    {
        public PaginationMetaData(IPagedList list, string prevLink, string nextLink)
        {
            CurrentPage = list.CurrentPage;
            TotalPages = list.TotalPages;
            PageSize = list.PageSize;
            TotalCount = list.TotalCount;
            PreviousePageLink = prevLink;
            NextPageLink = nextLink;
        }

        public int CurrentPage { get; private set; }

        public int TotalPages { get; private set; }

        public int PageSize { get; private set; }

        public int TotalCount { get; private set; }

        public string PreviousePageLink { get; set; }

        public string NextPageLink { get; set; }
    }
}
