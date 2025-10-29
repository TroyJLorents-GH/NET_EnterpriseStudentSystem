using Microsoft.AspNetCore.Mvc;
using Microsoft.ApplicationInsights;
using StudentSystem.Domain.Entities;
using StudentSystem.Domain.Interfaces;

namespace StudentSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClassScheduleController : ControllerBase
{
    private readonly IClassScheduleRepository _repository;
    private readonly TelemetryClient _telemetry;
    private readonly ILogger<ClassScheduleController> _logger;

    public ClassScheduleController(
        IClassScheduleRepository repository,
        TelemetryClient telemetry,
        ILogger<ClassScheduleController> logger)
    {
        _repository = repository;
        _telemetry = telemetry;
        _logger = logger;
    }

    /// <summary>
    /// Get all class schedules
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClassSchedule2254>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClassSchedule2254>>> GetAllClasses()
    {
        try
        {
            _telemetry.TrackEvent("GetAllClasses");
            _logger.LogInformation("Fetching all class schedules");

            var classes = await _repository.GetAllAsync();

            _telemetry.TrackMetric("ClassCount", classes.Count());
            return Ok(classes);
        }
        catch (Exception ex)
        {
            _telemetry.TrackException(ex);
            _logger.LogError(ex, "Error fetching all classes");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get class by ClassNum
    /// </summary>
    [HttpGet("{classNum}")]
    [ProducesResponseType(typeof(ClassSchedule2254), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClassSchedule2254>> GetClass(string classNum)
    {
        try
        {
            _telemetry.TrackEvent("GetClassByNum", new Dictionary<string, string>
            {
                { "ClassNum", classNum }
            });

            var classSchedule = await _repository.GetByClassNumAsync(classNum);

            if (classSchedule == null)
            {
                _logger.LogWarning("Class with ClassNum {ClassNum} not found", classNum);
                return NotFound();
            }

            return Ok(classSchedule);
        }
        catch (Exception ex)
        {
            _telemetry.TrackException(ex);
            _logger.LogError(ex, "Error fetching class {ClassNum}", classNum);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get classes by term
    /// </summary>
    [HttpGet("term/{term}")]
    [ProducesResponseType(typeof(IEnumerable<ClassSchedule2254>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClassSchedule2254>>> GetByTerm(string term)
    {
        try
        {
            _telemetry.TrackEvent("GetClassesByTerm", new Dictionary<string, string>
            {
                { "Term", term }
            });

            var classes = await _repository.GetByTermAsync(term);
            return Ok(classes);
        }
        catch (Exception ex)
        {
            _telemetry.TrackException(ex);
            _logger.LogError(ex, "Error fetching classes for term: {Term}", term);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get classes by subject
    /// </summary>
    [HttpGet("subject/{subject}")]
    [ProducesResponseType(typeof(IEnumerable<ClassSchedule2254>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClassSchedule2254>>> GetBySubject(string subject)
    {
        try
        {
            _telemetry.TrackEvent("GetClassesBySubject", new Dictionary<string, string>
            {
                { "Subject", subject }
            });

            var classes = await _repository.GetBySubjectAsync(subject);
            return Ok(classes);
        }
        catch (Exception ex)
        {
            _telemetry.TrackException(ex);
            _logger.LogError(ex, "Error fetching classes for subject: {Subject}", subject);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get classes by instructor
    /// </summary>
    [HttpGet("instructor/{instructorId}")]
    [ProducesResponseType(typeof(IEnumerable<ClassSchedule2254>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClassSchedule2254>>> GetByInstructor(int instructorId)
    {
        try
        {
            _telemetry.TrackEvent("GetClassesByInstructor", new Dictionary<string, string>
            {
                { "InstructorId", instructorId.ToString() }
            });

            var classes = await _repository.GetByInstructorAsync(instructorId);
            return Ok(classes);
        }
        catch (Exception ex)
        {
            _telemetry.TrackException(ex);
            _logger.LogError(ex, "Error fetching classes for instructor: {InstructorId}", instructorId);
            return StatusCode(500, "Internal server error");
        }
    }
}
