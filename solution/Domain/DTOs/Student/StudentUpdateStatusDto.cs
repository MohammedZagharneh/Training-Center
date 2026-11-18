using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.DTOs.Student
{
    public class StudentUpdateStatusDto
    {
        [Required]
        [EnumDataType(typeof(StudentStatus))]
        public StudentStatus StudentStatus { get; set; }
    }
}
