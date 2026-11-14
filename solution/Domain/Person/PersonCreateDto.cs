using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Person
{
    public class PersonCreateDto
    {
        public string NationalNumber { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? Email { get; set; }

        public DateOnly? BirthDate { get; set; }

        public string? PhoneNumber { get; set; }

    }
}
