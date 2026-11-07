using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class Student
{
    public int StudentId { get; set; }

    public string StudentStatus { get; set; } = null!;

    public int PersonId { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual Person Person { get; set; } = null!;
}
