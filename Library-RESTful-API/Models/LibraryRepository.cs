using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library_RESTful_API.Models
{
    public class LibraryRepository : ILibraryRepository
    {
        private LibraryContext _context;

        public LibraryRepository(LibraryContext context)
        {
            _context = context;
        }

    }
}

