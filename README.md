# Enterprise Student System

A comprehensive ASP.NET Core 8.0 enterprise application showcasing modern .NET technologies, multiple data access patterns, real-time communications, Azure integration, and CI/CD practices.

## Features

### Technologies Implemented

- **ASP.NET Core 8.0 Web API** - Modern REST API with Swagger/OpenAPI
- **Entity Framework Core** - Primary ORM for database access
- **Dapper** - Lightweight micro-ORM alternative
- **ADO.NET** - Raw SQL data access implementation
- **SignalR** - Real-time bi-directional communication
- **Application Insights** - Azure monitoring and telemetry
- **Blazor Server** - Modern .NET web UI framework
- **xUnit** - Unit testing with Moq and FluentAssertions
- **LINQ** - Language Integrated Query throughout codebase
- **Health Checks** - Application health monitoring endpoints
- **Azure Services** - Ready for Azure deployment

### Architecture

```
StudentSystem/
├── StudentSystem.Domain/          # Domain entities and interfaces
│   ├── Entities/                  # Database models
│   └── Interfaces/                # Repository contracts
├── StudentSystem.Infrastructure/  # Data access implementations
│   ├── Data/                      # DbContext
│   ├── Repositories/
│   │   ├── EFCore/               # Entity Framework implementation
│   │   ├── Dapper/               # Dapper implementation
│   │   └── ADO/                  # ADO.NET implementation
├── StudentSystem.API/             # REST API
│   ├── Controllers/              # API endpoints
│   ├── Hubs/                     # SignalR hubs
│   └── Program.cs                # App configuration
├── StudentSystem.Web/             # Blazor Server frontend
├── StudentSystem.Shared/          # Shared DTOs
└── StudentSystem.Tests/           # xUnit tests
```

## Database Schema

The application connects to your existing `MyDatabase` SQL Server database with the following tables:

- **StudentData** - Student information and academic records
- **ClassSchedule2254** - Course schedules and sections
- **StudentClassAssignments** - Student-to-class assignments
- **MastersIAGraderApplication2254** - Grader application records

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code
- Azure account (optional, for cloud services)

### Configuration

1. **Update Connection String** in `StudentSystem.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=MyDatabase;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
  }
}
```

2. **Configure Application Insights** (optional):
   - Create an Application Insights resource in Azure
   - Update the connection string in `appsettings.json`

3. **Configure Azure SignalR** (optional for production):
   - Create an Azure SignalR Service
   - Uncomment and configure in `Program.cs`

### Running the Application

#### Option 1: Visual Studio
1. Open `StudentSystem.sln` in Visual Studio 2022
2. Set `StudentSystem.API` as the startup project
3. Press F5 to run

#### Option 2: Command Line
```bash
cd C:\Users\Troy\projects\EnterpriseStudentSystem\StudentSystem.API
dotnet run
```

The API will start at:
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:7001
- **Swagger UI**: https://localhost:7001 (root)

### Running Tests

```bash
cd C:\Users\Troy\projects\EnterpriseStudentSystem
dotnet test
```

## API Endpoints

### Students
- `GET /api/students` - Get all students
- `GET /api/students/{id}` - Get student by ID
- `GET /api/students/search?term={term}` - Search students
- `GET /api/students/department/{dept}` - Get students by department
- `PUT /api/students/{id}` - Update student

### Health Checks
- `GET /health` - Overall health status
- `GET /health/ready` - Readiness probe
- `GET /health/live` - Liveness probe

### SignalR Hub
- **Endpoint**: `/hubs/student`
- **Methods**:
  - `SendAssignmentUpdate(message)`
  - `SendStudentUpdate(studentId, updateType)`
  - `JoinClassGroup(classNum)`

## Data Access Patterns

The application demonstrates three different data access approaches:

### 1. Entity Framework Core (Default)
```csharp
// Configure in Program.cs
builder.Services.AddScoped<IStudentRepository, EFStudentRepository>();
```

Features:
- Change tracking
- LINQ queries
- Navigation properties
- Migrations support

### 2. Dapper
```csharp
// Configure in Program.cs
builder.Services.AddScoped<IStudentRepository, DapperStudentRepository>();
```

Features:
- Lightweight and fast
- SQL-first approach
- Micro-ORM capabilities
- Excellent for read-heavy operations

### 3. ADO.NET
```csharp
// Configure in Program.cs
builder.Services.AddScoped<IStudentRepository, AdoStudentRepository>();
```

Features:
- Maximum control
- Raw SQL commands
- SqlDataReader for performance
- Manual object mapping

## Azure Deployment

### Required Azure Resources

