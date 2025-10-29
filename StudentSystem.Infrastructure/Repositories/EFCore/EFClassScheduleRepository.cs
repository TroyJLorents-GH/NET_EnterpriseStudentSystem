using Microsoft.EntityFrameworkCore;
using StudentSystem.Domain.Entities;
using StudentSystem.Domain.Interfaces;
using StudentSystem.Infrastructure.Data;

namespace StudentSystem.Infrastructure.Repositories.EFCore;

public class EFClassScheduleRepository : IClassScheduleRepository
{
    private readonly ApplicationDbContext _context;

    public EFClassScheduleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ClassSchedule2254>> GetAllAsync()
    {
        return await _context.ClassSchedules
            .AsNoTracking()
            .OrderBy(c => c.Subject)
            .ThenBy(c => c.CatalogNum)
            .ToListAsync();
    }

    public async Task<ClassSchedule2254?> GetByIdAsync(object id)
    {
        if (id is string classNum)
        {
            return await GetByClassNumAsync(classNum);
        }
        return null;
    }

    public async Task<ClassSchedule2254?> GetByClassNumAsync(string classNum)
    {
        return await _context.ClassSchedules
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ClassNum == classNum);
    }

    public async Task<IEnumerable<ClassSchedule2254>> GetByTermAsync(string term)
    {
        return await _context.ClassSchedules
            .AsNoTracking()
            .Where(c => c.Term == term)
            .OrderBy(c => c.Subject)
            .ThenBy(c => c.CatalogNum)
            .ToListAsync();
    }

    public async Task<IEnumerable<ClassSchedule2254>> GetByInstructorAsync(int instructorId)
    {
        return await _context.ClassSchedules
            .AsNoTracking()
            .Where(c => c.InstructorID == instructorId)
            .OrderBy(c => c.Subject)
            .ThenBy(c => c.CatalogNum)
            .ToListAsync();
    }

    public async Task<IEnumerable<ClassSchedule2254>> GetBySubjectAsync(string subject)
    {
        return await _context.ClassSchedules
            .AsNoTracking()
            .Where(c => c.Subject.Contains(subject))
            .OrderBy(c => c.CatalogNum)
            .ToListAsync();
    }

    public async Task<ClassSchedule2254> AddAsync(ClassSchedule2254 entity)
    {
        await _context.ClassSchedules.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(ClassSchedule2254 entity)
    {
        _context.ClassSchedules.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(object id)
    {
        if (id is string classNum)
        {
            var classSchedule = await _context.ClassSchedules.FindAsync(classNum);
            if (classSchedule != null)
            {
                _context.ClassSchedules.Remove(classSchedule);
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
