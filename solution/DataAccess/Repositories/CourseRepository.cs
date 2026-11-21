using DataAccess.Entities;
using Domain.DTOs.Course;
using Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Shared;

namespace DataAccess.Repositories
{
    public class CourseRepository
    {
        private readonly TrainingCenterContext _context;

        public CourseRepository(TrainingCenterContext context)
        {
            _context = context;
        }

        public async Task<CourseDetailsDto?> GetDetailsAsync(int courseId)
        {
            return await _context.Courses
                .Where(c => c.CourseId == courseId)
                .Select(c => new CourseDetailsDto
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    Code = c.Code,
                    Price = c.Price,
                    Description = c.Description,
                    InstructorId = c.InstructorId,
                    InstructorFirstName = c.Instructor.Person.FirstName,
                    InstructorLastName = c.Instructor.Person.LastName
                })
                .FirstOrDefaultAsync();
        }

        public async Task<PagedResult<CourseListDto>> GetListAsync(int pageNumber, int pageSize)
        {
            var query = _context.Courses.AsNoTracking();

            int totalCount = await query.CountAsync();

            var items = await query.OrderBy(c => c.CourseId).Skip((pageNumber - 1) * pageSize).Take(pageSize)
                .Select(c => new CourseListDto
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    Code = c.Code,
                    Price = c.Price,
                    InstructorId = c.InstructorId
                })
                .ToListAsync();

            return new PagedResult<CourseListDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<IEnumerable<CourseListDto>> GetInstructorCoursesAsync(int instructorId)
        {
            return await _context.Courses.Where(c => c.InstructorId == instructorId).OrderBy(c => c.CourseId)
                .Select(c => new CourseListDto
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    Code = c.Code,
                    Price = c.Price,
                    InstructorId = c.InstructorId

                }).ToListAsync();
        }

        public async Task<Course?> GetByIdAsync(int courseId)
        {
            return await _context.Courses.FindAsync(courseId);
        }

        public async Task<Course?> GetByIdWithInstructorAsync(int courseId)
        {
            return await _context.Courses.Include(c => c.Instructor).FirstOrDefaultAsync(c => c.CourseId == courseId);
        }

        public async Task<bool> AddAsync(Course course)
        {
            _context.Courses.Add(course);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
                {
                    return false;
                }

                throw;
            }
        }

        public async Task<bool> UpdateAsync()
        {
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

        public async Task<bool> DeleteAsync(Course course)
        {
            _context.Courses.Remove(course);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
            {
                return false;
            }
        }

    }
}
