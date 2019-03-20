using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library_RESTful_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library_RESTful_API.Controllers
{
    [Route("api/[controller]")]
    public class AuthorCollectionsController : Controller
    {
        private ILibraryRepository _libraryRepository;

        public AuthorCollectionsController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [HttpPost]
        public IActionResult CreateAuthorCollection([FromBody] IEnumerable<Author> authorCollection)
        {
            if(authorCollection == null)
            {
                return BadRequest();
            }

            foreach(var author in authorCollection)
            {
                _libraryRepository.AddAuthor(author);
            }

            if (!_libraryRepository.Save())
            {
                throw new Exception("Creating an author collection failed on server");
                //return StatusCode(500,"Unexpected problem occurs, try again later!")
            }

            return Ok();
        }
    }
}