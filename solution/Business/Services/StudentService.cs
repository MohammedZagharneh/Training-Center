using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.TransactionManager;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Domain.DTOs.Student;
using Domain.Enums;
using Domain.Shared;
using DataAccess.Entities;
using Domain.DTOs.Person;

namespace Business.Services
{
    public class StudentService
    {
        private readonly StudentRepository _studentRepo;
        private readonly PersonRepository _personRepo;
        private readonly EfTransactionManager _transactionManager;

        public StudentService(StudentRepository studentRepo, PersonRepository personRepo, EfTransactionManager transactionManager)
        {
            _studentRepo = studentRepo;
            _personRepo = personRepo;
            _transactionManager = transactionManager;
        }

        public async Task<GetStudentDetailsResult> GetDetailsAsync(int studentId)
        {
            if (studentId <= 0)
                return new GetStudentDetailsResult(GetDetailsStatus.InvalidInput, null);

            StudentDetailsDto? student = await _studentRepo.GetDetailsAsync(studentId);

            if(student == null)
                return new GetStudentDetailsResult(GetDetailsStatus.NotFound, null);

            else
                return new GetStudentDetailsResult(GetDetailsStatus.Found, student);
        }

        public async Task<PagedResult<StudentListDto>> GetListAsync(int pageNumber, int pageSize)
        { 
            return await _studentRepo.GetListAsync(pageNumber, pageSize);
        }

        public async Task<AddStudentResult> AddAsync(StudentCreateDto student)
        {
            if(student.PersonId <= 0) // this is a mistake , this validation should have been by Range attribute in DTO not here (Fix Later)
                return new AddStudentResult(AddStudentStatus.InvalidPersonId, null);

            Person? person = await _personRepo.GetByIdAsync(student.PersonId);

            if(person == null)
                return new AddStudentResult(AddStudentStatus.PersonNotFound, null);

            Student newStudent = new Student { StudentStatus = "Active", PersonId = student.PersonId };

            bool result = await _studentRepo.AddAsync(newStudent);

            if(result)
            {
                StudentDetailsDto newStudentDetails = new StudentDetailsDto
                {
                    StudentId = newStudent.StudentId,
                    StudentStatus = newStudent.StudentStatus,

                    PersonDetails = new PersonDto
                    {
                        PersonId = person.PersonId,
                        NationalNumber = person.NationalNumber,
                        FirstName = person.FirstName,
                        LastName = person.LastName,
                        Email = person.Email,
                        BirthDate = person.BirthDate,
                        PhoneNumber = person.PhoneNumber
                    }
                };

                return new AddStudentResult(AddStudentStatus.Success, newStudentDetails);
            }
            else
                return new AddStudentResult(AddStudentStatus.AlreadyStudent, null);
        }

        public async Task<AddStudentWithNewPersonResult> AddWithNewPersonAsync(PersonCreateDto person)
        {
            await _transactionManager.BeginTransactionAsync();

            try
            {
                Person NewPerson = new Person
                {
                    NationalNumber = person.NationalNumber,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    Email = person.Email,
                    BirthDate = person.BirthDate,
                    PhoneNumber = person.PhoneNumber
                };

                bool isPersonAdded = await _personRepo.AddAsync(NewPerson);

                if (!isPersonAdded)
                {
                    await _transactionManager.RollbackAsync();
                    return new AddStudentWithNewPersonResult(AddStudentWithNewPersonStatus.DuplicatePersonData, null);
                }

                Student newStudent = new Student { StudentStatus = "Active", PersonId = NewPerson.PersonId };

                bool result = await _studentRepo.AddAsync(newStudent);

                if(!result)
                {
                    await _transactionManager.RollbackAsync();
                    throw new InvalidOperationException("Failed to add student after successfully adding person");
                }

                await _transactionManager.CommitAsync();

                StudentDetailsDto newStudentDetails = new StudentDetailsDto
                {
                    StudentId = newStudent.StudentId,
                    StudentStatus = newStudent.StudentStatus,

                    PersonDetails = new PersonDto
                    {
                        PersonId = NewPerson.PersonId,
                        NationalNumber = NewPerson.NationalNumber,
                        FirstName = NewPerson.FirstName,
                        LastName = NewPerson.LastName,
                        Email = NewPerson.Email,
                        BirthDate = NewPerson.BirthDate,
                        PhoneNumber = NewPerson.PhoneNumber
                    }
                };

                return new AddStudentWithNewPersonResult(AddStudentWithNewPersonStatus.Success, newStudentDetails);
            }
            catch
            {
                await _transactionManager.RollbackAsync();
                throw;
            }
        }

        public async Task<DeleteResult> DeleteAsync(int studentId)
        {
            if (studentId <= 0)
                return DeleteResult.InvalidInput;

            Student? student = await _studentRepo.GetByIdAsync(studentId);

            if (student == null)
                return DeleteResult.NotFound;

            bool result = await _studentRepo.DeleteAsync(student);

            if (result)
                return DeleteResult.Deleted;

            else
                return DeleteResult.ForeignKeyViolation;
        }

        public async Task<UpdateStudentResult> UpdateStatusAsync(int  studentId, StudentUpdateStatusDto studentStatus)
        {
            if(studentId <= 0)
                return UpdateStudentResult.InvalidInput;

            Student? student = await _studentRepo.GetByIdAsync(studentId);

            if(student == null)
                return UpdateStudentResult.NotFound;

            student.StudentStatus = studentStatus.StudentStatus.ToString();

            await _studentRepo.UpdateAsync(student);
            return UpdateStudentResult.Success;
        }
    }
}
