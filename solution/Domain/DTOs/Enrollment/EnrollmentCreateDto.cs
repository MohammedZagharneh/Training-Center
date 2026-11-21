using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.Enrollment
{
    public class EnrollmentCreateDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int StudentId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CourseId { get; set; }
    }
}
