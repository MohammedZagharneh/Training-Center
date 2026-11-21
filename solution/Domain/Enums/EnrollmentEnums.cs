using Domain.DTOs.Enrollment;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum EnrollmentStatus
    {
        Active, Completed, Dropped
    }

    public enum GetStudentEnrollmentsStatus
    {
        InvalidStudentId, StudentNotFound, Success
    }

    public enum AddEnrollmentStatus
    {
        Success, CourseNotFound, StudentNotFound, InactiveInstructor, InactiveStudent, DuplicateStudentCoursePair
    }

    public enum UpdateEnrollmentStatusResult
    {
        Success, InvalidEnrollmentId ,EnrollmentNotFound, StatusInactive, InvalidStatus
    }

    public record GetEnrollmentDetailsResult(GetDetailsStatus Status, EnrollmentDetailsDto? Enrollment);


    public record GetEnrollmentsListResult(GetListStatus Status, PagedResult<EnrollmentDetailsDto>? PagedResult);


    public record GetStudentEnrollmentsResult(GetStudentEnrollmentsStatus Status, IEnumerable<StudentEnrollmentsDto>? Enrollments);

    public record AddEnrollmentResult(AddEnrollmentStatus Status, EnrollmentDetailsDto? Enrollment);

}
