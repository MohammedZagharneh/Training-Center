using Domain.DTOs.Person;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Instructor
{
    public class InstructorCreateWithNewPersonDto
    {
        [Required]
        [Range(typeof(decimal), "0", "99999999.99")]
        public decimal Salary { get; set; }

        [Range(1, int.MaxValue)]
        public int? ManagerId { get; set; }

        [Required]
        public PersonCreateDto Person { get; set; } = null!;
    }
}
