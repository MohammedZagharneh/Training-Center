using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities;
using Domain.Enums;
using Domain.Shared;
using Domain.DTOs.Person;

namespace Business.Services
{
    public class PersonService
    {
        private readonly PersonRepository _repo;

        public PersonService(PersonRepository repository)
        {
            _repo = repository;
        }

        //fetch
        public async Task<GetPersonDetailsResult> GetDetailsByIdAsync(int personId)
        {
            if (personId <= 0)
                return new GetPersonDetailsResult(GetDetailsStatus.InvalidInput, null);


            PersonDetailsDto? person = await _repo.GetDetailsByIdAsync(personId);

            if(person == null)
                return new GetPersonDetailsResult(GetDetailsStatus.NotFound, null);

            return new GetPersonDetailsResult(GetDetailsStatus.Found, person);
        }

        public async Task<GetPersonDetailsResult> GetDetailsByNationalNumberAsync(string nationalNumber)
        {
            if (string.IsNullOrWhiteSpace(nationalNumber) || !nationalNumber.All(char.IsLetterOrDigit))
                return new GetPersonDetailsResult(GetDetailsStatus.InvalidInput, null);


            PersonDetailsDto? person = await _repo.GetDetailsByNationalNumberAsync(nationalNumber);

            if (person == null)
                return new GetPersonDetailsResult(GetDetailsStatus.NotFound, null);

            return new GetPersonDetailsResult(GetDetailsStatus.Found, person);
        }

        public async Task<IEnumerable<PersonListDto>> GetListAsync()
        {
            return await _repo.GetListAsync();
        }


        //add
        public async Task<AddPersonResult> AddAsync(PersonCreateDto person)
        {
            Person NewPerson = new Person { NationalNumber = person.NationalNumber, FirstName = person.FirstName, 
                LastName = person.LastName, Email = person.Email, BirthDate = person.BirthDate, PhoneNumber = person.PhoneNumber};

            bool result = await _repo.AddAsync(NewPerson);


            if (result)
            {
                PersonDetailsDto newPersonDetails = new PersonDetailsDto
                {
                    PersonId = NewPerson.PersonId,
                    NationalNumber = NewPerson.NationalNumber,
                    FirstName = NewPerson.FirstName,
                    LastName = NewPerson.LastName,
                    Email = NewPerson.Email,
                    BirthDate = NewPerson.BirthDate,
                    PhoneNumber = NewPerson.PhoneNumber,
                    RegisteredAt = NewPerson.RegisteredAt,
                    UserId = null,
                    InstructorId = null,
                    StudentId = null
                };

                return new AddPersonResult(AddPersonStatus.Success, newPersonDetails);
            }
            else
                return new AddPersonResult(AddPersonStatus.Duplicate, null);
        }

        //update
        public async Task<UpdatePersonResult> UpdateAsync(int PersonId, PersonUpdateDto person)
        {
            if (PersonId <= 0)
                return UpdatePersonResult.InvalidInput;

            Person? updatedPerson = await _repo.GetByIdAsync(PersonId);

            if (updatedPerson == null)
            {
                return UpdatePersonResult.NotFound;
            }

            updatedPerson.FirstName = person.FirstName;
            updatedPerson.LastName = person.LastName;
            updatedPerson.Email = person.Email;
            updatedPerson.BirthDate = person.BirthDate;
            updatedPerson.PhoneNumber = person.PhoneNumber;

            bool result =  await _repo.UpdateAsync();

            return result ? UpdatePersonResult.Success:UpdatePersonResult.Duplicate;
        }

        public async Task<UpdatePersonResult> UpdateNationalNumberAsync(int PersonId, PersonUpdateNationalNumberDto person)
        {
            if (PersonId <= 0)
                return UpdatePersonResult.InvalidInput;

            Person? updatedPerson = await _repo.GetByIdAsync(PersonId);

            if (updatedPerson == null)
            {
                return UpdatePersonResult.NotFound;
            }

            updatedPerson.NationalNumber = person.NationalNumber;

            bool result = await _repo.UpdateAsync();

            return result ? UpdatePersonResult.Success : UpdatePersonResult.Duplicate;
        }


    }
}
