using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library_RESTful_API.Helpers
{
    public interface IPagedList
    {
        int CurrentPage { get; }

        int TotalPages { get; }

        int PageSize { get; }

        int TotalCount { get; }
    }
}
