using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentSystem.Domain.Entities;

[Table("StudentData", Schema = "dbo")]
public class StudentLookup
{
    [Key]
    public int Student_ID { get; set; }

    public double Cumulative_GPA { get; set; }
    public double Current_GPA { get; set; }

    public int? Term_Code { get; set; }
    public int? Admit_Term { get; set; }

    [Required]
    [MaxLength(50)]
    public string ASUrite { get; set; } = string.Empty;

    [Required]
    [MaxLength(75)]
    public string First_Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Last_Name { get; set; } = string.Empty;

    [MaxLength(75)]
    public string? Middle_Name { get; set; }

    [MaxLength(75)]
    public string? Preferred_Primary_First_Name { get; set; }

    [Required]
    [MaxLength(150)]
    public string ASU_Email_Adress { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Acad_Prog { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Acad_Prog_Descr { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    public string Acad_Career { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    public string Acad_Group { get; set; } = string.Empty;

    [Required]
    [MaxLength(25)]
    public string Acad_Org { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Acad_Plan { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Plan_Descr { get; set; } = string.Empty;

    [Required]
    [MaxLength(25)]
    public string Degree { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Transcript_Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Plan_Type { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Acad_Lvl_BOT { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Acad_Lvl_EOT { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Prog_Status { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Expected_Graduation_Term { get; set; }

    [Required]
    [MaxLength(50)]
    public string Campus { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Deans_List { get; set; } = string.Empty;
}
