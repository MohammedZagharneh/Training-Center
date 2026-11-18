using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Domain.DTOs.Student;

namespace DataAccess.Repositories
{
    public class StudentRepository
    {
        private readonly TrainingCenterContext _context;
        public StudentRepository(TrainingCenterContext context)
        {
            _context = context;
        }

        //Fetch
        public async Task<StudentDetailsDto?> GetDetailsAsync(int studentId)
        {
            return await _context.Students.Where(s => s.StudentId == studentId).Select(s => new StudentDetailsDto
            {
                StudentId = s.StudentId,
                StudentStatus = s.StudentStatus,

                PersonDetails = new PersonDto
                {
                    PersonId = s.Person.PersonId,
                    NationalNumber = s.Person.NationalNumber,
                    FirstName = s.Person.FirstName,
                    LastName = s.Person.LastName,
                    Email = s.Person.Email,
                    BirthDate = s.Person.BirthDate,
                    PhoneNumber = s.Person.PhoneNumber
                }
            }).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<StudentListDto>> GetListAsync()
        {
            return await _context.Students.Select(s => new StudentListDto
            {
                StudentId = s.StudentId,
                StudentStatus = s.StudentStatus,
                PersonId = s.Person.PersonId,
                NationalNumber = s.Person.NationalNumber,
                FirstName = s.Person.FirstName,
                LastName = s.Person.LastName,

            }).ToListAsync();
        }


        //Manipulate
        public async Task<bool> AddAsync(Student student)
        {
            /*
             1- person id is not found or person id is <= 0 is handled by business service
             2- person id is null is handled by asp using attributes 
             3- person id is already a student is handled by try catch            
             */

            _context.Students.Add(student);

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

        public async Task<Student?> GetByIdAsync(int StudentId)
        {
            return await _context.Students.FindAsync(StudentId);
        }

        public async Task<bool> DeleteAsync(Student student)
        {
            _context.Students.Remove(student);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }

            catch (DbUpdateException ex) when (ex.GetBaseException() is SqlException sqlEx && sqlEx.Number is 547)//547 foreign key violation
            {
                return false; // only if student is connected with courses in enrollments
            }
        }

        public async Task UpdateAsync(Student student)
        {
                await _context.SaveChangesAsync();
        }
    }
}
