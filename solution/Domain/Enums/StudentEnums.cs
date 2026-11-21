using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTOs.Student;

namespace Domain.Enums
{
    public enum StudentStatus
    {
        Active, Suspended, Graduated
    }

    public enum AddStudentStatus
    {
        Success, AlreadyStudent, PersonNotFound
    }

    public enum AddStudentWithNewPersonStatus
    {
        Success, DuplicatePersonData 
    }

    public enum UpdateStudentResult
    {
        Success, NotFound, InvalidInput
    }

    public record GetStudentDetailsResult(GetDetailsStatus Status, StudentDetailsDto? Student);

    public record AddStudentResult(AddStudentStatus Status, StudentDetailsDto? Student);

    public record AddStudentWithNewPersonResult(AddStudentWithNewPersonStatus Status, StudentDetailsDto? Student);
}
