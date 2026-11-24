using Business.Services;
using Domain.DTOs.Instructor;
using Domain.Enums;
using Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers
{
    [Authorize]
    [Route("instructors")]
    [ApiController]
    public class InstructorController : ControllerBase
    {
        private readonly InstructorService _service;

        public InstructorController(InstructorService service)
        {
            _service = service;
        }

        [HttpGet("{instructorId:int}", Name = "GetInstructorById")]
        [ProducesResponseType(typeof(InstructorDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]

        public async Task<IActionResult> GetDetailsAsync(int instructorId)
        {
            GetInstructorDetailsResult result = await _service.GetDetailsAsync(instructorId);

            if (result.Status == GetDetailsStatus.InvalidInput)
                return BadRequest("Invalid Instructor ID!");

            if (result.Status == GetDetailsStatus.NotFound)
                return NotFound($"Instructor with ID = {instructorId} was not found!");

            return Ok(result.Instructor);
        }

        [HttpGet("all", Name = "GetInstructorsList")]
        [ProducesResponseType(typeof(PagedResult<InstructorListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetListAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            GetInstructorsListResult result = await _service.GetListAsync(pageNumber, pageSize);

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

        [HttpPost("add", Name = "AddInstructor")]
        [ProducesResponseType(typeof(InstructorDetailsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddAsync(InstructorCreateDto instructor)
        {
            AddInstructorResult result = await _service.AddAsync(instructor);

            if (result.Status == AddInstructorStatus.PersonOrManagerNotFound)
                return NotFound("the referenced person or manager doesn't exist");

            if (result.Status == AddInstructorStatus.AlreadyInstructor)
                return NotFound($"Person with ID = {instructor.PersonId} is already an instructor!");

            return CreatedAtRoute("GetInstructorById",
                    new { instructorId = result.Instructor!.InstructorId }, result.Instructor);
        }


        [HttpPost("add-with-new-person", Name = "AddInstructorWithNewPerson")]
        [ProducesResponseType(typeof(InstructorDetailsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddWithNewPersonAsync(InstructorCreateWithNewPersonDto instructor)
        {
            AddInstructorWithNewPersonResult result = await _service.AddWithNewPersonAsync(instructor);

            if (result.Status == AddInstructorWithNewPersonStatus.ManagerIdNotFound)
                return NotFound("referenced manager doesn't exist!");

            if (result.Status == AddInstructorWithNewPersonStatus.DuplicatePersonData)
                return Conflict("National number, email and phone number must be unqiue!");

            return CreatedAtRoute("GetInstructorById", new { instructorId = result.Instructor!.InstructorId }, result.Instructor);
        }


        [HttpDelete("delete/{instructorId:int}", Name = "DeleteInstructor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]

        public async Task<IActionResult> DeleteAsync(int instructorId)
        {
            DeleteResult result = await _service.DeleteAsync(instructorId);

            if (result == DeleteResult.InvalidInput)
                return BadRequest("Invalid Instructor ID!");

            if (result == DeleteResult.NotFound)
                return NotFound($"Instructor with ID = {instructorId} was not found!");

            if (result == DeleteResult.ForeignKeyViolation)
                return Conflict("You can't delete this Instructor because he has courses or managing other instructors!");

            return NoContent();
        }


        [HttpPut("update/{instructorId:int}", Name = "UpdateInstructor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]

        public async Task<IActionResult> UpdateAsync(int instructorId, InstructorUpdateDto instructor)
        {
            UpdateInstructorResult result = await _service.UpdateAsync(instructorId, instructor);

            if (result == UpdateInstructorResult.InvalidId)
                return BadRequest("Invalid Instructor ID!");

            if (result == UpdateInstructorResult.InstructorNotFound)
                return NotFound($"Instructor with ID = {instructorId} was not found!");

            if (result == UpdateInstructorResult.ManagerNotFound)
                return NotFound($"referenced manager doesn't exist!");

            if (result == UpdateInstructorResult.SelfManagement)
                return Conflict("Instructor can't manage itself!");

            if (result == UpdateInstructorResult.CircularManagement)
                return Conflict("Circular management is not allowed!");

            return NoContent();
        }
    }
}
