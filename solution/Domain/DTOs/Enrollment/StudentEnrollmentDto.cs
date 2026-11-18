using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Enrollment
{
    public class StudentEnrollmentDto
    {
        public int EnrollmentId { get; set; }

        public int CourseId { get; set; }

        public string CourseTitle { get; set; } = null!;

        public string CourseCode { get; set; } = null!;

        public DateOnly EnrollmentDate { get; set; }

        public DateOnly? CompletionDate { get; set; }

        public string EnrollmentStatus { get; set; } = null!;
    }
}
