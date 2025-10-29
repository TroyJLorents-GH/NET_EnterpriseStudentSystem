using StudentSystem.Domain.Entities;

namespace StudentSystem.Domain.Interfaces;

public interface IGraderApplicationRepository : IRepository<MastersIAGraderApplication2254>
{
    Task<List<MastersIAGraderApplication2254>> GetByDegreeProgram(string degreeProgram);
    Task<List<MastersIAGraderApplication2254>> GetByEmail(string email);
    Task<List<MastersIAGraderApplication2254>> GetByStudentId(int studentId);
    Task<List<MastersIAGraderApplication2254>> GetByDateRange(DateTime startDate, DateTime endDate);
}
