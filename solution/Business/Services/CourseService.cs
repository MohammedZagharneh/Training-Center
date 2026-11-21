using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Repositories;
using DataAccess.Entities;
using Domain.DTOs.Course;
using Domain.Shared;
using Domain.Enums;

namespace Business.Services
{
    public class CourseService
    {
        private readonly CourseRepository _courseRepo;
        private readonly InstructorRepository _instructorRepo;


        public CourseService(CourseRepository courseRepo, InstructorRepository instructorRepo)
        {
            _courseRepo = courseRepo;
            _instructorRepo = instructorRepo;
        }

        public async Task<GetCourseDetailsResult> GetDetailsAsync(int courseId)
        {
            if (courseId <= 0)
                return new GetCourseDetailsResult(GetDetailsStatus.InvalidInput, null);

            CourseDetailsDto? course = await _courseRepo.GetDetailsAsync(courseId);

            if (course == null)
                return new GetCourseDetailsResult(GetDetailsStatus.NotFound, null);

            return new GetCourseDetailsResult(GetDetailsStatus.Found, course);
        }

        public async Task<GetCoursesListResult> GetListAsync(int pageNumber, int pageSize)
        {
            if(pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return new GetCoursesListResult(GetListStatus.InvalidPaginationInputs, null);

            PagedResult<CourseListDto> result = await _courseRepo.GetListAsync(pageNumber, pageSize);

            return new GetCoursesListResult(GetListStatus.Success, result);
        }

        public async Task<GetInstructorCoursesResult> GetInstructorCoursesAsync(int instructorId)
        {
            if(instructorId <= 0)
                return new GetInstructorCoursesResult(GetInstructorCoursesStatus.InvalidInstructorId, null);

            if (! await _instructorRepo.IsInstructorExist(instructorId))
                return new GetInstructorCoursesResult(GetInstructorCoursesStatus.InstructorNotFound, null);

            IEnumerable<CourseListDto> list = await _courseRepo.GetInstructorCoursesAsync(instructorId);

            return new GetInstructorCoursesResult(GetInstructorCoursesStatus.Success, list);
        }

        public async Task<AddCourseResult> AddAsync(CourseCreateDto course)
        {
            Instructor? instructor = await _instructorRepo.GetByIdAsync(course.InstructorId);

            if (instructor == null)
                return new AddCourseResult(AddCourseStatus.InstructorNotFound, null);

            if(!instructor.IsActive)
               return new AddCourseResult(AddCourseStatus.InstructorInactive, null);

            Course newCourse = new Course
            {
                Title = course.Title,
                Code = course.Code,
                Price = course.Price,
                Description = course.Description,
                InstructorId = course.InstructorId
            };

            bool result = await _courseRepo.AddAsync(newCourse);

            if(!result)
                return new AddCourseResult(AddCourseStatus.DuplicateCode, null);

            CourseDetailsDto newCourseDetails = new CourseDetailsDto
            {
                CourseId = newCourse.CourseId,
                Title = newCourse.Title,
                Code = newCourse.Code,
                Price = newCourse.Price,
                Description = newCourse.Description,
                InstructorId = instructor.InstructorId,
                InstructorFirstName = instructor.Person.FirstName,
                InstructorLastName = instructor.Person.LastName
            };

            return new AddCourseResult(AddCourseStatus.Success, newCourseDetails);
        }

        public async Task<UpdateCourseResult> UpdateAsync(int courseId, CourseUpdateDto course)
        {
            if (courseId <= 0)
                return UpdateCourseResult.InvalidCourseId;

            Course? courseToUpdate = await _courseRepo.GetByIdAsync(courseId);

            if (courseToUpdate == null)
                return UpdateCourseResult.CourseNotFound;

            Instructor? instructor = await _instructorRepo.GetByIdAsync(course.InstructorId);

            if (instructor == null)
                return UpdateCourseResult.InstructorNotFound;

            if (!instructor.IsActive)
                return UpdateCourseResult.InstructorInactive;

            courseToUpdate.Title = course.Title;
            courseToUpdate.Code = course.Code;
            courseToUpdate.Price = course.Price;
            courseToUpdate.Description = course.Description;
            courseToUpdate.InstructorId = instructor.InstructorId;


            bool result = await _courseRepo.UpdateAsync();

            if(!result)
            {
                return UpdateCourseResult.DuplicateCode;
            }

            return UpdateCourseResult.Success;
        }

        public async Task<DeleteResult> DeleteAsync(int courseId)
        {
            if (courseId <= 0)
                return DeleteResult.InvalidInput;

            Course? courseToDelete = await _courseRepo.GetByIdAsync(courseId);

            if (courseToDelete == null)
                return DeleteResult.NotFound;

            bool result = await _courseRepo.DeleteAsync(courseToDelete);

            if (!result)
                return DeleteResult.ForeignKeyViolation;

            return DeleteResult.Deleted;
        }
    }
}
