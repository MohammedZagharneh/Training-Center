using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Person;

namespace Domain.Enums
{

    public enum GetPersonDetailsStatus
    {
        Found, NotFound, BadRequest
    }

    public record GetPersonDetailsResult(GetPersonDetailsStatus Status, PersonDetailsDto? Person);
    public enum AddPersonStatus
    {
        Success, Duplicate
    }

    public record AddPersonResult(AddPersonStatus Status, PersonDetailsDto? Person);

    public enum UpdatePersonResult
    {
        Success, Duplicate, NotFound, BadRequest
    }
}
