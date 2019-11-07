using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library_RESTful_API.Models
{
    public abstract class BookBaseDto
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Max length of title is 100 characters")]
        public string Title { get; set; }

        [Required]
        [MaxLength(500)]
        public virtual string Description { get; set; }
    }
}
