using DataAccess.Entities;
using Domain.DTOs.Enrollment;
using Domain.Enums;
using Domain.Shared;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccess.Repositories
{
    public class EnrollmentRepository
    {
        private readonly TrainingCenterContext _context;

        public EnrollmentRepository(TrainingCenterContext context)
        {
            _context = context;
        }


        public async Task<EnrollmentDetailsDto?> GetDetailsAsync(int enrollmentId)
        {
            return await _context.Enrollments.Where(e => e.EnrollmentId == enrollmentId).Select(e => new EnrollmentDetailsDto
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                StudentName = e.Student.Person.FirstName + " " + e.Student.Person.LastName,
                CourseId = e.CourseId,
                CourseTitle = e.Course.Title,
                EnrollmentDate = e.EnrollmentDate,
                CompletionDate = e.CompletionDate,
                EnrollmentStatus = e.EnrollmentStatus

            }).FirstOrDefaultAsync();
        }

        public async Task<PagedResult<EnrollmentDetailsDto>> GetListAsync(int pageNumber, int pageSize)
        {
            var query = _context.Enrollments.AsNoTracking();

            int totalCount = await query.CountAsync();

            var items = await query.OrderBy(e => e.EnrollmentId).Skip((pageNumber-1) * pageSize).Take(pageSize).Select(e => new EnrollmentDetailsDto
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                StudentName = e.Student.Person.FirstName + " " + e.Student.Person.LastName,
                CourseId = e.CourseId,
                CourseTitle = e.Course.Title,
                EnrollmentDate = e.EnrollmentDate,
                CompletionDate = e.CompletionDate,
                EnrollmentStatus = e.EnrollmentStatus

            }).ToListAsync();

            return new PagedResult<EnrollmentDetailsDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<IEnumerable<StudentEnrollmentsDto>> GetStudentEnrollmentsAsync(int studentId)
        {
            return await _context.Enrollments.Where(e => e.StudentId == studentId).Select(e => new StudentEnrollmentsDto 
            {
                EnrollmentId = e.EnrollmentId,
                CourseId = e.CourseId,
                CourseTitle = e.Course.Title,
                CourseCode = e.Course.Code,
                EnrollmentDate = e.EnrollmentDate,
                CompletionDate = e.CompletionDate,
                EnrollmentStatus = e.EnrollmentStatus

            }).ToListAsync();
        }

        public async Task<Enrollment?> GetByIdAsync(int enrollmentId)
        {
            return await _context.Enrollments.FindAsync(enrollmentId);
        }

        public async Task<bool> AddAsync(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);

            try
            { 
                await _context.SaveChangesAsync(); 
                return true;
            }

            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Number == 2601 || sqlEx.Number == 2627)
                        return false;
                }

                throw;
            }
        }

        public async Task UpdateStatusAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
