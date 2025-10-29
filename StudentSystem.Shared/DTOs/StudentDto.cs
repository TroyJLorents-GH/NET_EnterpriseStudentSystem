namespace StudentSystem.Shared.DTOs;

public class StudentDto
{
    public int Student_ID { get; set; }
    public string ASUrite { get; set; } = string.Empty;
    public string First_Name { get; set; } = string.Empty;
    public string Last_Name { get; set; } = string.Empty;
    public string ASU_Email_Adress { get; set; } = string.Empty;
    public double Cumulative_GPA { get; set; }
    public double Current_GPA { get; set; }
    public string Acad_Org { get; set; } = string.Empty;
    public string Campus { get; set; } = string.Empty;
    public string Prog_Status { get; set; } = string.Empty;
}

public class AssignmentDto
{
    public int Id { get; set; }
    public int? Student_ID { get; set; }
    public string? ASUrite { get; set; }
    public string? Position { get; set; }
    public string? ClassNum { get; set; }
    public string Term { get; set; } = string.Empty;
    public string? InstructorName { get; set; }
    public double Compensation { get; set; }
    public bool? Offer_Sent { get; set; }
    public bool? Offer_Signed { get; set; }
}
