using System.Net.Http.Json;
using StudentSystem.Shared.DTOs;
using StudentSystem.Domain.Entities;

namespace StudentSystem.Web.Services;

public class StudentApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<StudentApiService> _logger;

    public StudentApiService(HttpClient httpClient, ILogger<StudentApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<StudentLookup>> GetAllStudentsAsync()
    {
        try
        {
            var students = await _httpClient.GetFromJsonAsync<List<StudentLookup>>("api/students");
            return students ?? new List<StudentLookup>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching students");
            return new List<StudentLookup>();
        }
    }

    public async Task<StudentLookup?> GetStudentByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<StudentLookup>($"api/students/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching student {StudentId}", id);
            return null;
        }
    }

    public async Task<List<StudentLookup>> SearchStudentsAsync(string searchTerm)
    {
        try
        {
            var students = await _httpClient.GetFromJsonAsync<List<StudentLookup>>($"api/students/search?term={searchTerm}");
            return students ?? new List<StudentLookup>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching students");
            return new List<StudentLookup>();
        }
    }

    public async Task<List<StudentLookup>> GetStudentsByDepartmentAsync(string department)
    {
        try
        {
            var students = await _httpClient.GetFromJsonAsync<List<StudentLookup>>($"api/students/department/{department}");
            return students ?? new List<StudentLookup>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching students by department");
            return new List<StudentLookup>();
        }
    }
}
