using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities;
using Domain.Enums;
using Domain.Person;

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
        public async Task<PersonDetailsDto?> GetByIdAsync(int personId)
        {
            return await _repo.GetDetailsByIdAsync(personId);
        }

        public async Task<PersonDetailsDto?> GetByNationalNumberAsync(string nationalNumber)
        {
            return await _repo.GetDetailsByNationalNumberAsync(nationalNumber);
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
                return new AddPersonResult(AddPersonStatus.Success, NewPerson.PersonId);
            else
                return new AddPersonResult(AddPersonStatus.Duplicate, null);
        }

        //update
        public async Task<UpdatePersonResult> UpdateAsync(int PersonId, PersonUpdateDto person)
        {
            if (PersonId <= 0)
                return UpdatePersonResult.BadRequest;

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

            bool result =  await _repo.UpdateAsync(updatedPerson);

            return result ? UpdatePersonResult.Success:UpdatePersonResult.Duplicate;
        }

        public async Task<UpdatePersonResult> UpdateNationalNumberAsync(int PersonId, PersonUpdateNationalNumberDto person)
        {
            if (PersonId <= 0)
                return UpdatePersonResult.BadRequest;

            Person? updatedPerson = await _repo.GetByIdAsync(PersonId);

            if (updatedPerson == null)
            {
                return UpdatePersonResult.NotFound;
            }

            updatedPerson.NationalNumber = person.NationalNumber;

            bool result = await _repo.UpdateAsync(updatedPerson);

            return result ? UpdatePersonResult.Success : UpdatePersonResult.Duplicate;
        }


    }
}
