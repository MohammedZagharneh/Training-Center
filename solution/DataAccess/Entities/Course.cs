using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class Course
{
    public int CourseId { get; set; }

    public string Title { get; set; } = null!;

    public string Code { get; set; } = null!;

    public decimal Price { get; set; }

    public string? Description { get; set; }

    public int InstructorId { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual Instructor Instructor { get; set; } = null!;
}
