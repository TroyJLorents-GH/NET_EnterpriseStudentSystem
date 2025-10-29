using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StudentSystem.Domain.Entities;
using StudentSystem.Infrastructure.Data;
using StudentSystem.Infrastructure.Repositories.EFCore;
using Xunit;

namespace StudentSystem.Tests.Repositories;

public class StudentRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly EFStudentRepository _repository;

    public StudentRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new EFStudentRepository(_context);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllStudents()
    {
        // Arrange
        var students = new List<StudentLookup>
        {
            CreateTestStudent(1, "John", "Doe", "jdoe@asu.edu"),
            CreateTestStudent(2, "Jane", "Smith", "jsmith@asu.edu")
        };
        await _context.StudentData.AddRangeAsync(students);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(s => s.First_Name == "John");
        result.Should().Contain(s => s.First_Name == "Jane");
    }

    [Fact]
    public async Task GetByStudentIdAsync_ShouldReturnStudent_WhenExists()
    {
        // Arrange
        var student = CreateTestStudent(1, "John", "Doe", "jdoe@asu.edu");
        await _context.StudentData.AddAsync(student);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByStudentIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.First_Name.Should().Be("John");
        result.Last_Name.Should().Be("Doe");
    }

    [Fact]
    public async Task GetByStudentIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _repository.GetByStudentIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SearchStudentsAsync_ShouldReturnMatchingStudents()
    {
        // Arrange
        var students = new List<StudentLookup>
        {
            CreateTestStudent(1, "John", "Doe", "jdoe@asu.edu"),
            CreateTestStudent(2, "Jane", "Smith", "jsmith@asu.edu"),
            CreateTestStudent(3, "Bob", "Johnson", "bjohnson@asu.edu")
        };
        await _context.StudentData.AddRangeAsync(students);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SearchStudentsAsync("john");

        // Assert
        result.Should().HaveCountGreaterThan(0);
        result.Should().Contain(s => s.First_Name.Contains("John") || s.Last_Name.Contains("Johnson"));
    }

    [Fact]
    public async Task AddAsync_ShouldAddStudent()
    {
        // Arrange
        var student = CreateTestStudent(0, "New", "Student", "newstudent@asu.edu");

        // Act
        var result = await _repository.AddAsync(student);

        // Assert
        result.Student_ID.Should().BeGreaterThan(0);
        _context.StudentData.Should().Contain(s => s.ASU_Email_Adress == "newstudent@asu.edu");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateStudent()
    {
        // Arrange
        var student = CreateTestStudent(1, "John", "Doe", "jdoe@asu.edu");
        await _context.StudentData.AddAsync(student);
        await _context.SaveChangesAsync();

        // Act
        student.First_Name = "Johnny";
        await _repository.UpdateAsync(student);

        // Assert
        var updated = await _context.StudentData.FindAsync(1);
        updated!.First_Name.Should().Be("Johnny");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveStudent()
    {
        // Arrange
        var student = CreateTestStudent(1, "John", "Doe", "jdoe@asu.edu");
        await _context.StudentData.AddAsync(student);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(1);

        // Assert
        var deleted = await _context.StudentData.FindAsync(1);
        deleted.Should().BeNull();
    }

    private StudentLookup CreateTestStudent(int id, string firstName, string lastName, string email)
    {
        return new StudentLookup
        {
            Student_ID = id,
            First_Name = firstName,
            Last_Name = lastName,
            ASU_Email_Adress = email,
            ASUrite = firstName.ToLower() + lastName.ToLower(),
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

    public void Dispose()
    {
        _context.Dispose();
    }
}
