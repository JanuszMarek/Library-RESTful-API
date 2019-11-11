using Library_RESTful_API.Helpers;
using Library_RESTful_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library_RESTful_API.Services
{
    public class LibraryRepository : ILibraryRepository
    {
        private LibraryContext _context;
        private IPropertyMappingService _propertyMappingService;

        public LibraryRepository(LibraryContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }
        
        //AUTHORS
        public Author GetAuthor(Guid authorId)
        {
            return _context.Authors.FirstOrDefault(a => a.Id == authorId);
        }

        public PagedList<Author> GetAuthors(AuthorsResourceParameters authorsResourceParameters)
        {
            //var authors = _context.Authors
            //    .OrderBy(a => a.LastName)
            //    .ThenBy(a => a.FirstName).AsQueryable();

            var authors = _context.Authors.ApplySort(authorsResourceParameters.OrderBy, _propertyMappingService.GetPropertyMapping<AuthorDto, Author>());

            if (!string.IsNullOrEmpty(authorsResourceParameters.Genre))
            {
                //trim and ignore casing
                var genre = authorsResourceParameters.Genre.Trim().ToLowerInvariant();
                authors = authors.Where(a => a.Genre.ToLowerInvariant() == genre);
            }

            if (!string.IsNullOrEmpty(authorsResourceParameters.SearchQuery))
            {
                //trim and ignore casing
                var search = authorsResourceParameters.SearchQuery.Trim().ToLowerInvariant();
                authors = authors.Where(a =>
                a.Genre.ToLowerInvariant().Contains(search) ||
                a.FirstName.ToLowerInvariant().Contains(search) ||
                a.LastName.ToLowerInvariant().Contains(search));
            }

            return PagedList<Author>.Create(authors, authorsResourceParameters.PageNumber, authorsResourceParameters.PageSize);
        }

        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds)
        {
            return _context.Authors.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName)
                .ToList();
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

        public void UpdateBookForAuthor(Book book)
        {
            // no code in this implementation
        }

        public void DeleteAuthor(Author author)
        {
            _context.Authors.Remove(author);
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

        public void AddBookForAuthor(Guid authorId, Book book)
        {
            var author = GetAuthor(authorId);
            if (author != null)
            {
                // if there isn't an id filled out (ie: we're not upserting),
                // we should generate one
                if (book.Id == Guid.Empty)
                {
                    book.Id = Guid.NewGuid();
                }
                author.Books.Add(book);
            }
        }

        public void DeleteBook(Book book)
        {
            _context.Books.Remove(book);
        }

        //GENERALS
        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}

