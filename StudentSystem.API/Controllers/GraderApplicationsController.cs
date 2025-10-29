using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using StudentSystem.Domain.Entities;
using StudentSystem.Domain.Interfaces;

namespace StudentSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GraderApplicationsController : ControllerBase
{
    private readonly IGraderApplicationRepository _repository;
    private readonly TelemetryClient _telemetryClient;
    private readonly ILogger<GraderApplicationsController> _logger;

    public GraderApplicationsController(
        IGraderApplicationRepository repository,
        TelemetryClient telemetryClient,
        ILogger<GraderApplicationsController> logger)
    {
        _repository = repository;
        _telemetryClient = telemetryClient;
        _logger = logger;
    }

    /// <summary>
    /// Get all grader applications
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<MastersIAGraderApplication2254>>> GetAllApplications()
    {
        _logger.LogInformation("Fetching all grader applications");
        _telemetryClient.TrackEvent("GraderApplications_GetAll");

        var applications = await _repository.GetAllAsync();
        return Ok(applications);
    }

    /// <summary>
    /// Get grader application by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<MastersIAGraderApplication2254>> GetApplication(int id)
    {
        _logger.LogInformation("Fetching grader application {ApplicationId}", id);
        _telemetryClient.TrackEvent("GraderApplications_GetById", new Dictionary<string, string>
        {
            { "ApplicationId", id.ToString() }
        });

        var application = await _repository.GetByIdAsync(id);
        if (application == null)
        {
            _logger.LogWarning("Grader application {ApplicationId} not found", id);
            return NotFound();
        }

        return Ok(application);
    }

    /// <summary>
    /// Get applications by degree program
    /// </summary>
    [HttpGet("program/{degreeProgram}")]
    public async Task<ActionResult<List<MastersIAGraderApplication2254>>> GetByDegreeProgram(string degreeProgram)
    {
        _logger.LogInformation("Fetching applications for degree program {DegreeProgram}", degreeProgram);
        _telemetryClient.TrackEvent("GraderApplications_GetByProgram");

        var applications = await _repository.GetByDegreeProgram(degreeProgram);
        return Ok(applications);
    }

    /// <summary>
    /// Get applications by email
    /// </summary>
    [HttpGet("email/{email}")]
    public async Task<ActionResult<List<MastersIAGraderApplication2254>>> GetByEmail(string email)
    {
        _logger.LogInformation("Fetching applications for email {Email}", email);
        _telemetryClient.TrackEvent("GraderApplications_GetByEmail");

        var applications = await _repository.GetByEmail(email);
        return Ok(applications);
    }

    /// <summary>
    /// Get applications by student ID
    /// </summary>
    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<List<MastersIAGraderApplication2254>>> GetByStudentId(int studentId)
    {
        _logger.LogInformation("Fetching applications for student {StudentId}", studentId);
        _telemetryClient.TrackEvent("GraderApplications_GetByStudentId");

        var applications = await _repository.GetByStudentId(studentId);
        return Ok(applications);
    }

    /// <summary>
    /// Create a new grader application
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<MastersIAGraderApplication2254>> CreateApplication(MastersIAGraderApplication2254 application)
    {
        _logger.LogInformation("Creating new grader application for {Email}", application.Email);
        _telemetryClient.TrackEvent("GraderApplications_Create");

        var created = await _repository.AddAsync(application);
        return CreatedAtAction(nameof(GetApplication), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update an existing grader application
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateApplication(int id, MastersIAGraderApplication2254 application)
    {
        if (id != application.Id)
        {
            return BadRequest();
        }

        _logger.LogInformation("Updating grader application {ApplicationId}", id);
        _telemetryClient.TrackEvent("GraderApplications_Update");

        await _repository.UpdateAsync(application);
        return NoContent();
    }

    /// <summary>
    /// Delete a grader application
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteApplication(int id)
    {
        _logger.LogInformation("Deleting grader application {ApplicationId}", id);
        _telemetryClient.TrackEvent("GraderApplications_Delete");

        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
