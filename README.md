# HnH Mapper Server

A .NET 9 implementation of the Haven and Hearth Auto-Mapper Server with multi-tenancy support.

## Features

- Full backward compatibility with existing HnH game clients
- Multi-tenant architecture with complete data isolation
- Invitation-based registration with admin approval workflow
- Role-based permissions (SuperAdmin, TenantAdmin, TenantUser)
- Per-tenant storage quotas with real-time tracking
- Real-time updates via Server-Sent Events
- Custom map markers with CRUD API
- Audit logging for sensitive operations
- Blazor admin interface with MudBlazor

## Quick Start

Prerequisites: .NET 9 SDK

```bash
cd src/HnHMapperServer.AppHost
dotnet run
```

The Aspire dashboard opens automatically with links to the Web and API services.

Default credentials:
- Username: `admin`
- Password: `admin123!`

Change the password immediately after first login.

## Architecture

```
HnHMapperServer/
  src/
    HnHMapperServer.AppHost/         .NET Aspire orchestration
    HnHMapperServer.Core/            Domain entities and DTOs
    HnHMapperServer.Infrastructure/  EF Core data access
    HnHMapperServer.Services/        Business logic
    HnHMapperServer.Api/             Game client and admin APIs
    HnHMapperServer.Web/             Blazor UI
  deploy/                            Docker deployment configs
  map/                               Runtime data storage
```

| Component     | Technology                    |
|---------------|-------------------------------|
| Framework     | .NET 9                        |
| Web           | ASP.NET Core, Blazor Server   |
| UI            | MudBlazor                     |
| Database      | SQLite with EF Core           |
| Auth          | ASP.NET Core Identity         |
| Orchestration | .NET Aspire                   |
| Deployment    | Docker, GitHub Actions        |

## Documentation

- [CLAUDE.md](CLAUDE.md) - Technical documentation
- [deploy/](deploy/) - Deployment guides and Docker configuration
- [deploy/SECURITY.md](deploy/SECURITY.md) - Security configuration