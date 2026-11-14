using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum AddPersonStatus
    {
        Success, Duplicate
    }

    public record AddPersonResult(AddPersonStatus Status, int? PersonId);

    public enum UpdatePersonResult
    {
        Success, Duplicate, NotFound, BadRequest
    }
}
