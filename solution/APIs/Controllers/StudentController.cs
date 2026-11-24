using Business.Services;
using Domain.DTOs.Person;
using Domain.DTOs.Student;
using Domain.Enums;
using Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers
{
    [Authorize]
    [Route("students")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly StudentService _service;

        public StudentController(StudentService service)
        {
            _service = service;
        }

        [HttpGet("{studentId:int}", Name = "GetStudentById")]
        [ProducesResponseType(typeof(StudentDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]

        public async Task<IActionResult> GetDetailsAsync(int studentId)
        {
            GetStudentDetailsResult result = await _service.GetDetailsAsync(studentId);

            if (result.Status == GetDetailsStatus.InvalidInput)
                return BadRequest("Invalid Student ID!");

            else if(result.Status == GetDetailsStatus.NotFound)
                return NotFound($"Student with ID = {studentId} was not found!");

            return Ok(result.Student);
        }

        [HttpGet("all", Name = "GetStudentsList")]
        [ProducesResponseType(typeof(PagedResult<StudentListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetListAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            GetStudentsListResult result = await _service.GetListAsync(pageNumber, pageSize);

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


        [HttpPost("add", Name = "AddStudent")]
        [ProducesResponseType(typeof(StudentDetailsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]

        public async Task<IActionResult> AddAsync(StudentCreateDto student)
        {
            AddStudentResult result = await _service.AddAsync(student);

            if (result.Status == AddStudentStatus.PersonNotFound)
                return NotFound($"Person with ID = {student.PersonId} was not found!");

            if (result.Status == AddStudentStatus.AlreadyStudent)
                return Conflict($"Person with ID = {student.PersonId} is already a student!");

            return CreatedAtRoute("GetStudentById",
                    new { studentId = result.Student!.StudentId }, result.Student);
        }


        [HttpPost("add-with-new-person", Name = "AddStudentWithNewPerson")]
        [ProducesResponseType(typeof(StudentDetailsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]

        public async Task<IActionResult> AddWithNewPersonAsync(PersonCreateDto person)
        {
            AddStudentWithNewPersonResult result = await _service.AddWithNewPersonAsync(person);

            if(result.Status == AddStudentWithNewPersonStatus.DuplicatePersonData)
                return Conflict("National number, email and phone number must be unqiue!");

            else
                return CreatedAtRoute("GetStudentById",
                    new { studentId = result.Student!.StudentId }, result.Student);
        }


        [HttpDelete("delete/{studentId:int}", Name = "DeleteStudent")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]

        public async Task<IActionResult> DeleteAsync(int studentId)
        {
            DeleteResult result = await _service.DeleteAsync(studentId);

            if (result == DeleteResult.InvalidInput)
                return BadRequest("Invalid Student ID!");

            if (result == DeleteResult.NotFound)
                return NotFound($"Student with ID = {studentId} was not found!");

            if (result == DeleteResult.ForeignKeyViolation)
                return Conflict("You can't delete this student because he has enrollments!");

            return NoContent();
        }


        [HttpPatch("update-status/{studentId:int}", Name = "UpdateStudentStatus")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]

        public async Task<IActionResult> UpdateStatusAsync(int studentId, StudentUpdateStatusDto updateStatus)
        {
            UpdateStudentResult result = await _service.UpdateStatusAsync(studentId, updateStatus);

            if(result == UpdateStudentResult.InvalidInput)
                return BadRequest("Invalid Student ID!");

            if(result == UpdateStudentResult.NotFound)
                return NotFound($"Student with ID = {studentId} was not found!");

            return NoContent();
        }
    }
}
