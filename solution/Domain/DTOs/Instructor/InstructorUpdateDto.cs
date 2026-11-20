using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Instructor
{
    public class InstructorUpdateDto
    {
        [Required]
        [Range(typeof(decimal), "0", "99999999.99")]
        public decimal Salary { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public int? ManagerId { get; set; }
    }
}
