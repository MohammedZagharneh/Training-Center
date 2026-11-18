using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shared
{
    public class PersonDto
    {
        public int PersonId { get; set; }

        public string NationalNumber { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? Email { get; set; }

        public DateOnly? BirthDate { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
