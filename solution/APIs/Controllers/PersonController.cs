using Business.Services;
using DataAccess.Entities;
using Domain.DTOs.Person;
using Domain.Enums;
using Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers
{
    [Authorize]
    [Route("people")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly PersonService _service;
        public PersonController(PersonService service) 
        {
            _service = service;
        }

        //fetch
        [HttpGet("{personId:int}", Name = "GetPersonById")]
        [ProducesResponseType(typeof(PersonDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDetailsByIdAsync(int personId)
        {
            GetPersonDetailsResult result = await _service.GetDetailsByIdAsync(personId);

            if (result.Status == GetDetailsStatus.InvalidInput)
                return BadRequest("Invalid ID!");

            else if(result.Status == GetDetailsStatus.NotFound)
                return NotFound($"Person with ID = {personId} was not found!");

            else 
                return Ok(result.Person);
        }

        [HttpGet("by-national-number/{nationalNumber}", Name = "GetPersonByNationalNumber")]
        [ProducesResponseType(typeof(PersonDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDetailsByNationalNumberAsync(string nationalNumber)
        {
            GetPersonDetailsResult result = await _service.GetDetailsByNationalNumberAsync(nationalNumber);

            if (result.Status == GetDetailsStatus.InvalidInput)
                return BadRequest("Invalid National Number!");

            else if (result.Status == GetDetailsStatus.NotFound)
                return NotFound($"Person with National Number = {nationalNumber} was not found!");

            else
                return Ok(result.Person);
        }


        [HttpGet("all", Name = "GetPeopleList")]
        [ProducesResponseType(typeof(PagedResult<PersonListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetListAsync([FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            GetPeopleListResult result = await _service.GetListAsync(pageNumber, pageSize);

            switch (result.Status)
            {
                case GetListStatus.InvalidPaginationInputs:
                    return BadRequest("Invalid pagination parameters!");

                case GetListStatus.Success:
                    return Ok(result.PagedResult);

                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        //add
        [HttpPost("add",Name = "AddPerson")]
        [ProducesResponseType(typeof(PersonDetailsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddAsync(PersonCreateDto person)
        {
            AddPersonResult result = await _service.AddAsync(person);

            if (result.Status == AddPersonStatus.Success)
                return CreatedAtRoute("GetPersonById", 
                    new { personId = result.Person!.PersonId }, result.Person);

            else
                return Conflict("National number, email and phone number must be unqiue!");
        }


        //update
        [HttpPut("update/{personId:int}", Name = "UpdatePerson")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateAsync([FromRoute]int personId, [FromBody]PersonUpdateDto person)
        {
            UpdatePersonResult result = await _service.UpdateAsync(personId, person);

            if (result == UpdatePersonResult.Duplicate)
                return Conflict("National number, email and phone number must be unqiue!");

            else if (result == UpdatePersonResult.NotFound)
                return NotFound($"Person with ID = {personId} was not found!");

            else if (result == UpdatePersonResult.InvalidInput)
                return BadRequest("Invalid ID!");

            else
                return NoContent();
        }

        [HttpPatch("update-national-number/{personId:int}", Name = "UpdatePersonNationalNumber")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateNationalNumberAsync([FromRoute] int personId, [FromBody] PersonUpdateNationalNumberDto person)
        {
            UpdatePersonResult result = await _service.UpdateNationalNumberAsync(personId, person);

            if (result == UpdatePersonResult.Duplicate)
                return Conflict("National number must be unqiue!");

            else if (result == UpdatePersonResult.NotFound)
                return NotFound($"Person with ID = {personId} was not found!");

            else if (result == UpdatePersonResult.InvalidInput)
                return BadRequest("Invalid ID!");

            else
                return NoContent();
        }
    }
}
