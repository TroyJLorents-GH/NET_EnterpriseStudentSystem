using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudentSystem.Domain.Entities;
using StudentSystem.Domain.Interfaces;

namespace StudentSystem.Infrastructure.Repositories.Dapper;

/// <summary>
/// Dapper implementation of Student Repository
/// Demonstrates lightweight ORM with raw SQL and mapping
/// </summary>
public class DapperStudentRepository : IStudentRepository
{
    private readonly string _connectionString;

    public DapperStudentRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException("Connection string not found");
    }

    private IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }

    public async Task<IEnumerable<StudentLookup>> GetAllAsync()
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM dbo.StudentData";
        return await connection.QueryAsync<StudentLookup>(sql);
    }

    public async Task<StudentLookup?> GetByIdAsync(object id)
    {
        if (id is int studentId)
        {
            return await GetByStudentIdAsync(studentId);
        }
        return null;
    }

    public async Task<StudentLookup?> GetByStudentIdAsync(int studentId)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM dbo.StudentData WHERE Student_ID = @StudentId";
        return await connection.QueryFirstOrDefaultAsync<StudentLookup>(sql, new { StudentId = studentId });
    }

    public async Task<IEnumerable<StudentLookup>> GetByASUriteAsync(string asurite)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM dbo.StudentData WHERE ASUrite LIKE @ASUrite";
        return await connection.QueryAsync<StudentLookup>(sql, new { ASUrite = $"%{asurite}%" });
    }

    public async Task<IEnumerable<StudentLookup>> GetByDepartmentAsync(string department)
    {
        using var connection = CreateConnection();
        var sql = @"
            SELECT * FROM dbo.StudentData
            WHERE Acad_Org LIKE @Department
            ORDER BY Last_Name, First_Name";
        return await connection.QueryAsync<StudentLookup>(sql, new { Department = $"%{department}%" });
    }

    public async Task<IEnumerable<StudentLookup>> SearchStudentsAsync(string searchTerm)
    {
        using var connection = CreateConnection();
        var sql = @"
            SELECT TOP 50 * FROM dbo.StudentData
            WHERE First_Name LIKE @SearchTerm
               OR Last_Name LIKE @SearchTerm
               OR ASUrite LIKE @SearchTerm
               OR ASU_Email_Adress LIKE @SearchTerm";
        return await connection.QueryAsync<StudentLookup>(sql, new { SearchTerm = $"%{searchTerm}%" });
    }

    public async Task<StudentLookup> AddAsync(StudentLookup entity)
    {
        using var connection = CreateConnection();
        var sql = @"
            INSERT INTO dbo.StudentData
            (Cumulative_GPA, Current_GPA, Term_Code, Admit_Term, ASUrite, First_Name, Last_Name,
             Middle_Name, Preferred_Primary_First_Name, ASU_Email_Adress, Acad_Prog, Acad_Prog_Descr,
             Acad_Career, Acad_Group, Acad_Org, Acad_Plan, Plan_Descr, Degree, Transcript_Description,
             Plan_Type, Acad_Lvl_BOT, Acad_Lvl_EOT, Prog_Status, Expected_Graduation_Term, Campus, Deans_List)
            VALUES
            (@Cumulative_GPA, @Current_GPA, @Term_Code, @Admit_Term, @ASUrite, @First_Name, @Last_Name,
             @Middle_Name, @Preferred_Primary_First_Name, @ASU_Email_Adress, @Acad_Prog, @Acad_Prog_Descr,
             @Acad_Career, @Acad_Group, @Acad_Org, @Acad_Plan, @Plan_Descr, @Degree, @Transcript_Description,
             @Plan_Type, @Acad_Lvl_BOT, @Acad_Lvl_EOT, @Prog_Status, @Expected_Graduation_Term, @Campus, @Deans_List);
            SELECT CAST(SCOPE_IDENTITY() as int)";

        var id = await connection.QuerySingleAsync<int>(sql, entity);
        entity.Student_ID = id;
        return entity;
    }

    public async Task UpdateAsync(StudentLookup entity)
    {
        using var connection = CreateConnection();
        var sql = @"
            UPDATE dbo.StudentData
            SET Cumulative_GPA = @Cumulative_GPA,
                Current_GPA = @Current_GPA,
                ASUrite = @ASUrite,
                First_Name = @First_Name,
                Last_Name = @Last_Name,
                ASU_Email_Adress = @ASU_Email_Adress,
                Acad_Org = @Acad_Org,
                Campus = @Campus
            WHERE Student_ID = @Student_ID";

        await connection.ExecuteAsync(sql, entity);
    }

    public async Task DeleteAsync(object id)
    {
        if (id is int studentId)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM dbo.StudentData WHERE Student_ID = @StudentId";
            await connection.ExecuteAsync(sql, new { StudentId = studentId });
        }
    }

    public Task<int> SaveChangesAsync()
    {
        // Dapper executes immediately, no need for separate save
        return Task.FromResult(0);
    }
}
