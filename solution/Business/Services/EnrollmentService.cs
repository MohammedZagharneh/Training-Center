using DataAccess.Repositories;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Shared;
using Domain.DTOs.Enrollment;
using System.Reflection.Metadata.Ecma335;
using DataAccess.Entities;

namespace Business.Services
{
    public class EnrollmentService
    {
        private readonly EnrollmentRepository _enrollmentRepo;
        private readonly StudentRepository _studentRepo;
        private readonly CourseRepository _courseRepo;

        public EnrollmentService(EnrollmentRepository enrollmentRepo, StudentRepository studentRepo, CourseRepository courseRepo)
        {
            _enrollmentRepo = enrollmentRepo;
            _studentRepo = studentRepo;
            _courseRepo = courseRepo;
        }

        public async Task<GetEnrollmentDetailsResult> GetDetailsAsync(int enrollmentId)
        {
            if (enrollmentId <= 0)
                return new GetEnrollmentDetailsResult(GetDetailsStatus.InvalidInput, null);

            EnrollmentDetailsDto? enrollment = await _enrollmentRepo.GetDetailsAsync(enrollmentId);

            if(enrollment == null)
                return new GetEnrollmentDetailsResult(GetDetailsStatus.NotFound, null);

            return new GetEnrollmentDetailsResult(GetDetailsStatus.Found, enrollment);
        }

        public async Task<GetEnrollmentsListResult> GetListAsync(int pageNumber, int pageSize)
        {
            if(pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return new GetEnrollmentsListResult(GetListStatus.InvalidPaginationInputs, null);

            PagedResult<EnrollmentDetailsDto> result = await _enrollmentRepo.GetListAsync(pageNumber, pageSize);

            return new GetEnrollmentsListResult(GetListStatus.Success, result);
        }

        public async Task<GetStudentEnrollmentsResult> GetStudentEnrollmentsAsync(int studentId)
        {
            if (studentId <= 0)
                return new GetStudentEnrollmentsResult(GetStudentEnrollmentsStatus.InvalidStudentId, null);

            if(! await _studentRepo.IsStudentExists(studentId))
                return new GetStudentEnrollmentsResult(GetStudentEnrollmentsStatus.StudentNotFound, null);

            IEnumerable<StudentEnrollmentsDto> list = await _enrollmentRepo.GetStudentEnrollmentsAsync(studentId);

            return new GetStudentEnrollmentsResult(GetStudentEnrollmentsStatus.Success, list);
        }

        public async Task<AddEnrollmentResult> AddAsync(EnrollmentCreateDto enrollment)
        {
            Course? course = await _courseRepo.GetByIdWithInstructorAsync(enrollment.CourseId);

            if(course == null)
                return new AddEnrollmentResult(AddEnrollmentStatus.CourseNotFound, null);

            if(!course.Instructor.IsActive)
                return new AddEnrollmentResult(AddEnrollmentStatus.InactiveInstructor, null);

            Student? student = await _studentRepo.GetByIdWithPersonAsync(enrollment.StudentId);

            if (student == null)
                return new AddEnrollmentResult(AddEnrollmentStatus.StudentNotFound, null);

            if (student.StudentStatus != "Active")
                return new AddEnrollmentResult(AddEnrollmentStatus.InactiveStudent, null);

            Enrollment newEnrollment = new Enrollment
            {
                StudentId = student.StudentId,
                CourseId = course.CourseId,
                EnrollmentDate = DateOnly.FromDateTime(DateTime.Today),
                CompletionDate = null,
                EnrollmentStatus = "Active"
            };

            bool result = await _enrollmentRepo.AddAsync(newEnrollment);

            if(!result)
                return new AddEnrollmentResult(AddEnrollmentStatus.DuplicateStudentCoursePair, null);

            EnrollmentDetailsDto newEnrollmentDetails = new EnrollmentDetailsDto
            {
                EnrollmentId = newEnrollment.EnrollmentId,
                StudentId = student.StudentId,
                StudentName = student.Person.FirstName + " " + student.Person.LastName,
                CourseId = course.CourseId,
                CourseTitle = course.Title,
                EnrollmentDate = newEnrollment.EnrollmentDate,
                CompletionDate = newEnrollment.CompletionDate,
                EnrollmentStatus = newEnrollment.EnrollmentStatus
            };

            return new AddEnrollmentResult(AddEnrollmentStatus.Success, newEnrollmentDetails);
        }

        public async Task<UpdateEnrollmentStatusResult> UpdateStatusAsync(int enrollmentId, EnrollmentStatus newStatus)
        {
            if (enrollmentId <= 0)
                return UpdateEnrollmentStatusResult.InvalidEnrollmentId;

            if (newStatus != EnrollmentStatus.Completed && newStatus != EnrollmentStatus.Dropped)
                return UpdateEnrollmentStatusResult.InvalidStatus;

            Enrollment? enrollment = await _enrollmentRepo.GetByIdAsync(enrollmentId);

            if (enrollment == null)
                return UpdateEnrollmentStatusResult.EnrollmentNotFound;

            if (enrollment.EnrollmentStatus != "Active")
                return UpdateEnrollmentStatusResult.StatusInactive;

            enrollment.EnrollmentStatus = newStatus.ToString();

            if (newStatus == EnrollmentStatus.Completed)
                enrollment.CompletionDate = DateOnly.FromDateTime(DateTime.Today);

            await _enrollmentRepo.UpdateStatusAsync();
            return UpdateEnrollmentStatusResult.Success;
        }
    }
}
