using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library_RESTful_API.Models;
using Library_RESTful_API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Library_RESTful_API.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController : Controller
    {
        private ILibraryRepository _libraryRepository;
        private ILogger<BooksController> _logger;
        private IUrlHelper _urlHelper;

        public BooksController(ILibraryRepository libraryRepository, ILogger<BooksController> logger, IUrlHelper urlHelper)
        {
            _libraryRepository = libraryRepository;
            _logger = logger;
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = nameof(GetBooksForAutor))]
        public IActionResult GetBooksForAutor(Guid authorId)
        {
            if(!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var booksRepo = _libraryRepository.GetBooksForAuthor(authorId);

            var books = AutoMapper.Mapper.Map<IEnumerable<BookDto>>(booksRepo);

            books = books.Select(book =>
            {
                book = CreateLinksForBook(book);
                return book;
            });

            var booksWrapper = new LinkedCollectionResourceWrapperDto<BookDto>(books);
            booksWrapper = CreateLinksForBooks(booksWrapper);

            return Ok(booksWrapper);
        }

        [HttpGet("{bookId}", Name = nameof(GetBookForAutor))]
        public IActionResult GetBookForAutor(Guid authorId, Guid bookId)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookRepo = _libraryRepository.GetBookForAuthor(authorId, bookId);

            if(bookRepo == null)
            {
                return NotFound();
            }

            var book = AutoMapper.Mapper.Map<BookDto>(bookRepo);
            book = CreateLinksForBook(book);

            return Ok(book);
        }

        [HttpPost()]
        public IActionResult AddBookForAuthor(Guid authorId, [FromBody] BookForCreateDto bookforCreateDto)
        {
            if(bookforCreateDto == null)
            {
                return BadRequest();
            }

            if(bookforCreateDto.Description == bookforCreateDto.Title)
            {
                ModelState.AddModelError(nameof(BookForCreateDto), "Title is equal to Description");
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var book = AutoMapper.Mapper.Map<Book>(bookforCreateDto);

            _libraryRepository.AddBookForAuthor(authorId, book);

            if (!_libraryRepository.Save())
            {
                throw new Exception("Creating an author failed on server");
                //return StatusCode(500,"Unexpected problem occurs, try again later!")
            }

            var bookDto = AutoMapper.Mapper.Map<BookDto>(book);
            bookDto = CreateLinksForBook(bookDto);

            //return new JsonResult(bookDto);
            return CreatedAtAction(nameof(GetBookForAutor), new { authorId, bookId = book.Id}, book);
        }

        //FULLY UPDATE
        [HttpPut("{bookid}", Name = nameof(UpdateBookForAuthor))]
        public IActionResult UpdateBookForAuthor(Guid authorId, Guid bookid, [FromBody] BookForUpdateDto book)
        {
            if(book == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            //Upserting
            //if book not exist, create book
            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, bookid);
            if (bookForAuthorFromRepo == null) 
            {
                // return NotFound();
                var bookToAdd = AutoMapper.Mapper.Map<Book>(book);
                bookToAdd.Id = bookid;

                _libraryRepository.AddBookForAuthor(authorId, bookToAdd);
                if (!_libraryRepository.Save())
                {
                    throw new Exception($"Upserting book {bookForAuthorFromRepo.Title} for auhtor {bookForAuthorFromRepo.Author.FirstName + " " + bookForAuthorFromRepo.Author.LastName} failed on server");
                }

                var bookToReturn = AutoMapper.Mapper.Map<BookDto>(bookToAdd);
                bookToReturn = CreateLinksForBook(bookToReturn);

                return CreatedAtAction(nameof(GetBookForAutor), new { authorId, bookId = bookid }, bookToReturn);
            }


            AutoMapper.Mapper.Map(book, bookForAuthorFromRepo);
            _libraryRepository.UpdateBookForAuthor(bookForAuthorFromRepo);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Updating book {bookForAuthorFromRepo.Title} for author {bookForAuthorFromRepo.Author.FirstName + " " + bookForAuthorFromRepo.Author.LastName} failed on server");
            }

            return NoContent();
        }
        
        [HttpPatch("{id}", Name = nameof(PartiallyUpdateBookForAuthor))]
        public IActionResult PartiallyUpdateBookForAuthor(Guid authorId, Guid id,[FromBody] JsonPatchDocument<BookForUpdateDto> patchDoc)
        {
            if(patchDoc == null)
            {
                return BadRequest();
            }

            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);
            if (bookForAuthorFromRepo == null)
            {
                return NotFound();
            }

            var bookToPatch = AutoMapper.Mapper.Map<BookForUpdateDto>(bookForAuthorFromRepo);

            patchDoc.ApplyTo(bookToPatch, ModelState);

            AutoMapper.Mapper.Map(bookToPatch, bookForAuthorFromRepo);

            TryValidateModel(bookToPatch);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            _libraryRepository.UpdateBookForAuthor(bookForAuthorFromRepo);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Patching book {bookForAuthorFromRepo.Title} for auhtor {bookForAuthorFromRepo.Author.FirstName + " " + bookForAuthorFromRepo.Author.LastName} failed on server");
            }

            return NoContent();
        }


        [HttpDelete("{id}", Name = nameof(DeleteBookForAuthor))]
        public IActionResult DeleteBookForAuthor(Guid authorId, Guid id)
        {
            if(!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);
            if(bookForAuthorFromRepo == null)
            {
                return NotFound();
            }

            _libraryRepository.DeleteBook(bookForAuthorFromRepo);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Deleting book {bookForAuthorFromRepo.Title} for auhtor {bookForAuthorFromRepo.Author.FirstName + " " + bookForAuthorFromRepo.Author.LastName} failed on server");
            }

            return NoContent();
        }

        private BookDto CreateLinksForBook(BookDto book)
        {
            book.Links.Add(new LinkDto(
                _urlHelper.Link(nameof(GetBookForAutor), new { bookId = book.Id }),
                "self",
                "GET"));

            book.Links.Add(new LinkDto(
                _urlHelper.Link(nameof(DeleteBookForAuthor), new { id = book.Id }),
                "delete_book",
                "DELETE"));

            book.Links.Add(new LinkDto(
                _urlHelper.Link(nameof(UpdateBookForAuthor), new { bookid = book.Id }),
                "update_book",
                "PUT"));

            book.Links.Add(new LinkDto(
                _urlHelper.Link(nameof(PartiallyUpdateBookForAuthor), new { id = book.Id }),
                "partially_update_book",
                "PATCH" +
                ""));

            return book;
        }

        private LinkedCollectionResourceWrapperDto<BookDto> CreateLinksForBooks(LinkedCollectionResourceWrapperDto<BookDto> booksWrapper)
        {
            booksWrapper.Links.Add(new LinkDto(
                _urlHelper.Link(nameof(GetBooksForAutor), new {}),
                "self",
                "GET"));

            return booksWrapper;
        }
    }
}