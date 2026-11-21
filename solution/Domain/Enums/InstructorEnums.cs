using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTOs.Instructor;
using Domain.Shared;

namespace Domain.Enums
{
    public enum AddInstructorDataAccessResult
    {
        Success, FKsViolation, DuplicatePersonId
    }

    public record GetInstructorDetailsResult(GetDetailsStatus Status, InstructorDetailsDto? Instructor);

    public enum AddInstructorStatus
    {
        Success, PersonOrManagerNotFound, AlreadyInstructor
    }

    public record AddInstructorResult(AddInstructorStatus Status, InstructorDetailsDto? Instructor);

    public enum AddInstructorWithNewPersonStatus
    {
        Success, DuplicatePersonData, ManagerIdNotFound
    }
    
    public record AddInstructorWithNewPersonResult(AddInstructorWithNewPersonStatus Status, InstructorDetailsDto? Instructor);

    public enum UpdateInstructorResult
    {
        Success, InvalidId, InstructorNotFound, ManagerNotFound, SelfManagement, CircularManagement
    }

    public record GetInstructorsListResult(GetListStatus Status, PagedResult<InstructorListDto>? PagedResult);
}
