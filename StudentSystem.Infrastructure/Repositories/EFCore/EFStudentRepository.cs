using Microsoft.EntityFrameworkCore;
using StudentSystem.Domain.Entities;
using StudentSystem.Domain.Interfaces;
using StudentSystem.Infrastructure.Data;

namespace StudentSystem.Infrastructure.Repositories.EFCore;

/// <summary>
/// Entity Framework Core implementation of Student Repository
/// Demonstrates LINQ queries and EF Core features
/// </summary>
public class EFStudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;

    public EFStudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StudentLookup>> GetAllAsync()
    {
        return await _context.StudentData
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<StudentLookup?> GetByIdAsync(object id)
    {
        if (id is int studentId)
        {
            return await _context.StudentData
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Student_ID == studentId);
        }
        return null;
    }

    public async Task<StudentLookup?> GetByStudentIdAsync(int studentId)
    {
        return await _context.StudentData
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Student_ID == studentId);
    }

    public async Task<IEnumerable<StudentLookup>> GetByASUriteAsync(string asurite)
    {
        return await _context.StudentData
            .AsNoTracking()
            .Where(s => s.ASUrite.Contains(asurite))
            .ToListAsync();
    }

    public async Task<IEnumerable<StudentLookup>> GetByDepartmentAsync(string department)
    {
        return await _context.StudentData
            .AsNoTracking()
            .Where(s => s.Acad_Org.Contains(department))
            .OrderBy(s => s.Last_Name)
            .ThenBy(s => s.First_Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<StudentLookup>> SearchStudentsAsync(string searchTerm)
    {
        return await _context.StudentData
            .AsNoTracking()
            .Where(s => s.First_Name.Contains(searchTerm) ||
                       s.Last_Name.Contains(searchTerm) ||
                       s.ASUrite.Contains(searchTerm) ||
                       s.ASU_Email_Adress.Contains(searchTerm))
            .Take(50)
            .ToListAsync();
    }

    public async Task<StudentLookup> AddAsync(StudentLookup entity)
    {
        await _context.StudentData.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(StudentLookup entity)
    {
        _context.StudentData.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(object id)
    {
        if (id is int studentId)
        {
            var student = await _context.StudentData.FindAsync(studentId);
            if (student != null)
            {
                _context.StudentData.Remove(student);
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
