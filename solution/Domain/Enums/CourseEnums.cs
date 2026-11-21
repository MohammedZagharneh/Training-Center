using Domain.DTOs.Course;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public record GetCourseDetailsResult(GetDetailsStatus Status, CourseDetailsDto? Course);
    
    public enum GetInstructorCoursesStatus
    {
        Success, InstructorNotFound, InvalidInstructorId
    }

    public record GetInstructorCoursesResult(GetInstructorCoursesStatus Status, IEnumerable<CourseListDto>? Courses);


    public enum AddCourseStatus
    {
        Success, DuplicateCode, InstructorNotFound, InstructorInactive
    }

    public record AddCourseResult(AddCourseStatus Status, CourseDetailsDto? Course);

    public enum UpdateCourseResult
    {
        Success, InvalidCourseId, CourseNotFound,DuplicateCode, InstructorNotFound, InstructorInactive
    }

    public record GetCoursesListResult(GetListStatus Status, PagedResult<CourseListDto>? PagedResult);
}
