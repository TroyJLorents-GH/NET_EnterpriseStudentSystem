using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using StudentSystem.Domain.Entities;
using StudentSystem.Domain.Interfaces;

namespace StudentSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssignmentsController : ControllerBase
{
    private readonly IAssignmentRepository _repository;
    private readonly TelemetryClient _telemetryClient;
    private readonly ILogger<AssignmentsController> _logger;

    public AssignmentsController(
        IAssignmentRepository repository,
        TelemetryClient telemetryClient,
        ILogger<AssignmentsController> logger)
    {
        _repository = repository;
        _telemetryClient = telemetryClient;
        _logger = logger;
    }

    /// <summary>
    /// Get all student class assignments
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentClassAssignment>>> GetAllAssignments()
    {
        _logger.LogInformation("Fetching all assignments");
        _telemetryClient.TrackEvent("Assignments_GetAll");

        var assignments = await _repository.GetAllAsync();
        return Ok(assignments);
    }

    /// <summary>
    /// Get assignment by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<StudentClassAssignment>> GetAssignment(int id)
    {
        _logger.LogInformation("Fetching assignment {AssignmentId}", id);
        _telemetryClient.TrackEvent("Assignments_GetById", new Dictionary<string, string>
        {
            { "AssignmentId", id.ToString() }
        });

        var assignment = await _repository.GetAssignmentByIdAsync(id);
        if (assignment == null)
        {
            _logger.LogWarning("Assignment {AssignmentId} not found", id);
            return NotFound();
        }

        return Ok(assignment);
    }

    /// <summary>
    /// Get assignments by student ID
    /// </summary>
    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<IEnumerable<StudentClassAssignment>>> GetByStudentId(int studentId)
    {
        _logger.LogInformation("Fetching assignments for student {StudentId}", studentId);
        _telemetryClient.TrackEvent("Assignments_GetByStudent");

        var assignments = await _repository.GetByStudentIdAsync(studentId);
        return Ok(assignments);
    }

    /// <summary>
    /// Get assignments by term
    /// </summary>
    [HttpGet("term/{term}")]
    public async Task<ActionResult<IEnumerable<StudentClassAssignment>>> GetByTerm(string term)
    {
        _logger.LogInformation("Fetching assignments for term {Term}", term);
        _telemetryClient.TrackEvent("Assignments_GetByTerm");

        var assignments = await _repository.GetByTermAsync(term);
        return Ok(assignments);
    }

    /// <summary>
    /// Get assignments by class number
    /// </summary>
    [HttpGet("class/{classNum}")]
    public async Task<ActionResult<IEnumerable<StudentClassAssignment>>> GetByClassNum(string classNum)
    {
        _logger.LogInformation("Fetching assignments for class {ClassNum}", classNum);
        _telemetryClient.TrackEvent("Assignments_GetByClass");

        var assignments = await _repository.GetByClassNumAsync(classNum);
        return Ok(assignments);
    }

    /// <summary>
    /// Create a new assignment
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<StudentClassAssignment>> CreateAssignment(StudentClassAssignment assignment)
    {
        _logger.LogInformation("Creating new assignment for student {StudentId}", assignment.Student_ID);
        _telemetryClient.TrackEvent("Assignments_Create");

        var created = await _repository.AddAsync(assignment);
        return CreatedAtAction(nameof(GetAssignment), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update an existing assignment
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAssignment(int id, StudentClassAssignment assignment)
    {
        if (id != assignment.Id)
        {
            return BadRequest();
        }

        _logger.LogInformation("Updating assignment {AssignmentId}", id);
        _telemetryClient.TrackEvent("Assignments_Update");

        await _repository.UpdateAsync(assignment);
        return NoContent();
    }

    /// <summary>
    /// Delete an assignment
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAssignment(int id)
    {
        _logger.LogInformation("Deleting assignment {AssignmentId}", id);
        _telemetryClient.TrackEvent("Assignments_Delete");

        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
