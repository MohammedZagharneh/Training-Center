using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities;
using Domain.DTOs.Person;
using Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Domain.Shared;

namespace DataAccess.Repositories
{
    public class PersonRepository
    {
        private readonly TrainingCenterContext _context;
        public PersonRepository(TrainingCenterContext context)
        {
            _context = context;
        }

        //fetch
        public async Task<PersonDetailsDto?> GetDetailsByIdAsync(int personId)
        {
            return await _context.People.Where(p => p.PersonId == personId).Select(p => new PersonDetailsDto
            {
                PersonId = p.PersonId,
                NationalNumber = p.NationalNumber,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                BirthDate = p.BirthDate,
                PhoneNumber = p.PhoneNumber,
                RegisteredAt = p.RegisteredAt,
                UserId = p.User != null ? p.User.UserId : null,
                InstructorId = p.Instructor != null ? p.Instructor.InstructorId : null,
                StudentId = p.Student != null ? p.Student.StudentId : null
            }).FirstOrDefaultAsync();
        }

        public async Task<PersonDetailsDto?> GetDetailsByNationalNumberAsync(string nationalNumber)
        {
            return await _context.People.Where(p => p.NationalNumber == nationalNumber).Select(p => new PersonDetailsDto
            {
                PersonId = p.PersonId,
                NationalNumber = p.NationalNumber,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                BirthDate = p.BirthDate,
                PhoneNumber = p.PhoneNumber,
                RegisteredAt = p.RegisteredAt,
                UserId = p.User != null ? p.User.UserId : null,
                InstructorId = p.Instructor != null ? p.Instructor.InstructorId : null,
                StudentId = p.Student != null ? p.Student.StudentId : null
            }).FirstOrDefaultAsync();
        }

        public async Task<PagedResult<PersonListDto>> GetListAsync(int pageNumber, int pageSize)
        {
            var query = _context.People.AsNoTracking();

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.PersonId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PersonListDto
                {
                    PersonId = p.PersonId,
                    NationalNumber = p.NationalNumber,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Email = p.Email,
                    BirthDate = p.BirthDate,
                    PhoneNumber = p.PhoneNumber,
                    RegisteredAt = p.RegisteredAt
                })
                .ToListAsync();

            return new PagedResult<PersonListDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }


        public async Task<bool> IsPersonExist(int personId)
        {
            return await _context.People.AnyAsync(p => p.PersonId == personId);
        }

        //add
        public async Task<bool> AddAsync(Person person)
        {
            /* 
            I am not benifiting from rows affected returned here because every possible scenario is handled 
            any internal db error is handled by middleware 
            any client violation of db constraints other than unique constraints is handled by asp.net using the attributes on DTOs and will return bad request
            so the only options left are 1- success 2- faliure due to duplicates (violation of unique indexes and constraints) which is handled here
            */

            _context.People.Add(person);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex) when (ex.GetBaseException() is SqlException SqlEx && SqlEx.Number is 2601 or 2627)
            {
                return false;
            }
        }

        //update
        public async Task<Person?> GetByIdAsync(int PersonId)
        {
            return await _context.People.FindAsync(PersonId);
        }

        public async Task<bool> UpdateAsync()
        {

            try
            { 
                await _context.SaveChangesAsync(); 
                return true;
            }

            catch (DbUpdateException ex) when (ex.GetBaseException() is SqlException SqlEx && SqlEx.Number is 2601 or 2627)
            {
                return false;
            }
        }
    }
}