1. **App Service** - Host the API
2. **Azure SQL Database** - Production database
3. **Application Insights** - Monitoring and analytics
4. **Azure SignalR Service** - Scalable real-time messaging

### CI/CD Pipeline

The solution includes an Azure DevOps pipeline (`azure-pipelines.yml`):

**Pipeline Stages:**
1. **Build**
   - Restore NuGet packages
   - Build solution
   - Run xUnit tests with code coverage
   - Publish artifacts

2. **Deploy to Development**
   - Triggered on `develop` branch
   - Deploy to Dev App Service

3. **Deploy to Production**
   - Triggered on `main` branch
   - Deploy to Prod App Service
   - Configure app settings

### Setting Up Azure DevOps

1. Create a new pipeline in Azure DevOps
2. Connect to your repository
3. Use the existing `azure-pipelines.yml`
4. Configure pipeline variables:
   - `AzureSubscription`
   - `ApiAppServiceName`
   - `ResourceGroupName`
   - `AppInsightsConnectionString`
   - `SignalRConnectionString`

## Application Insights Integration

The application automatically tracks:
- **HTTP Requests** - All API calls
- **Dependencies** - SQL queries, external APIs
- **Exceptions** - Unhandled errors
- **Custom Events** - Business events
- **Custom Metrics** - Performance counters

### Custom Telemetry Example:
```csharp
_telemetry.TrackEvent("StudentCreated", new Dictionary<string, string>
{
    { "StudentId", student.Id.ToString() },
    { "Department", student.Department }
});
```

## Testing

### Test Coverage

- **Repository Tests** - Data access layer testing with InMemory DB
- **Controller Tests** - API endpoint testing with Moq
- **Integration Tests** - End-to-end scenarios (extend as needed)

### Running Specific Tests
```bash
# Run only repository tests
dotnet test --filter "StudentRepositoryTests"

# Run only controller tests
dotnet test --filter "StudentsControllerTests"
```

## Development Guidelines

### Adding a New Entity

1. Create entity in `StudentSystem.Domain/Entities`
2. Add repository interface in `StudentSystem.Domain/Interfaces`
3. Implement repository in `StudentSystem.Infrastructure/Repositories`
4. Add DbSet to `ApplicationDbContext`
5. Create controller in `StudentSystem.API/Controllers`
6. Write tests in `StudentSystem.Tests`

### Switching Data Access Implementations

In `StudentSystem.API/Program.cs`, change the repository registration:

```csharp
// Use EF Core
builder.Services.AddScoped<IStudentRepository, EFStudentRepository>();

// Use Dapper
builder.Services.AddScoped<IStudentRepository, DapperStudentRepository>();

// Use ADO.NET
builder.Services.AddScoped<IStudentRepository, AdoStudentRepository>();
```

## Monitoring and Diagnostics

### Local Development
- **Swagger UI**: https://localhost:7001
- **Health Checks**: https://localhost:7001/health

### Production (Azure)
- **Application Insights**: View in Azure Portal
- **Live Metrics**: Real-time performance
- **Failures**: Exception tracking
- **Performance**: Request duration analysis

## Security Considerations

- Connection strings should be stored in Azure Key Vault for production
- Enable authentication/authorization as needed
- Use HTTPS in production
- Configure CORS policies appropriately
- Implement rate limiting for public APIs

## Performance Optimization

- **Database Indexing**: Ensure proper indexes on StudentData table
- **Caching**: Add response caching for read-heavy endpoints
- **Connection Pooling**: Enabled by default in SQL Server
- **Azure SignalR**: Use for production scaling of SignalR connections

## Troubleshooting

### Common Issues

**Issue: Cannot connect to database**
- Verify SQL Server is running
- Check connection string in appsettings.json
- Ensure database `MyDatabase` exists

**Issue: SignalR hub not connecting**
- Check CORS policy includes SignalR origin
- Verify `/hubs/student` endpoint is accessible
- Check browser console for connection errors

**Issue: Tests failing**
- Ensure all packages are restored: `dotnet restore`
- Clean and rebuild: `dotnet clean && dotnet build`
- Check InMemory database setup in tests

## Contributing

1. Create a feature branch
2. Make changes
3. Write/update tests
4. Ensure all tests pass
5. Create pull request

## License

This project is created for educational and demonstration purposes.

## Support

For questions or issues:
- Check the Swagger documentation at the API root
- Review Application Insights for runtime errors
- Check Azure DevOps pipeline logs for deployment issues

---

**Built with:**
- .NET 8.0
- Entity Framework Core 8.0
- SignalR
- Application Insights
- Blazor Server
- xUnit, Moq, FluentAssertions

**Date Created:** 2025-10-28
