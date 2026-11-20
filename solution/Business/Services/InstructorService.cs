using DataAccess.Repositories;
using DataAccess.TransactionManager;
using Domain.DTOs.Instructor;
using Domain.Enums;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using DataAccess.Entities;

namespace Business.Services
{
    public class InstructorService
    {
        private readonly InstructorRepository _instructorRepo;
        private readonly PersonRepository _personRepo;
        private readonly EfTransactionManager _transactionManager;
        public InstructorService(InstructorRepository instructorRepo, PersonRepository personRepo, EfTransactionManager transactionManager)
        {
            _instructorRepo = instructorRepo;
            _personRepo = personRepo;
            _transactionManager = transactionManager;
        }

        public async Task<GetInstructorDetailsResult> GetDetailsAsync(int instructorId)
        {
            if (instructorId <= 0)
                return new GetInstructorDetailsResult(GetDetailsStatus.InvalidInput, null);

            InstructorDetailsDto? instructorDto = await _instructorRepo.GetDetailsAsync(instructorId);

            if (instructorDto == null)
                return new GetInstructorDetailsResult(GetDetailsStatus.NotFound, null);

            return new GetInstructorDetailsResult(GetDetailsStatus.Found, instructorDto);
        }

        public async Task<IEnumerable<InstructorListDto>> GetListAsync()
        {
            return await _instructorRepo.GetListAsnyc();
        }

        public async Task<AddInstructorResult> AddAsync(InstructorCreateDto instructor)
        {

            Instructor newInstructor = new Instructor
            {
                Salary = instructor.Salary,
                IsActive = true,
                PersonId = instructor.PersonId,
                ManagerId = instructor.ManagerId
            };


            AddInstructorDataAccessResult result = await _instructorRepo.AddAsync(newInstructor);

            if (result == AddInstructorDataAccessResult.FKsViolation)
                return new AddInstructorResult(AddInstructorStatus.PersonOrManagerNotFound, null);

            if (result == AddInstructorDataAccessResult.DuplicatePersonId)
                return new AddInstructorResult(AddInstructorStatus.AlreadyInstructor, null);

            InstructorDetailsDto? InstructorDetails = await _instructorRepo.GetDetailsAsync(newInstructor.InstructorId);

            return new AddInstructorResult(AddInstructorStatus.Success, InstructorDetails);
        }

        public async Task<AddInstructorWithNewPersonResult> AddWithNewPersonAsync(InstructorCreateWithNewPersonDto instructor)
        {
            await _transactionManager.BeginTransactionAsync();

            try
            {
                Person newPerson = new Person
                {
                    NationalNumber = instructor.Person.NationalNumber,
                    FirstName = instructor.Person.FirstName,
                    LastName = instructor.Person.LastName,
                    Email = instructor.Person.Email,
                    BirthDate = instructor.Person.BirthDate,
                    PhoneNumber = instructor.Person.PhoneNumber

                };

               bool isAdded = await _personRepo.AddAsync(newPerson);

                if (!isAdded)
                {
                    await _transactionManager.RollbackAsync();
                    return new AddInstructorWithNewPersonResult(AddInstructorWithNewPersonStatus.DuplicatePersonData, null); 
                }

                Instructor newInstructor = new Instructor
                {
                    Salary = instructor.Salary,
                    IsActive = true,
                    PersonId = newPerson.PersonId,
                    ManagerId = instructor.ManagerId
                };


                AddInstructorDataAccessResult result = await _instructorRepo.AddAsync(newInstructor);

                if (result == AddInstructorDataAccessResult.FKsViolation)
                {
                    await _transactionManager.RollbackAsync();
                    return new AddInstructorWithNewPersonResult(AddInstructorWithNewPersonStatus.ManagerIdNotFound, null); 
                }

                await _transactionManager.CommitAsync();

                InstructorDetailsDto newInstructorDetails = new InstructorDetailsDto
                {
                    InstructorId = newInstructor.InstructorId,
                    Salary = newInstructor.Salary,
                    IsActive = newInstructor.IsActive,
                    ManagerId = newInstructor.ManagerId,

                    PersonDetails = new PersonDto
                    {
                        PersonId = newPerson.PersonId,
                        NationalNumber = newPerson.NationalNumber,
                        FirstName = newPerson.FirstName,
                        LastName = newPerson.LastName,
                        Email = newPerson.Email,
                        BirthDate = newPerson.BirthDate,
                        PhoneNumber = newPerson.PhoneNumber
                    }
                };

                return new AddInstructorWithNewPersonResult(AddInstructorWithNewPersonStatus.Success, newInstructorDetails);
            }
            catch
            {
                await _transactionManager.RollbackAsync();
                throw;
            }

        }

        private async Task<bool> HasCircularManagementAsync(int instructorId, int? managerId)
        {
            if(managerId == null)
                return false;

            var relationships = await _instructorRepo.GetManagementRelationshipsAsync();

            int? currentManagerId = managerId;

            while(currentManagerId != null)
            {
                if(currentManagerId == instructorId)
                    return true;

                if (!relationships.TryGetValue(currentManagerId.Value, out var nextManagerId))
                    return false;

                currentManagerId = nextManagerId;
            }

            return false;
        }

        public async Task<UpdateInstructorResult> UpdateAsync(int instructorId, InstructorUpdateDto instructor)
        {
            if(instructorId <= 0)
                return UpdateInstructorResult.InvalidId;

            if (instructor.ManagerId.HasValue && instructorId == instructor.ManagerId.Value)
                return UpdateInstructorResult.SelfManagement;

            Instructor? instructorToUpdate = await _instructorRepo.GetByIdAsync(instructorId);

            if (instructorToUpdate == null)
                return UpdateInstructorResult.InstructorNotFound;

            if(await HasCircularManagementAsync(instructorId, instructor.ManagerId))
                return UpdateInstructorResult.CircularManagement;

            instructorToUpdate.Salary = instructor.Salary;
            instructorToUpdate.IsActive = instructor.IsActive;
            instructorToUpdate.ManagerId = instructor.ManagerId;

            bool result = await _instructorRepo.UpdateAsync(instructorToUpdate);

            if (!result)
                return UpdateInstructorResult.ManagerNotFound;

            return UpdateInstructorResult.Success;
        }

        public async Task<DeleteResult> DeleteAsync(int instructorId)
        {
            if(instructorId <= 0)
                return DeleteResult.InvalidInput;

            Instructor? instructorToDelete = await _instructorRepo.GetByIdAsync(instructorId);

            if(instructorToDelete == null) 
                return DeleteResult.NotFound;

            bool result = await _instructorRepo.DeleteAsync(instructorToDelete);

            if (!result)
                return DeleteResult.ForeignKeyViolation;

            return DeleteResult.Deleted;
        }
    }
}
