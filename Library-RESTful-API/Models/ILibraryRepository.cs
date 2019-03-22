using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library_RESTful_API.Models
{
    public interface ILibraryRepository
    {
        bool AuthorExists(Guid authorId);
        Author GetAuthor(Guid id);
        IEnumerable<Author> GetAuthors();
        IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds);
        void AddAuthor(Author author);
        void UpdateBookForAuthor(Book book);
        void DeleteAuthor(Author author);

        Book GetBookForAuthor(Guid authorId, Guid bookId);
        IEnumerable<Book> GetBooksForAuthor(Guid authorId);
        void AddBookForAuthor(Guid authorId, Book book);
        void DeleteBook(Book book);


        bool Save();
    }
}
