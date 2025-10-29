using Microsoft.AspNetCore.Mvc;
using Microsoft.ApplicationInsights;
using StudentSystem.Domain.Entities;
using StudentSystem.Domain.Interfaces;

namespace StudentSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentRepository _studentRepository;
    private readonly TelemetryClient _telemetry;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(
        IStudentRepository studentRepository,
        TelemetryClient telemetry,
        ILogger<StudentsController> logger)
    {
        _studentRepository = studentRepository;
        _telemetry = telemetry;
        _logger = logger;
    }

    /// <summary>
    /// Get all students
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StudentLookup>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StudentLookup>>> GetAllStudents()
    {
        try
        {
            _telemetry.TrackEvent("GetAllStudents");
            _logger.LogInformation("Fetching all students");

            var students = await _studentRepository.GetAllAsync();

            _telemetry.TrackMetric("StudentCount", students.Count());
            return Ok(students);
        }
        catch (Exception ex)
        {
            _telemetry.TrackException(ex);
            _logger.LogError(ex, "Error fetching all students");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get student by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(StudentLookup), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentLookup>> GetStudent(int id)
    {
        try
        {
            _telemetry.TrackEvent("GetStudentById", new Dictionary<string, string>
            {
                { "StudentId", id.ToString() }
            });

            var student = await _studentRepository.GetByStudentIdAsync(id);

            if (student == null)
            {
                _logger.LogWarning("Student with ID {StudentId} not found", id);
                return NotFound();
            }

            return Ok(student);
        }
        catch (Exception ex)
        {
            _telemetry.TrackException(ex);
            _logger.LogError(ex, "Error fetching student {StudentId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Search students by term
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<StudentLookup>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StudentLookup>>> SearchStudents([FromQuery] string term)
    {
        try
        {
            _telemetry.TrackEvent("SearchStudents", new Dictionary<string, string>
            {
                { "SearchTerm", term }
            });

            var students = await _studentRepository.SearchStudentsAsync(term);
            return Ok(students);
        }
        catch (Exception ex)
        {
            _telemetry.TrackException(ex);
            _logger.LogError(ex, "Error searching students with term: {Term}", term);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get students by department
    /// </summary>
    [HttpGet("department/{department}")]
    [ProducesResponseType(typeof(IEnumerable<StudentLookup>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StudentLookup>>> GetByDepartment(string department)
    {
        try
        {
            _telemetry.TrackEvent("GetStudentsByDepartment", new Dictionary<string, string>
            {
                { "Department", department }
            });

            var students = await _studentRepository.GetByDepartmentAsync(department);
            return Ok(students);
        }
        catch (Exception ex)
        {
            _telemetry.TrackException(ex);
            _logger.LogError(ex, "Error fetching students for department: {Department}", department);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update student
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentLookup student)
    {
        try
        {
            if (id != student.Student_ID)
            {
                return BadRequest("ID mismatch");
            }

            var existing = await _studentRepository.GetByStudentIdAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            _telemetry.TrackEvent("UpdateStudent", new Dictionary<string, string>
            {
                { "StudentId", id.ToString() }
            });

            await _studentRepository.UpdateAsync(student);
            return NoContent();
        }
        catch (Exception ex)
        {
            _telemetry.TrackException(ex);
            _logger.LogError(ex, "Error updating student {StudentId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
