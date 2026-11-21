using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Enrollment
{
    public class EnrollmentDetailsDto
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public DateOnly EnrollmentDate { get; set; }
        public DateOnly? CompletionDate { get; set; }
        public string EnrollmentStatus { get; set; } = string.Empty;
    }
}
