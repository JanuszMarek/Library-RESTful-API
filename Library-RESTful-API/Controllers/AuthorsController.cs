using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library_RESTful_API.Helpers;
using Library_RESTful_API.Models;
using Library_RESTful_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Library_RESTful_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : Controller
    {
        private ILibraryRepository _libraryRepository;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;

        public AuthorsController(ILibraryRepository libraryRepository, IUrlHelper urlHelper, IPropertyMappingService propertyMappingService)
        {
            _libraryRepository = libraryRepository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
        }

        private string CreateAuthorsResourceUrl(AuthorsResourceParameters authorsResourceParameters, ResourceUriType type)
        {
            switch(type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetAuthors",
                        new
                        {
                            orderBy = authorsResourceParameters.OrderBy,
                            searchQuery = authorsResourceParameters.SearchQuery,
                            genre = authorsResourceParameters.Genre,
                            pageNumber = authorsResourceParameters.PageNumber - 1,
                            pageSize = authorsResourceParameters.PageSize
                        });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetAuthors",
                        new
                        {
                            orderBy = authorsResourceParameters.OrderBy,
                            searchQuery = authorsResourceParameters.SearchQuery,
                            genre = authorsResourceParameters.Genre,
                            pageNumber = authorsResourceParameters.PageNumber + 1,
                            pageSize = authorsResourceParameters.PageSize
                        });
                default:
                    return _urlHelper.Link("GetAuthors",
                        new
                        {
                            orderBy = authorsResourceParameters.OrderBy,
                            searchQuery = authorsResourceParameters.SearchQuery,
                            genre = authorsResourceParameters.Genre,
                            pageNumber = authorsResourceParameters.PageNumber,
                            pageSize = authorsResourceParameters.PageSize
                        });
            }
        }

        [HttpGet(Name = nameof(GetAuthors))]
        public IActionResult GetAuthors([FromQuery] AuthorsResourceParameters authorsResourceParameters)
        {
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

            if(!_propertyMappingService.ValidMappingExistsFor<AuthorDto, Author>(authorsResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var repoAuthors = _libraryRepository.GetAuthors(authorsResourceParameters);

            var previousPageLink = repoAuthors.HasPrevious ? CreateAuthorsResourceUrl(authorsResourceParameters, ResourceUriType.PreviousPage) : null;
            var nextPageLink = repoAuthors.HasNext ? CreateAuthorsResourceUrl(authorsResourceParameters, ResourceUriType.NextPage) : null;

            var paginationMetaData = new PaginationMetaData(repoAuthors, previousPageLink, nextPageLink);


            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetaData));

            var authors = AutoMapper.Mapper.Map<IEnumerable<AuthorDto>>(repoAuthors);
            return new JsonResult(authors);
        }

        //do not working
        //[HttpGet("{id}", Name = "GetAuthor")]
        [HttpGet("{id}")]
        public IActionResult GetAuthor(Guid id)
        {
            var authorRepo = _libraryRepository.GetAuthor(id);

            if (authorRepo == null)
            {
                return NotFound();
            }


            var author = AutoMapper.Mapper.Map<AuthorDto>(authorRepo);

            return new JsonResult(author);
        }

        [HttpPost]
        public IActionResult CreateAuthor([FromBody] AuthorForCreateDto authorForCreateDto)
        {
            if(authorForCreateDto == null)
            {
                return BadRequest();
            }

            TryValidateModel(authorForCreateDto);

            if(!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var author = AutoMapper.Mapper.Map<Author>(authorForCreateDto);

            _libraryRepository.AddAuthor(author);

            if(!_libraryRepository.Save())
            {
                throw new Exception("Creating an author failed on server");
                //return StatusCode(500,"Unexpected problem occurs, try again later!")
            }

            var authorDto = AutoMapper.Mapper.Map<AuthorDto>(author);

            //do not working
            //return CreatedAtRoute("GetAuthor", new { id = authorDto.Id });


            //return new JsonResult(authorDto);
            return CreatedAtAction(nameof(GetAuthor), new { id = authorDto.Id }, author);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAutor(Guid id)
        {
            var authorFromRepo = _libraryRepository.GetAuthor(id);
            if(authorFromRepo == null)
            {
                return NotFound();
            }

            _libraryRepository.DeleteAuthor(authorFromRepo);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Deleting  auhtor {authorFromRepo.FirstName + " " + authorFromRepo.LastName} failed on server");
            }

            return NoContent();
        }
    }
}