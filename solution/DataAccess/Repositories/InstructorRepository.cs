using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities;
using Domain.DTOs.Instructor;
using Domain.Shared;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace DataAccess.Repositories
{
    public class InstructorRepository
    {
        private readonly TrainingCenterContext _context;

        public InstructorRepository(TrainingCenterContext context)
        {
            _context = context;
        }

        public async Task<InstructorDetailsDto?> GetDetailsAsync(int instructorId)
        {
            return await _context.Instructors.Where(i => i.InstructorId == instructorId).Select(i => new InstructorDetailsDto
            {
                InstructorId = i.InstructorId,
                Salary = i.Salary,
                IsActive = i.IsActive,
                ManagerId = i.ManagerId,

                PersonDetails = new PersonDto
                {
                    PersonId = i.Person.PersonId,
                    NationalNumber = i.Person.NationalNumber,
                    FirstName = i.Person.FirstName,
                    LastName = i.Person.LastName,
                    Email = i.Person.Email,
                    BirthDate = i.Person.BirthDate,
                    PhoneNumber = i.Person.PhoneNumber
                }
            }).FirstOrDefaultAsync();
        }

        public async Task<Instructor?> GetByIdAsync(int instructorId)
        {
            return await _context.Instructors.Include(i => i.Person).FirstOrDefaultAsync(i => i.InstructorId == instructorId);
        }

        public async Task<bool> IsInstructorExist(int instructorId)
        {
            return await _context.Instructors.AnyAsync(i => i.InstructorId == instructorId);
        }

        public async Task<PagedResult<InstructorListDto>> GetListAsync(int pageNumber, int pageSize)
        {
            var query = _context.Instructors.AsNoTracking();

            var totalCount = await query.CountAsync();

            var items = await query.OrderBy(s => s.PersonId).Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(i => new InstructorListDto
            {
                InstructorId = i.InstructorId,
                FirstName = i.Person.FirstName,
                LastName = i.Person.LastName,
                Salary = i.Salary,
                IsActive = i.IsActive,
                ManagerId = i.ManagerId

            }).ToListAsync();

            return new PagedResult<InstructorListDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<AddInstructorDataAccessResult> AddAsync(Instructor instructor)
        {
            _context.Instructors.Add(instructor);

            try
            {
                await _context.SaveChangesAsync();
                return AddInstructorDataAccessResult.Success;
            }
            catch(DbUpdateException ex)
            {
                if(ex.InnerException is SqlException sqlEx)
                {
                    if(sqlEx.Number == 2601 || sqlEx.Number == 2627)
                        return AddInstructorDataAccessResult.DuplicatePersonId;

                    if (sqlEx.Number == 547)
                        return AddInstructorDataAccessResult.FKsViolation;
                }

                throw;
            }
        }

        public async Task<bool> UpdateAsync(Instructor instructor)
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }

            catch(DbUpdateException ex) when (ex.GetBaseException() is SqlException sqlEx && sqlEx.Number is 547)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Instructor instructor)
        {
            _context.Instructors.Remove(instructor);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }

            catch (DbUpdateException ex) when (ex.GetBaseException() is SqlException sqlEx && sqlEx.Number is 547)//547 foreign key violation
            {
                return false;
            }
        }

        public async Task<Dictionary<int, int?>> GetManagementRelationshipsAsync()
        {
            return await _context.Instructors.Select(i => new { i.InstructorId, i.ManagerId }).ToDictionaryAsync(i => i.InstructorId, i => i.ManagerId);
        }
    }
}
