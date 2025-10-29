using StudentSystem.Domain.Entities;

namespace StudentSystem.Domain.Interfaces;

public interface IAssignmentRepository : IRepository<StudentClassAssignment>
{
    Task<IEnumerable<StudentClassAssignment>> GetByStudentIdAsync(int studentId);
    Task<IEnumerable<StudentClassAssignment>> GetByTermAsync(string term);
    Task<IEnumerable<StudentClassAssignment>> GetByClassNumAsync(string classNum);
    Task<StudentClassAssignment?> GetAssignmentByIdAsync(int id);
}
