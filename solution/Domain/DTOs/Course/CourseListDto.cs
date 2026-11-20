using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Course
{
    public class CourseListDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = null!;
        public string Code { get; set; } = null!;
        public decimal Price { get; set; }
        public int InstructorId { get; set; }
    }
}
