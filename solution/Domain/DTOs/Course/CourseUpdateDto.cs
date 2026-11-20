using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.Course
{
    public class CourseUpdateDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Code { get; set; } = null!;

        [Required]
        [Range(typeof(decimal), "0", "99999999.99")]
        public decimal Price { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int InstructorId { get; set; }
    }
}
