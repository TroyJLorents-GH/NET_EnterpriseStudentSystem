using System.Net.Http.Json;
using StudentSystem.Domain.Entities;

namespace StudentSystem.Web.Services;

public class GraderApplicationApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GraderApplicationApiService> _logger;

    public GraderApplicationApiService(HttpClient httpClient, ILogger<GraderApplicationApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<MastersIAGraderApplication2254>> GetAllApplicationsAsync()
    {
        try
        {
            var applications = await _httpClient.GetFromJsonAsync<List<MastersIAGraderApplication2254>>("api/graderapplications");
            return applications ?? new List<MastersIAGraderApplication2254>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching grader applications");
            return new List<MastersIAGraderApplication2254>();
        }
    }

    public async Task<MastersIAGraderApplication2254?> GetApplicationByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<MastersIAGraderApplication2254>($"api/graderapplications/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching grader application {ApplicationId}", id);
            return null;
        }
    }

    public async Task<List<MastersIAGraderApplication2254>> GetApplicationsByDegreeProgram(string degreeProgram)
    {
        try
        {
            var applications = await _httpClient.GetFromJsonAsync<List<MastersIAGraderApplication2254>>($"api/graderapplications/program/{degreeProgram}");
            return applications ?? new List<MastersIAGraderApplication2254>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching applications by degree program");
            return new List<MastersIAGraderApplication2254>();
        }
    }

    public async Task<List<MastersIAGraderApplication2254>> GetApplicationsByEmail(string email)
    {
        try
        {
            var applications = await _httpClient.GetFromJsonAsync<List<MastersIAGraderApplication2254>>($"api/graderapplications/email/{email}");
            return applications ?? new List<MastersIAGraderApplication2254>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching applications by email");
            return new List<MastersIAGraderApplication2254>();
        }
    }

    public async Task<List<MastersIAGraderApplication2254>> GetApplicationsByStudentId(int studentId)
    {
        try
        {
            var applications = await _httpClient.GetFromJsonAsync<List<MastersIAGraderApplication2254>>($"api/graderapplications/student/{studentId}");
            return applications ?? new List<MastersIAGraderApplication2254>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching applications by student ID");
            return new List<MastersIAGraderApplication2254>();
        }
    }
}
