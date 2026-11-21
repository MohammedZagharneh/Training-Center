using Business.Services;
using Domain.DTOs.Enrollment;
using Domain.Enums;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers
{
    [ApiController]
    [Route("enrollments")]
    public class EnrollmentController : ControllerBase
    {
        private readonly EnrollmentService _service;

        public EnrollmentController(EnrollmentService service)
        {
            _service = service;
        }

        [HttpGet("{enrollmentId:int}", Name = "GetEnrollmentByID")]
        [ProducesResponseType(typeof(EnrollmentDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDetailsAsync(int enrollmentId)
        {
            GetEnrollmentDetailsResult result = await _service.GetDetailsAsync(enrollmentId);

            switch (result.Status)
            {
                case GetDetailsStatus.InvalidInput:
                    return BadRequest("Invalid enrollment ID!");

                case GetDetailsStatus.NotFound:
                    return NotFound($"Enrollment with ID = {enrollmentId} was not found!");

                case GetDetailsStatus.Found:
                    return Ok(result.Enrollment);

                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }



        [HttpGet("all")]
        [ProducesResponseType(typeof(PagedResult<EnrollmentDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetListAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            GetEnrollmentsListResult result = await _service.GetListAsync(pageNumber, pageSize);

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



        [HttpGet("student/{studentId:int}")]
        [ProducesResponseType(typeof(IEnumerable<StudentEnrollmentsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStudentEnrollmentsAsync(int studentId)
        {
            GetStudentEnrollmentsResult result = await _service.GetStudentEnrollmentsAsync(studentId);

            switch (result.Status)
            {
                case GetStudentEnrollmentsStatus.InvalidStudentId:
                    return BadRequest("Invalid student ID!");

                case GetStudentEnrollmentsStatus.StudentNotFound:
                    return NotFound($"Student with ID = {studentId} was not found!");

                case GetStudentEnrollmentsStatus.Success:
                    return Ok(result.Enrollments);

                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }



        [HttpPost("add")]
        [ProducesResponseType(typeof(EnrollmentDetailsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddAsync(EnrollmentCreateDto enrollment)
        {
            AddEnrollmentResult result = await _service.AddAsync(enrollment);

            switch (result.Status)
            {
                case AddEnrollmentStatus.CourseNotFound:
                    return NotFound($"Course with ID = {enrollment.CourseId} was not found!");

                case AddEnrollmentStatus.StudentNotFound:
                    return NotFound($"Student with ID = {enrollment.StudentId} was not found!");

                case AddEnrollmentStatus.InactiveInstructor:
                    return Conflict("The course instructor is not active!");

                case AddEnrollmentStatus.InactiveStudent:
                    return Conflict("Student is not active!");

                case AddEnrollmentStatus.DuplicateStudentCoursePair:
                    return Conflict("Student is already enrolled in this course!");

                case AddEnrollmentStatus.Success:
                    return CreatedAtRoute("GetEnrollmentByID",new { enrollmentId = result.Enrollment!.EnrollmentId },result.Enrollment);

                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }



        [HttpPatch("complete/{enrollmentId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CompleteAsync(int enrollmentId)
        {
            UpdateEnrollmentStatusResult result = await _service.UpdateStatusAsync(enrollmentId, EnrollmentStatus.Completed);

            switch (result)
            {
                case UpdateEnrollmentStatusResult.InvalidEnrollmentId:
                    return BadRequest("Invalid enrollment ID!");

                case UpdateEnrollmentStatusResult.InvalidStatus:
                    return BadRequest("Invalid enrollment status!");

                case UpdateEnrollmentStatusResult.EnrollmentNotFound:
                    return NotFound($"Enrollment with ID = {enrollmentId} was not found!");

                case UpdateEnrollmentStatusResult.StatusInactive:
                    return Conflict("Only active enrollments can be completed!");

                case UpdateEnrollmentStatusResult.Success:
                    return NoContent();

                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }



        [HttpPatch("drop/{enrollmentId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DropAsync(int enrollmentId)
        {
            UpdateEnrollmentStatusResult result = await _service.UpdateStatusAsync(enrollmentId, EnrollmentStatus.Dropped);

            switch (result)
            {
                case UpdateEnrollmentStatusResult.InvalidEnrollmentId:
                    return BadRequest("Invalid enrollment ID!");

                case UpdateEnrollmentStatusResult.InvalidStatus:
                    return BadRequest("Invalid enrollment status!");

                case UpdateEnrollmentStatusResult.EnrollmentNotFound:
                    return NotFound($"Enrollment with ID = {enrollmentId} was not found!");

                case UpdateEnrollmentStatusResult.StatusInactive:
                    return Conflict("Only active enrollments can be dropped!");

                case UpdateEnrollmentStatusResult.Success:
                    return NoContent();

                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
