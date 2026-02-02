# StartUp - Clean Architecture .NET Template

A production-ready Clean Architecture template built with **.NET 10**, featuring CQRS, resilient caching, background jobs, and comprehensive authentication.

## 🚀 Quick Start

### Option 1: Use as Template (Recommended)
```bash
# Install the template globally
dotnet new install CleanArchitecture.Template::1.0.0

# Create a new project
dotnet new cleanarch-api -n YourProjectName

# Navigate to your project
cd YourProjectName

# Run setup script for your OS
```

The setup script will:
- ✅ Rename all occurrences of "StartUp" to your project name
- ✅ Update namespaces, file names, and configurations
- ✅ Clean up and prepare your project

### Option 2: Clone and Customize
- Clone the repository
```bash
git clone https://github.com/yourusername/cleanarch-template.git YourProjectName
cd YourProjectName

# Run setup script for your OS
```
## 📋 Setup Instructions
- For Windows Users:
```powershell
# Run the setup script
.\setup.ps1

# Or with a custom project name
.\setup.ps1 "MyCustomProjectName"
```
- For Linux/Mac Users:
``` bash 
# Make the script executable
chmod +x setup.sh

# Run the setup script
./setup.sh

# Or with a custom project name
./setup.sh "MyCustomProjectName"
```
### 🎯 What the Setup Script Does
- ✅ Cleans up: Removes template-specific files and old Git history
- ✅ Renames: Updates all occurrences of template names to your project name
- ✅ Initializes Git: Creates a fresh repository with initial commit
- ✅ Sets up .NET: Restores packages and builds the solution
- ✅ Removes remotes: Ensures no connections to template repository

## ⚙️ Pre-requisites
1.  Database Setup
	- Edit `src/Web/appsettings.json` file
2.  JWT Configuration
	- Set a secure secret key in `appsettings.json` under `JwtOptions:Secret`
3. Environment Variables
	- Copy .env.example to .env and update values:
	```
	ASPNETCORE_ENVIRONMENT=Development
	CONNECTION_STRING=your-connection-string
	JWT_SECRET=your-jwt-secret
	```
## 🛠️ Development Commands
### build the database
```
# Create migration
dotnet ef migrations add InitialCreate --project src/YourProject.Infrastructure

# Update database
dotnet ef database update --project src/YourProject.Infrastructure

# Remove migration
dotnet ef migrations remove --project src/YourProject.Infrastructure
```

### Run
```bash
# Run normally
dotnet run --project src/YourProject.Web

# Run with hot reload
dotnet watch run --project src/YourProject.Web

# Run in development mode
ASPNETCORE_ENVIRONMENT=Development dotnet run
```

### Test
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/YourProject.UnitTests

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

Navigate to `https://localhost:7215/swagger`

## ✨ Features

### Architecture & Patterns
- ✅ **Clean Architecture** - Separation of concerns with clear layer boundaries
- ✅ **CQRS Pattern** - Command Query Responsibility Segregation with MediatR
- ✅ **Repository Pattern** - Clean data access abstraction
- ✅ **Domain-Driven Design** - Rich domain models with ULID identifiers

### Infrastructure
- ✅ **Redis Caching** - High-performance distributed caching with Polly circuit breaker for resilience
- ✅ **SQL Server** - Entity Framework Core with code-first migrations
- ✅ **Hangfire** - Background job processing and scheduling
- ✅ **JWT Authentication** - Secure token-based authentication with ASP.NET Core Identity

### Developer Experience
- ✅ **AutoMapper** - Object-to-object mapping
- ✅ **FluentValidation** - Elegant validation rules
- ✅ **Serilog** - Structured logging to console and file
- ✅ **Swagger/OpenAPI** - Interactive API documentation
- ✅ **Dynamic LINQ** - Runtime query building
- ✅ **Docker Support** - Containerization ready

### Reliability
- ✅ **Circuit Breaker Pattern** - Polly-based resilience for external services
- ✅ **Graceful Degradation** - App continues working when Redis is unavailable
- ✅ **Error Handling** - Comprehensive exception handling middleware


## 🛠️ Tech Stack

| Layer | Technologies |
|-------|-------------|
| **Framework** | .NET 10 |
| **API** | ASP.NET Core Web API, Swagger/OpenAPI |
| **Database** | SQL Server, Entity Framework Core 10 |
| **Caching** | Redis (StackExchange.Redis) with Polly Circuit Breaker |
| **Authentication** | ASP.NET Core Identity, JWT Bearer |
| **CQRS** | MediatR 12.5 |
| **Validation** | FluentValidation 12.1 |
| **Mapping** | AutoMapper 14.0 |
| **Logging** | Serilog (Console + File sinks) |
| **Background Jobs** | Hangfire 1.8 |
| **Resilience** | Polly 8.6 |
| **Identifiers** | ULID 1.4 |

## 🏗️ Architecture Highlights

### Clean Architecture Layers

1. **Domain Layer** (Inner-most)
   - No dependencies on other layers
   - Contains enterprise business rules
   - Entities use ULID for distributed-friendly IDs

2. **Application Layer**
   - Depends only on Domain
   - Contains application business rules
   - CQRS commands/queries with MediatR
   - FluentValidation for request validation

3. **Infrastructure Layer**
   - Implements interfaces defined in Application/Domain
   - Entity Framework Core data access
   - Redis caching with Polly resilience
   - Identity and authentication services

4. **Web Layer** (Outer-most)
   - ASP.NET Core API controllers
   - Dependency injection configuration
   - Middleware pipeline

### Key Patterns

**CQRS with MediatR**
```
Request → MediatR → Handler → Response
```

**Caching with Circuit Breaker**
```
Request → Try Cache (with timeout) → On Failure: Circuit Breaker Opens → Fallback to Database
```

**Repository Pattern**
```
Controller → Service → Repository → DbContext
```


## 🐳 Docker Support

```bash
# Build and run with Docker Compose
docker-compose up --build

# Run specific services
docker-compose up db redis api

# View logs
docker-compose logs -f api
```

## 📝 API Documentation

Once running, access interactive API documentation at:
- **Swagger UI**: `https://localhost:7215/swagger`
- **Hangfire Dashboard**: `https://localhost:7215/hangfire`

## 🔐 Authentication Flow

1. **Register**: `POST /api/auth/register`
2. **Confirm Email**: `GET /api/auth/confirm-email`
3. **Login**: `POST /api/auth/login` → Returns JWT token
4. **Use Token**: Add `Authorization: Bearer {token}` header to requests

## 🚦 Resilience Features

### Redis Circuit Breaker
- Opens after 2 consecutive failures
- Stays open for 30 seconds
- Automatically retries when half-open
- App continues working without cache when circuit is open

### Connection Timeouts
- Redis: 1 second connect timeout
- Operations: 500ms timeout with Polly


## Future Enhancements
- Support  DeploymentTo Azure App Service
- Support Kubernetes
- Implement GraphQL API alongside REST
- 

## 📚 Resources

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [Circuit Breaker Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/circuit-breaker)

## Support
- You can reach out for support via GitHub Issues on the repository page.
- Or contact me at: [email](mailto:mahmoudsayed1332002@gmail.com)


## 🤝 Contributing

This is a template repository. Feel free to fork and customize for your needs!

## 📄 License

MIT License - See [LICENSE.txt](LICENSE.txt) for details

---

**Built with ❤️ using Clean Architecture principles**