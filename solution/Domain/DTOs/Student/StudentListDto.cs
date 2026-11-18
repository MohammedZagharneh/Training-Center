using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Student
{
    public class StudentListDto
    {
        public int StudentId { get; set; }

        public string StudentStatus { get; set; } = null!;

        public int PersonId { get; set; }

        public string NationalNumber { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;
    }
}
