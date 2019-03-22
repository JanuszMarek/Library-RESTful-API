using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library_RESTful_API.Helpers;
using Library_RESTful_API.Models;
using Microsoft.AspNetCore.Http;
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
            if (authorCollection == null)
            {
                return BadRequest();
            }

            foreach (var author in authorCollection)
            {
                _libraryRepository.AddAuthor(author);
            }

            if (!_libraryRepository.Save())
            {
                throw new Exception("Creating an author collection failed on server");
                //return StatusCode(500,"Unexpected problem occurs, try again later!")
            }

            var authorCollectionToReturn = AutoMapper.Mapper.Map<IEnumerable<AuthorDto>>(authorCollection);
            var idsAsString = string.Join(",", authorCollection.Select(a => a.Id));

            return CreatedAtAction(nameof(GetAuthorsCollection), new { ids = idsAsString }, authorCollectionToReturn);
            // return Ok();
        }

        [HttpGet("({ids})")]
        public IActionResult GetAuthorsCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }

            var authorEntities = _libraryRepository.GetAuthors(ids);

            if (ids.Count() != authorEntities.Count())
            {
                return NotFound();
            }

            var authorsToReturn = AutoMapper.Mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
            return Ok(authorsToReturn);
        }

        [HttpPost("{id}")]
        public IActionResult BlockAuthorCreation(Guid id)
        {
            if(_libraryRepository.AuthorExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }
    }
}