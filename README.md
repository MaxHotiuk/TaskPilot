# TaskPilot

TaskPilot — Your Mission Control for Task Management

## Overview

TaskPilot is a robust task management system built with .NET 9 using Clean Architecture principles. It provides a comprehensive solution for organizing tasks across multiple boards with customizable states, team collaboration, and efficient task tracking.

## Architecture

The project follows Clean Architecture and CQRS (Command Query Responsibility Segregation) patterns, organized into the following layers:

### Domain Layer

Core business entities independent of any external frameworks:

- `Board`: Represents a project board containing tasks and states
- `TaskItem`: Individual tasks with titles, descriptions, assignees, and due dates
- `State`: Custom workflow states for tasks (e.g., To Do, In Progress, Done)
- `User`: System users who can own boards, be assigned tasks, and participate as board members
- `BoardMember`: Associates users with boards with specific roles
- `Comment`: User comments on tasks for collaboration

### Application Layer

Contains business logic and use cases:

- Implements CQRS with MediatR for handling commands and queries
- Commands: Create, update, and delete operations for all entities
- Queries: Data retrieval operations with DTOs for presentation
- Validation using FluentValidation
- Abstraction interfaces for persistence operations

### Infrastructure Layer

External concerns and cross-cutting implementations:

- Cross-cutting concerns
- External service integrations
- Infrastructure configuration

### Persistence Layer

Database access implementation details:

- Database context using Entity Framework Core
- Repository pattern implementations 
- Unit of work pattern for transaction management
- Database migrations using DbUp

### Web API Layer

RESTful API endpoints for client applications:

- Minimal API endpoint architecture
- Swagger/OpenAPI documentation
- Exception handling middleware
- CORS configuration
- Serilog for structured logging

## Getting Started

### Prerequisites

- .NET 9 SDK or later
- SQL Server (or compatible database)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/TaskPilot.git
   cd TaskPilot
   ```

2. Configure database connection and Azure AD:
   Create a `.env` file in the root directory:
   ```
   CONNECTION_STRING=Server=your_server;Database=TaskPilot;User Id=your_username;Password=your_password;TrustServerCertificate=True;
   AZURE_AD_INSTANCE=https://login.microsoftonline.com/
   AZURE_AD_TENANT_ID=your-tenant-id-here
   AZURE_AD_CLIENT_ID=your-client-id-here
   AZURE_AD_AUDIENCE=your-audience-here
   ```

3. Build the solution:
   ```bash
   dotnet build
   ```

4. Run the API:
   ```bash
   cd src/WebApi
   dotnet run
   ```

5. Access the Swagger UI:
   ```
   https://localhost:7291/swagger
   ```

## Features

- **Board Management**: Create and organize multiple task boards for different projects
- **Task Tracking**: Create, assign, and manage tasks with due dates and descriptions
- **Customizable Workflow**: Define custom states for each board
- **Team Collaboration**: Add team members to boards with specific roles
- **Comments**: Discuss tasks through a comment system
- **RESTful API**: Complete API for integration with frontend applications

## Testing

The project includes two types of tests:

### Unit Tests

Tests for the application logic without external dependencies:

```bash
dotnet test tests/Application.Tests
```

### Integration Tests

Tests for repositories with an in-memory database:

```bash
dotnet test tests/Persistence.Tests
```

## Project Structure

```
TaskPilot/
├── src/
│   ├── Application/           # Business logic and use cases
│   │   ├── Abstractions/      # Interfaces for external dependencies
│   │   ├── Commands/          # Write operations
│   │   ├── Common/            # Shared components and DTOs
│   │   ├── Queries/           # Read operations
│   │   └── Validators/        # Input validation
│   ├── Domain/                # Enterprise business rules
│   │   └── Entities/          # Domain entities
│   ├── Infrastructure/        # External frameworks and tools
│   ├── Persistence/           # Database access implementation
│   │   ├── Configurations/    # Entity configurations
│   │   ├── Migrations/        # Database migrations
│   │   └── Repositories/      # Data access implementations
│   └── WebApi/                # API endpoints and configuration
│       ├── Endpoints/         # API endpoints organized by entity
│       ├── Extensions/        # Extension methods
│       └── Middlewares/       # Custom middleware components
└── tests/
    ├── Application.Tests/     # Unit tests for application logic
    └── Persistence.Tests/     # Integration tests for repositories
```

## Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/my-feature`
3. Commit your changes: `git commit -m 'Add some feature'`
4. Push to the branch: `git push origin feature/my-feature`
5. Open a pull request
