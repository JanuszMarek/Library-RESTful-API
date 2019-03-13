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

        public IEnumerable<Author> GetAuthors()
        {
            return _context.Authors.OrderBy(a => a.LastName).ThenBy(a => a.FirstName);
        }

        public Author GetAuthor(Guid id)
        {
            return _context.Authors.FirstOrDefault(a => a.Id == id);
        }

    }
}

