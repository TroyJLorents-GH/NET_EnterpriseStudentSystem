using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentSystem.Domain.Entities;

[Table("Students")]
public class Student
{
    [Key]
    public int StudentId { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [MaxLength(50)]
    public string? StudentNumber { get; set; }

    public DateTime EnrollmentDate { get; set; }

    public string? Major { get; set; }

    public decimal GPA { get; set; }

    public StudentStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public enum StudentStatus
{
    Active = 1,
    Inactive = 2,
    Graduated = 3,
    Suspended = 4
}
