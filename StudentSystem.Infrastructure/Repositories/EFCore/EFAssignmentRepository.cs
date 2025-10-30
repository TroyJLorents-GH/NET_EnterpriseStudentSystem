using Microsoft.EntityFrameworkCore;
using StudentSystem.Domain.Entities;
using StudentSystem.Domain.Interfaces;
using StudentSystem.Infrastructure.Data;

namespace StudentSystem.Infrastructure.Repositories.EFCore;

public class EFAssignmentRepository : IAssignmentRepository
{
    private readonly ApplicationDbContext _context;

    public EFAssignmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StudentClassAssignment>> GetAllAsync()
    {
        return await _context.StudentClassAssignments
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<StudentClassAssignment?> GetByIdAsync(object id)
    {
        return await _context.StudentClassAssignments.FindAsync(id);
    }

    public async Task<StudentClassAssignment?> GetAssignmentByIdAsync(int id)
    {
        return await _context.StudentClassAssignments.FindAsync(id);
    }

    public async Task<IEnumerable<StudentClassAssignment>> GetByStudentIdAsync(int studentId)
    {
        return await _context.StudentClassAssignments
            .Where(a => a.Student_ID == studentId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<StudentClassAssignment>> GetByTermAsync(string term)
    {
        return await _context.StudentClassAssignments
            .Where(a => a.Term == term)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<StudentClassAssignment>> GetByClassNumAsync(string classNum)
    {
        return await _context.StudentClassAssignments
            .Where(a => a.ClassNum == classNum)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<StudentClassAssignment> AddAsync(StudentClassAssignment entity)
    {
        _context.StudentClassAssignments.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(StudentClassAssignment entity)
    {
        _context.StudentClassAssignments.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(object id)
    {
        var assignment = await GetByIdAsync(id);
        if (assignment != null)
        {
            _context.StudentClassAssignments.Remove(assignment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
