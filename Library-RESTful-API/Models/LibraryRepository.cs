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


        //AUTHORS
        public IEnumerable<Author> GetAuthors()
        {
            return _context.Authors.OrderBy(a => a.LastName).ThenBy(a => a.FirstName);
        }

        public Author GetAuthor(Guid authorId)
        {
            return _context.Authors.FirstOrDefault(a => a.Id == authorId);
        }

        public bool AuthorExists(Guid authorId)
        {
            return _context.Authors.Any(a => a.Id == authorId);
        }

        public void AddAuthor(Author author)
        {
            author.Id = Guid.NewGuid();
            _context.Authors.Add(author);

            if(author.Books.Any())
            {
                foreach (var book in author.Books)
                {
                    book.Id = Guid.NewGuid();
                }
            }
        }


        //BOOKS
        public Book GetBookForAuthor(Guid authorId, Guid bookId)
        {
            return _context.Books.Where(b => b.AuthorId == authorId && b.Id == bookId).FirstOrDefault();
        }

        public IEnumerable<Book> GetBooksForAuthor(Guid authorId)
        {
            return _context.Books.Where(b => b.AuthorId == authorId).OrderBy(b => b.Title).ToList();
        }


        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}

