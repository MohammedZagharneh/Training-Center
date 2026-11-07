using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class Person
{
    public int PersonId { get; set; }

    public string NationalNumber { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Email { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string? PhoneNumber { get; set; }

    public DateOnly RegisteredAt { get; set; }

    public virtual Instructor? Instructor { get; set; }

    public virtual Student? Student { get; set; }

    public virtual User? User { get; set; }
}
