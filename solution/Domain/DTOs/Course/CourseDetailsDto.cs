using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Course
{
    public class CourseDetailsDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = null!;
        public string Code { get; set; } = null!;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public int InstructorId { get; set; }
        public string InstructorFirstName { get; set; } = null!;
        public string InstructorLastName { get; set; } = null!;
    }

}
