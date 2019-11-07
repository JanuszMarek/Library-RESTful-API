using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library_RESTful_API.Models
{
    public class BookForUpdateDto : BookBaseDto
    {
        [Required(ErrorMessage = "Please fill the description")]
        public override string Description { get => base.Description; set => base.Description = value; }
    }
}
