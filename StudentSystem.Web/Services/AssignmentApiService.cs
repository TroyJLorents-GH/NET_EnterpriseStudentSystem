using System.Net.Http.Json;
using StudentSystem.Domain.Entities;

namespace StudentSystem.Web.Services;

public class AssignmentApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AssignmentApiService> _logger;

    public AssignmentApiService(HttpClient httpClient, ILogger<AssignmentApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<StudentClassAssignment>> GetAllAssignmentsAsync()
    {
        try
        {
            var assignments = await _httpClient.GetFromJsonAsync<List<StudentClassAssignment>>("api/assignments");
            return assignments ?? new List<StudentClassAssignment>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching assignments");
            return new List<StudentClassAssignment>();
        }
    }

    public async Task<StudentClassAssignment?> GetAssignmentByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<StudentClassAssignment>($"api/assignments/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching assignment {AssignmentId}", id);
            return null;
        }
    }

    public async Task<List<StudentClassAssignment>> GetAssignmentsByStudentIdAsync(int studentId)
    {
        try
        {
            var assignments = await _httpClient.GetFromJsonAsync<List<StudentClassAssignment>>($"api/assignments/student/{studentId}");
            return assignments ?? new List<StudentClassAssignment>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching assignments by student ID");
            return new List<StudentClassAssignment>();
        }
    }

    public async Task<List<StudentClassAssignment>> GetAssignmentsByTermAsync(string term)
    {
        try
        {
            var assignments = await _httpClient.GetFromJsonAsync<List<StudentClassAssignment>>($"api/assignments/term/{term}");
            return assignments ?? new List<StudentClassAssignment>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching assignments by term");
            return new List<StudentClassAssignment>();
        }
    }

    public async Task<List<StudentClassAssignment>> GetAssignmentsByClassNumAsync(string classNum)
    {
        try
        {
            var assignments = await _httpClient.GetFromJsonAsync<List<StudentClassAssignment>>($"api/assignments/class/{classNum}");
            return assignments ?? new List<StudentClassAssignment>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching assignments by class number");
            return new List<StudentClassAssignment>();
        }
    }

    public async Task<StudentClassAssignment?> CreateAssignmentAsync(StudentClassAssignment assignment)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/assignments", assignment);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<StudentClassAssignment>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating assignment");
            return null;
        }
    }

    public async Task<bool> UpdateAssignmentAsync(int id, StudentClassAssignment assignment)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/assignments/{id}", assignment);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating assignment {AssignmentId}", id);
            return false;
        }
    }
}
