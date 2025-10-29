using StudentSystem.Domain.Entities;

namespace StudentSystem.Domain.Interfaces;

public interface IStudentRepository : IRepository<StudentLookup>
{
    Task<IEnumerable<StudentLookup>> GetByASUriteAsync(string asurite);
    Task<StudentLookup?> GetByStudentIdAsync(int studentId);
    Task<IEnumerable<StudentLookup>> GetByDepartmentAsync(string department);
    Task<IEnumerable<StudentLookup>> SearchStudentsAsync(string searchTerm);
}
