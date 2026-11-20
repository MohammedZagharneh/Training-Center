using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Shared;

namespace Domain.DTOs.Instructor
{
    public class InstructorDetailsDto
    {
        public int InstructorId { get; set; }
        public decimal Salary { get; set; }
        public bool IsActive { get; set; }
        public int? ManagerId { get; set; }
        public PersonDto PersonDetails { get; set; } = null!;
    }
}
