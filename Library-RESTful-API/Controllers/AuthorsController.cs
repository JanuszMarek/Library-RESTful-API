using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library_RESTful_API.Helpers;
using Library_RESTful_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library_RESTful_API.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
    public class AuthorsController : Controller
    {
        private ILibraryRepository _libraryRepository;

        public AuthorsController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [HttpGet()]
        public IActionResult GetAuthors()
        {
            var repoAuthors = _libraryRepository.GetAuthors();

            /* Slow and errorprone method
            var authors = new List<AuthorDto>();
            foreach(var auth in repoAuthors)
            {
                authors.Add(new AuthorDto()
                {
                    Id = auth.Id,
                    Name = $"{auth.FirstName} {auth.LastName}",
                    Genre = auth.Genre,
                    Age = auth.DateOfBirth.GetCurentAge(),
                });
            }
            */

            //Exception Handled in Startup
            /*
            try
            {
                throw new Exception("Test");

                var authors = AutoMapper.Mapper.Map<IEnumerable<AuthorDto>>(repoAuthors);
                return new JsonResult(authors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected fault happend. Please try again later!");
            }
            */
            

            var authors = AutoMapper.Mapper.Map<IEnumerable<AuthorDto>>(repoAuthors);
            return new JsonResult(authors);
        }

        [HttpGet("{id}")]
        public IActionResult GetAuthor(Guid id)
        {
            var authorRepo = _libraryRepository.GetAuthor(id);

            if(authorRepo == null)
            {
                return NotFound();
            }


            var author = AutoMapper.Mapper.Map<AuthorDto>(authorRepo);

            return new JsonResult(author);
        }
    }
}