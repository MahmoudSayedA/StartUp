# StartUp - Clean Architecture .NET Template

A production-ready Clean Architecture template built with .NET 10, featuring CQRS, resilient caching, background jobs, and comprehensive authentication.

## 🚀 Quick Start

### 1. Clone and Setup
```bash
# Clone the repository
git clone https://github.com/MahmoudSayedA/StartUp.git MyNewProject
cd MyNewProject

# Run setup (Windows)
.\setup.ps1

# OR setup (Linux/Mac)
chmod +x setup.sh
./setup.sh
```

The setup script will:
- ✅ Rename all occurrences of "StartUp" to your project name
- ✅ Update namespaces, file names, and configurations
- ✅ Clean up and prepare your project

### 2. Configure
Edit `src/Web/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=YourDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True",
    "Redis": "localhost:6379",
    "Hangfire": "Server=localhost;Database=YourDb_Hangfire;..."
  },
  "JwtOptions": {
    "Secret": "your-super-secret-key-minimum-32-characters-long",
    "ValidIssuer": "YourApp",
    "ValidAudience": "YourApp",
    "ExpiryMinutes": 60
  }
}
```

### 3. Run Migrations
```bash
cd src/Web
dotnet ef database update
```

### 4. Run
```bash
dotnet run
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

## 🔧 Configuration

### Required Services

1. **SQL Server** - Primary database
2. **Redis** (Optional) - Caching layer with automatic fallback
3. **Hangfire Database** - Background job storage

### Environment Variables

You can use environment variables instead of appsettings.json:

```bash
ConnectionStrings__DefaultConnection="Server=..."
ConnectionStrings__Redis="localhost:6379"
JwtOptions__Secret="your-secret-key"
```

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## 🐳 Docker Support

```bash
# Build and run with Docker Compose
docker-compose up -d

# Build image
docker build -t startup-api .
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

## 📚 Resources

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [Circuit Breaker Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/circuit-breaker)

## 🤝 Contributing

This is a template repository. Feel free to fork and customize for your needs!

## 📄 License

MIT License - See [LICENSE.txt](LICENSE.txt) for details

---

**Built with ❤️ using Clean Architecture principles**