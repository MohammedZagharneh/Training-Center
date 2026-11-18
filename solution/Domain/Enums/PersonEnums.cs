using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTOs.Person;
using Domain.Shared;

namespace Domain.Enums
{
    public enum AddPersonStatus
    {
        Success, Duplicate
    }
    public record GetPersonDetailsResult(GetDetailsStatus Status, PersonDetailsDto? Person);

    public record AddPersonResult(AddPersonStatus Status, PersonDetailsDto? Person);

    public enum UpdatePersonResult
    {
        Success, Duplicate, NotFound, InvalidInput
    }
}
