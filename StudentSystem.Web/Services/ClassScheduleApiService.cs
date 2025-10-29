using System.Net.Http.Json;
using StudentSystem.Domain.Entities;

namespace StudentSystem.Web.Services;

public class ClassScheduleApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ClassScheduleApiService> _logger;

    public ClassScheduleApiService(HttpClient httpClient, ILogger<ClassScheduleApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<ClassSchedule2254>> GetAllClassesAsync()
    {
        try
        {
            var classes = await _httpClient.GetFromJsonAsync<List<ClassSchedule2254>>("api/classschedule");
            return classes ?? new List<ClassSchedule2254>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching class schedules");
            return new List<ClassSchedule2254>();
        }
    }

    public async Task<ClassSchedule2254?> GetClassByNumAsync(string classNum)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ClassSchedule2254>($"api/classschedule/{classNum}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching class {ClassNum}", classNum);
            return null;
        }
    }

    public async Task<List<ClassSchedule2254>> GetClassesByTermAsync(string term)
    {
        try
        {
            var classes = await _httpClient.GetFromJsonAsync<List<ClassSchedule2254>>($"api/classschedule/term/{term}");
            return classes ?? new List<ClassSchedule2254>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching classes by term");
            return new List<ClassSchedule2254>();
        }
    }

    public async Task<List<ClassSchedule2254>> GetClassesBySubjectAsync(string subject)
    {
        try
        {
            var classes = await _httpClient.GetFromJsonAsync<List<ClassSchedule2254>>($"api/classschedule/subject/{subject}");
            return classes ?? new List<ClassSchedule2254>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching classes by subject");
            return new List<ClassSchedule2254>();
        }
    }

    public async Task<List<ClassSchedule2254>> GetClassesByInstructorAsync(int instructorId)
    {
        try
        {
            var classes = await _httpClient.GetFromJsonAsync<List<ClassSchedule2254>>($"api/classschedule/instructor/{instructorId}");
            return classes ?? new List<ClassSchedule2254>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching classes by instructor");
            return new List<ClassSchedule2254>();
        }
    }
}
