using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library_RESTful_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library_RESTful_API.Controllers
{
    public class AuthorsController : Controller
    {
        private ILibraryRepository _libraryRepository;

        public AuthorsController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        public IActionResult GetAuthors()
        {
            return View();
        }
    }
}