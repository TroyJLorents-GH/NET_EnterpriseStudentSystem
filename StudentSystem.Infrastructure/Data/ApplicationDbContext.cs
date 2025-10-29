using Microsoft.EntityFrameworkCore;
using StudentSystem.Domain.Entities;

namespace StudentSystem.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<StudentLookup> StudentData { get; set; }
    public DbSet<ClassSchedule2254> ClassSchedules { get; set; }
    public DbSet<StudentClassAssignment> StudentClassAssignments { get; set; }
    public DbSet<MastersIAGraderApplication2254> GraderApplications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure entities
        modelBuilder.Entity<StudentLookup>(entity =>
        {
            entity.HasKey(e => e.Student_ID);
            entity.ToTable("StudentData", "dbo");
        });

        modelBuilder.Entity<ClassSchedule2254>(entity =>
        {
            entity.HasKey(e => e.ClassNum);
            entity.ToTable("ClassSchedule2254", "dbo");
        });

        modelBuilder.Entity<StudentClassAssignment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("StudentClassAssignments", "dbo");
        });

        modelBuilder.Entity<MastersIAGraderApplication2254>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("MastersIAGraderApplication2254", "dbo");
        });
    }
}
