using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudentSystem.Domain.Entities;
using StudentSystem.Domain.Interfaces;

namespace StudentSystem.Infrastructure.Repositories.ADO;

/// <summary>
/// ADO.NET implementation of Student Repository
/// Demonstrates raw ADO.NET with SqlCommand, SqlDataReader, and manual mapping
/// </summary>
public class AdoStudentRepository : IStudentRepository
{
    private readonly string _connectionString;

    public AdoStudentRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException("Connection string not found");
    }

    public async Task<IEnumerable<StudentLookup>> GetAllAsync()
    {
        var students = new List<StudentLookup>();

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("SELECT * FROM dbo.StudentData", connection);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            students.Add(MapReaderToStudent(reader));
        }

        return students;
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
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(
            "SELECT * FROM dbo.StudentData WHERE Student_ID = @StudentId",
            connection);

        command.Parameters.AddWithValue("@StudentId", studentId);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return MapReaderToStudent(reader);
        }

        return null;
    }

    public async Task<IEnumerable<StudentLookup>> GetByASUriteAsync(string asurite)
    {
        var students = new List<StudentLookup>();

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(
            "SELECT * FROM dbo.StudentData WHERE ASUrite LIKE @ASUrite",
            connection);

        command.Parameters.AddWithValue("@ASUrite", $"%{asurite}%");

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            students.Add(MapReaderToStudent(reader));
        }

        return students;
    }

    public async Task<IEnumerable<StudentLookup>> GetByDepartmentAsync(string department)
    {
        var students = new List<StudentLookup>();

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(
            "SELECT * FROM dbo.StudentData WHERE Acad_Org LIKE @Department ORDER BY Last_Name, First_Name",
            connection);

        command.Parameters.AddWithValue("@Department", $"%{department}%");

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            students.Add(MapReaderToStudent(reader));
        }

        return students;
    }

    public async Task<IEnumerable<StudentLookup>> SearchStudentsAsync(string searchTerm)
    {
        var students = new List<StudentLookup>();

        using var connection = new SqlConnection(_connectionString);
        var sql = @"
            SELECT TOP 50 * FROM dbo.StudentData
            WHERE First_Name LIKE @SearchTerm
               OR Last_Name LIKE @SearchTerm
               OR ASUrite LIKE @SearchTerm
               OR ASU_Email_Adress LIKE @SearchTerm";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            students.Add(MapReaderToStudent(reader));
        }

        return students;
    }

    public async Task<StudentLookup> AddAsync(StudentLookup entity)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = @"
            INSERT INTO dbo.StudentData
            (Cumulative_GPA, Current_GPA, ASUrite, First_Name, Last_Name, ASU_Email_Adress,
             Acad_Prog, Acad_Prog_Descr, Acad_Career, Acad_Group, Acad_Org, Acad_Plan,
             Plan_Descr, Degree, Transcript_Description, Plan_Type, Acad_Lvl_BOT,
             Acad_Lvl_EOT, Prog_Status, Campus, Deans_List)
            VALUES
            (@Cumulative_GPA, @Current_GPA, @ASUrite, @First_Name, @Last_Name, @ASU_Email_Adress,
             @Acad_Prog, @Acad_Prog_Descr, @Acad_Career, @Acad_Group, @Acad_Org, @Acad_Plan,
             @Plan_Descr, @Degree, @Transcript_Description, @Plan_Type, @Acad_Lvl_BOT,
             @Acad_Lvl_EOT, @Prog_Status, @Campus, @Deans_List);
            SELECT CAST(SCOPE_IDENTITY() as int)";

        using var command = new SqlCommand(sql, connection);
        AddStudentParameters(command, entity);

        await connection.OpenAsync();
        var id = (int)await command.ExecuteScalarAsync();
        entity.Student_ID = id;

        return entity;
    }

    public async Task UpdateAsync(StudentLookup entity)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = @"
            UPDATE dbo.StudentData
            SET Cumulative_GPA = @Cumulative_GPA,
                Current_GPA = @Current_GPA,
                First_Name = @First_Name,
                Last_Name = @Last_Name,
                ASU_Email_Adress = @ASU_Email_Adress
            WHERE Student_ID = @Student_ID";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Cumulative_GPA", entity.Cumulative_GPA);
        command.Parameters.AddWithValue("@Current_GPA", entity.Current_GPA);
        command.Parameters.AddWithValue("@First_Name", entity.First_Name);
        command.Parameters.AddWithValue("@Last_Name", entity.Last_Name);
        command.Parameters.AddWithValue("@ASU_Email_Adress", entity.ASU_Email_Adress);
        command.Parameters.AddWithValue("@Student_ID", entity.Student_ID);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(object id)
    {
        if (id is int studentId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                "DELETE FROM dbo.StudentData WHERE Student_ID = @StudentId",
                connection);

            command.Parameters.AddWithValue("@StudentId", studentId);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }

    public Task<int> SaveChangesAsync()
    {
        // ADO.NET executes immediately, no need for separate save
        return Task.FromResult(0);
    }

    private StudentLookup MapReaderToStudent(SqlDataReader reader)
    {
        return new StudentLookup
        {
            Student_ID = reader.GetInt32("Student_ID"),
            Cumulative_GPA = reader.GetDouble("Cumulative_GPA"),
            Current_GPA = reader.GetDouble("Current_GPA"),
            Term_Code = reader.IsDBNull("Term_Code") ? null : reader.GetInt32("Term_Code"),
            Admit_Term = reader.IsDBNull("Admit_Term") ? null : reader.GetInt32("Admit_Term"),
            ASUrite = reader.GetString("ASUrite"),
            First_Name = reader.GetString("First_Name"),
            Last_Name = reader.GetString("Last_Name"),
            Middle_Name = reader.IsDBNull("Middle_Name") ? null : reader.GetString("Middle_Name"),
            Preferred_Primary_First_Name = reader.IsDBNull("Preferred_Primary_First_Name")
                ? null : reader.GetString("Preferred_Primary_First_Name"),
            ASU_Email_Adress = reader.GetString("ASU_Email_Adress"),
            Acad_Prog = reader.GetString("Acad_Prog"),
            Acad_Prog_Descr = reader.GetString("Acad_Prog_Descr"),
            Acad_Career = reader.GetString("Acad_Career"),
            Acad_Group = reader.GetString("Acad_Group"),
            Acad_Org = reader.GetString("Acad_Org"),
            Acad_Plan = reader.GetString("Acad_Plan"),
            Plan_Descr = reader.GetString("Plan_Descr"),
            Degree = reader.GetString("Degree"),
            Transcript_Description = reader.GetString("Transcript_Description"),
            Plan_Type = reader.GetString("Plan_Type"),
            Acad_Lvl_BOT = reader.GetString("Acad_Lvl_BOT"),
            Acad_Lvl_EOT = reader.GetString("Acad_Lvl_EOT"),
            Prog_Status = reader.GetString("Prog_Status"),
            Expected_Graduation_Term = reader.IsDBNull("Expected_Graduation_Term")
                ? null : reader.GetString("Expected_Graduation_Term"),
            Campus = reader.GetString("Campus"),
            Deans_List = reader.GetString("Deans_List")
        };
    }

    private void AddStudentParameters(SqlCommand command, StudentLookup entity)
    {
        command.Parameters.AddWithValue("@Cumulative_GPA", entity.Cumulative_GPA);
        command.Parameters.AddWithValue("@Current_GPA", entity.Current_GPA);
        command.Parameters.AddWithValue("@ASUrite", entity.ASUrite);
        command.Parameters.AddWithValue("@First_Name", entity.First_Name);
        command.Parameters.AddWithValue("@Last_Name", entity.Last_Name);
        command.Parameters.AddWithValue("@ASU_Email_Adress", entity.ASU_Email_Adress);
        command.Parameters.AddWithValue("@Acad_Prog", entity.Acad_Prog);
        command.Parameters.AddWithValue("@Acad_Prog_Descr", entity.Acad_Prog_Descr);
        command.Parameters.AddWithValue("@Acad_Career", entity.Acad_Career);
        command.Parameters.AddWithValue("@Acad_Group", entity.Acad_Group);
        command.Parameters.AddWithValue("@Acad_Org", entity.Acad_Org);
        command.Parameters.AddWithValue("@Acad_Plan", entity.Acad_Plan);
        command.Parameters.AddWithValue("@Plan_Descr", entity.Plan_Descr);
        command.Parameters.AddWithValue("@Degree", entity.Degree);
        command.Parameters.AddWithValue("@Transcript_Description", entity.Transcript_Description);
        command.Parameters.AddWithValue("@Plan_Type", entity.Plan_Type);
        command.Parameters.AddWithValue("@Acad_Lvl_BOT", entity.Acad_Lvl_BOT);
        command.Parameters.AddWithValue("@Acad_Lvl_EOT", entity.Acad_Lvl_EOT);
        command.Parameters.AddWithValue("@Prog_Status", entity.Prog_Status);
        command.Parameters.AddWithValue("@Campus", entity.Campus);
        command.Parameters.AddWithValue("@Deans_List", entity.Deans_List);
    }
}
