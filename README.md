# StartUp – Clean Architecture Template for .NET 10

[![.NET](https://img.shields.io/badge/.NET-10.0-blue?logo=dotnet)](https://dotnet.microsoft.com)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Stars](https://img.shields.io/github/stars/MahmoudSayedA/StartUp?style=social)](https://github.com/MahmoudSayedA/StartUp)

**Production-ready Clean Architecture starter kit** built with modern .NET practices.  
Perfect for scalable APIs, microservices. Features CQRS, MediatR, EF Core, JWT, Redis caching with Polly resilience, Hangfire background jobs, Serilog structured logging, ULID IDs, FluentValidation, and Swagger.

----

### Why StartUp?
- Clear separation of concerns (Domain → Application → Infrastructure → Web)
- Ready for real-world use (auth, caching, background jobs, resilience)
- Minimal boilerplate – focus on business logic
- Extensible for Docker, CI/CD, or frontend (React/Angular)

-----

## 🚀 Quick Start

## option 1: Use NuGet Template (Recommended) -> soon, not published yet
```bash
# Install the template globally
dotnet new install MahmoudSayedA.StartUp.Template::10.0.0

# Create new project
dotnet new clean-arch-api -n MyAwesomeApp

cd MyAwesomeApp
dotnet run --project src/Web
```

Open:

- Swagger UI: https://localhost:7215/swagger
- Hangfire Dashboard: https://localhost:7215/hangfire 


### Option 2: Clone then create your instance
```bash
# Clone
git clone https://github.com/MahmoudSayedA/StartUp.git
cd StartUp

# Restore & run
dotnet restore
dotnet run --project src/Web

# create your instance // inside StartUp root sln folder
dotnet new install .

dotnet new cleanarch-api -n YourProjectName

cd YourProjectName

# run 
dotnet run --project src/Web
```
### option 3: Clone and use the setup script
```bash
git clone https://github.com/MahmoudSayedA/StartUp.git MyAwesomeApp
cd MyAwesomeApp

# Optional: Run setup script to rename project (if included)
.\setup.ps1 "MyAwesomeApp"    # Windows
# or
chmod +x setup.sh && ./setup.sh "MyAwesomeApp"   # Linux/Mac

# Restore & run
dotnet restore
dotnet run --project src/Web
```
-------

### 🐳 Docker Compose Support
```bash
# Build and start all services (API + SQL Server + Redis + Hangfire)
docker-compose up --build

# Or start in detached mode
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop everything
docker-compose down
```
Included services:

- web: The .NET Web API
- sqlserver: SQL Server
- redis: Redis for caching

Customize docker-compose.yml for your needs (e.g., ports, volumes).

---- 

## ⚙️ Pre-requisites
option 1:
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Redis](https://redis.io/download) (or use Docker)
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
----

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

--- 

## ✨ Features

### Architecture & Patterns
- Clean Architecture + DDD principles
- CQRS with MediatR
- ULID for unique, sortable IDs
- JWT Authentication + ASP.NET Core Identity
- Redis caching with Polly circuit breaker
- Hangfire for background & scheduled jobs
- Serilog structured logging (console + file)
- FluentValidation + AutoMapper
- Swagger/OpenAPI interactive docs
- Health checks endpoint
- Dynamic LINQ support
- Docker Compose ready
---

## Project Structure
```
StartUp/
├── src/
│   ├── Domain/               # Entities, Value Objects, Domain Events, ULID IDs
│   ├── Application/          # CQRS (Commands/Queries/Handlers), Services, Validators
│   ├── Infrastructure/       # EF Core, Repos, Identity, External Services
│   └── Web/                  # API Endpoints, Swagger, Health Checks, Program.cs
├── tests/                    # (Add your unit/integration tests here)
├── docker-compose.yml        # Docker setup
├── setup.ps1 & setup.sh      # Project renaming scripts (optional)
└── README.md
```

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
---

## 📝 API Documentation

Once running, access interactive API documentation at:
- **Swagger UI**: `https://localhost:7215/swagger`
- **Hangfire Dashboard**: `https://localhost:7215/hangfire`

---

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
Questions or feedback?
- 📧 mahmoudsayed1332002@gmail.com
- 🔗 LinkedIn: [mahmoudsayed13](https:linkedin.com/in/mahmoudsayed13)

**Built with ❤️ using Clean Architecture principles**
