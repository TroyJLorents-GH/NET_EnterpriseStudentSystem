using Microsoft.EntityFrameworkCore;
using StudentSystem.Domain.Entities;
using StudentSystem.Domain.Interfaces;
using StudentSystem.Infrastructure.Data;

namespace StudentSystem.Infrastructure.Repositories.EFCore;

public class EFGraderApplicationRepository : IGraderApplicationRepository
{
    private readonly ApplicationDbContext _context;

    public EFGraderApplicationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MastersIAGraderApplication2254>> GetAllAsync()
    {
        return await _context.GraderApplications
            .OrderByDescending(g => g.Completion_time)
            .ToListAsync();
    }

    public async Task<MastersIAGraderApplication2254?> GetByIdAsync(object id)
    {
        return await _context.GraderApplications.FindAsync(id);
    }

    public async Task<List<MastersIAGraderApplication2254>> GetByDegreeProgram(string degreeProgram)
    {
        return await _context.GraderApplications
            .Where(g => g.DegreeProgram.Contains(degreeProgram))
            .OrderByDescending(g => g.Completion_time)
            .ToListAsync();
    }

    public async Task<List<MastersIAGraderApplication2254>> GetByEmail(string email)
    {
        return await _context.GraderApplications
            .Where(g => g.Email == email || g.YourASUEmailAddress == email)
            .OrderByDescending(g => g.Completion_time)
            .ToListAsync();
    }

    public async Task<List<MastersIAGraderApplication2254>> GetByStudentId(int studentId)
    {
        return await _context.GraderApplications
            .Where(g => g.ASU10DigitID == studentId)
            .OrderByDescending(g => g.Completion_time)
            .ToListAsync();
    }

    public async Task<List<MastersIAGraderApplication2254>> GetByDateRange(DateTime startDate, DateTime endDate)
    {
        return await _context.GraderApplications
            .Where(g => g.Completion_time.HasValue &&
                       g.Completion_time.Value >= startDate &&
                       g.Completion_time.Value <= endDate)
            .OrderByDescending(g => g.Completion_time)
            .ToListAsync();
    }

    public async Task<MastersIAGraderApplication2254> AddAsync(MastersIAGraderApplication2254 entity)
    {
        _context.GraderApplications.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(MastersIAGraderApplication2254 entity)
    {
        _context.GraderApplications.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(object id)
    {
        var application = await GetByIdAsync(id);
        if (application != null)
        {
            _context.GraderApplications.Remove(application);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
