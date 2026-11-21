
using Business.Services;
using Domain.DTOs.Course;
using Domain.Enums;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers
{
    [ApiController]
    [Route("courses")]
    public class CourseController : ControllerBase
    {
        private readonly CourseService _service;

        public CourseController(CourseService service)
        {
            _service = service;
        }

        [HttpGet("{courseId:int}", Name = "GetCourseByID")]
        [ProducesResponseType(typeof(CourseDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDetailsAsync(int courseId)
        {
            GetCourseDetailsResult result = await _service.GetDetailsAsync(courseId);

            switch (result.Status)
            {
                case GetDetailsStatus.InvalidInput:
                    return BadRequest("Invalid course ID!");

                case GetDetailsStatus.NotFound:
                    return NotFound($"Course with ID = {courseId} was not found!");

                case GetDetailsStatus.Found:
                    return Ok(result.Course);

                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpGet("all")]
        [ProducesResponseType(typeof(PagedResult<CourseListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetListAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            GetCoursesListResult result = await _service.GetListAsync(pageNumber, pageSize);

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



        [HttpGet("instructor/{instructorId:int}")]
        [ProducesResponseType(typeof(IEnumerable<CourseListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetInstructorCoursesAsync(int instructorId)
        {
            GetInstructorCoursesResult result = await _service.GetInstructorCoursesAsync(instructorId);

            switch (result.Status)
            {
                case GetInstructorCoursesStatus.InvalidInstructorId:
                    return BadRequest("Invalid instructor ID!");

                case GetInstructorCoursesStatus.InstructorNotFound:
                    return NotFound($"Instructor with ID = {instructorId} was not found!");

                case GetInstructorCoursesStatus.Success:
                    return Ok(result.Courses);

                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }



        [HttpPost("add")]
        [ProducesResponseType(typeof(CourseDetailsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddAsync(CourseCreateDto course)
        {
            AddCourseResult result = await _service.AddAsync(course);

            switch (result.Status)
            {
                case AddCourseStatus.InstructorNotFound:
                    return NotFound($"Instructor with ID = {course.InstructorId} was not found!");

                case AddCourseStatus.InstructorInactive:
                    return Conflict("Instructor is not active!");

                case AddCourseStatus.DuplicateCode:
                    return Conflict("Duplicate course code!");

                case AddCourseStatus.Success:
                    return CreatedAtRoute("GetCourseByID",new { courseId = result.Course!.CourseId },result.Course);

                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }



        [HttpPut("update/{courseId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateAsync(int courseId, CourseUpdateDto course)
        {
            UpdateCourseResult result = await _service.UpdateAsync(courseId, course);

            switch (result)
            {
                case UpdateCourseResult.InvalidCourseId:
                    return BadRequest("Invalid course ID!");

                case UpdateCourseResult.CourseNotFound:
                    return NotFound($"Course with ID = {courseId} was not found!");

                case UpdateCourseResult.InstructorNotFound:
                    return NotFound($"Instructor with ID = {course.InstructorId} was not found!");

                case UpdateCourseResult.InstructorInactive:
                    return Conflict("Instructor is not active!");

                case UpdateCourseResult.DuplicateCode:
                    return Conflict("Duplicate course code!");

                case UpdateCourseResult.Success:
                    return NoContent();

                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }



        [HttpDelete("delete/{courseId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteAsync(int courseId)
        {
            DeleteResult result = await _service.DeleteAsync(courseId);

            switch (result)
            {
                case DeleteResult.InvalidInput:
                    return BadRequest("Invalid course ID!");

                case DeleteResult.NotFound:
                    return NotFound($"Course with ID = {courseId} was not found!");

                case DeleteResult.ForeignKeyViolation:
                    return Conflict("you cannot delete this course because it has enrollments connected to it!");

                case DeleteResult.Deleted:
                    return NoContent();

                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
