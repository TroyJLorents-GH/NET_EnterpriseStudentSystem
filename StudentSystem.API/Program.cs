using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using StudentSystem.Domain.Interfaces;
using StudentSystem.Infrastructure.Data;
using StudentSystem.Infrastructure.Repositories.EFCore;
using StudentSystem.API.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add Application Insights
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});

// Add controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Configure DbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)
    ));

// Register repositories (using EF Core implementation by default)
// You can switch to Dapper or ADO.NET implementations by changing the registration
builder.Services.AddScoped<IStudentRepository, EFStudentRepository>();
builder.Services.AddScoped<IClassScheduleRepository, EFClassScheduleRepository>();
builder.Services.AddScoped<IGraderApplicationRepository, EFGraderApplicationRepository>();
builder.Services.AddScoped<IAssignmentRepository, EFAssignmentRepository>();
// Alternative: builder.Services.AddScoped<IStudentRepository, DapperStudentRepository>();
// Alternative: builder.Services.AddScoped<IStudentRepository, AdoStudentRepository>();

// Add SignalR
builder.Services.AddSignalR();

// Add Azure SignalR Service (optional - for production scaling)
// builder.Services.AddSignalR().AddAzureSignalR(builder.Configuration["Azure:SignalR:ConnectionString"]);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:5000",
                "https://localhost:5000",
                "https://localhost:7001"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") ?? "",
        name: "sqlserver",
        tags: new[] { "db", "sql", "sqlserver" });

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Student System API",
        Version = "v1",
        Description = "Enterprise Student Management System API with EF Core, Dapper, ADO.NET, SignalR, and Application Insights",
        Contact = new OpenApiContact
        {
            Name = "Student System Team"
        }
    });
});

// Add logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddApplicationInsights();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student System API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at app's root
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Map SignalR Hub
app.MapHub<StudentHub>("/hubs/student");

// Map Health Checks
app.MapHealthChecks("/health");

app.Logger.LogInformation("Student System API starting up...");
app.Logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);

app.Run();

// Make Program class accessible to tests
public partial class Program { }
