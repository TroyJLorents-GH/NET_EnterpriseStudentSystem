using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentSystem.Domain.Entities;

[Table("ClassSchedule2254", Schema = "dbo")]
public class ClassSchedule2254
{
    [Key]
    public string ClassNum { get; set; } = string.Empty;

    public string? Term { get; set; }
    public string Session { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public int CatalogNum { get; set; }
    public int SectionNum { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? InstructorID { get; set; }
    public string InstructorLastName { get; set; } = string.Empty;
    public string InstructorFirstName { get; set; } = string.Empty;
    public string InstructorEmail { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Campus { get; set; } = string.Empty;
    public string AcadCareer { get; set; } = string.Empty;
}
