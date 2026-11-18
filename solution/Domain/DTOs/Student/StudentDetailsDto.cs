using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Shared;

namespace Domain.DTOs.Student
{
    public class StudentDetailsDto
    {
        public int StudentId { get; set; }

        public string StudentStatus { get; set; } = null!;

        public PersonDto PersonDetails { get; set; } = null!;
    }
}
