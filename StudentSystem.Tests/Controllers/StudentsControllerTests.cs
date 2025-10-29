using FluentAssertions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StudentSystem.API.Controllers;
using StudentSystem.Domain.Entities;
using StudentSystem.Domain.Interfaces;
using Xunit;

namespace StudentSystem.Tests.Controllers;

public class StudentsControllerTests
{
    private readonly Mock<IStudentRepository> _mockRepository;
    private readonly StudentsController _controller;
    private readonly TelemetryClient _telemetryClient;
    private readonly Mock<ILogger<StudentsController>> _mockLogger;

    public StudentsControllerTests()
    {
        _mockRepository = new Mock<IStudentRepository>();
        _mockLogger = new Mock<ILogger<StudentsController>>();
        _telemetryClient = new TelemetryClient(new TelemetryConfiguration());
        _controller = new StudentsController(_mockRepository.Object, _telemetryClient, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllStudents_ShouldReturnOkResult_WithStudents()
    {
        // Arrange
        var students = new List<StudentLookup>
        {
            CreateTestStudent(1, "John", "Doe"),
            CreateTestStudent(2, "Jane", "Smith")
        };
        _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(students);

        // Act
        var result = await _controller.GetAllStudents();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedStudents = okResult!.Value as IEnumerable<StudentLookup>;
        returnedStudents.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetStudent_ShouldReturnOkResult_WhenStudentExists()
    {
        // Arrange
        var student = CreateTestStudent(1, "John", "Doe");
        _mockRepository.Setup(repo => repo.GetByStudentIdAsync(1)).ReturnsAsync(student);

        // Act
        var result = await _controller.GetStudent(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedStudent = okResult!.Value as StudentLookup;
        returnedStudent!.First_Name.Should().Be("John");
    }

    [Fact]
    public async Task GetStudent_ShouldReturnNotFound_WhenStudentDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetByStudentIdAsync(999)).ReturnsAsync((StudentLookup?)null);

        // Act
        var result = await _controller.GetStudent(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task SearchStudents_ShouldReturnOkResult_WithMatchingStudents()
    {
        // Arrange
        var students = new List<StudentLookup>
        {
            CreateTestStudent(1, "John", "Doe")
        };
        _mockRepository.Setup(repo => repo.SearchStudentsAsync("John")).ReturnsAsync(students);

        // Act
        var result = await _controller.SearchStudents("John");

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedStudents = okResult!.Value as IEnumerable<StudentLookup>;
        returnedStudents.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdateStudent_ShouldReturnNoContent_WhenUpdateSucceeds()
    {
        // Arrange
        var student = CreateTestStudent(1, "John", "Doe");
        _mockRepository.Setup(repo => repo.GetByStudentIdAsync(1)).ReturnsAsync(student);
        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<StudentLookup>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateStudent(1, student);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateStudent_ShouldReturnBadRequest_WhenIdMismatch()
    {
        // Arrange
        var student = CreateTestStudent(1, "John", "Doe");

        // Act
        var result = await _controller.UpdateStudent(2, student);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    private StudentLookup CreateTestStudent(int id, string firstName, string lastName)
    {
        return new StudentLookup
        {
            Student_ID = id,
            First_Name = firstName,
            Last_Name = lastName,
            ASU_Email_Adress = $"{firstName.ToLower()}{lastName.ToLower()}@asu.edu",
            ASUrite = $"{firstName.ToLower()}{lastName.ToLower()}",
            Cumulative_GPA = 3.5,
            Current_GPA = 3.6,
            Acad_Prog = "MS",
            Acad_Prog_Descr = "Master of Science",
            Acad_Career = "GRAD",
            Acad_Group = "Engineering",
            Acad_Org = "Computer Science",
            Acad_Plan = "MSCSE",
            Plan_Descr = "MS Computer Science",
            Degree = "MS",
            Transcript_Description = "Master of Science in Computer Science",
            Plan_Type = "MAJ",
            Acad_Lvl_BOT = "GR",
            Acad_Lvl_EOT = "GR",
            Prog_Status = "AC",
            Campus = "TEMPE",
            Deans_List = "N"
        };
    }
}
