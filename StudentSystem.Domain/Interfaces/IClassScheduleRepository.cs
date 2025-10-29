using StudentSystem.Domain.Entities;

namespace StudentSystem.Domain.Interfaces;

public interface IClassScheduleRepository : IRepository<ClassSchedule2254>
{
    Task<IEnumerable<ClassSchedule2254>> GetByTermAsync(string term);
    Task<IEnumerable<ClassSchedule2254>> GetByInstructorAsync(int instructorId);
    Task<IEnumerable<ClassSchedule2254>> GetBySubjectAsync(string subject);
    Task<ClassSchedule2254?> GetByClassNumAsync(string classNum);
}
