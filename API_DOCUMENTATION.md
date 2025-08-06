# TaskPilot API Documentation

## Overview

TaskPilot is a comprehensive task management system built with .NET 9 using Clean Architecture and CQRS patterns. The API provides RESTful endpoints for managing boards, tasks, users, and collaborative features.

## Architecture

### Project Structure
- **Domain Layer**: Core business entities and authorization constants
- **Application Layer**: Business logic with CQRS pattern using MediatR
- **Infrastructure Layer**: External services, SignalR hubs, and background jobs
- **Persistence Layer**: Database access with Entity Framework Core and Repository pattern
- **WebApi Layer**: RESTful endpoints using Minimal API architecture
- **Database Layer**: Database migrations using DbUp

### Technology Stack
- **.NET 9**: Main framework
- **Entity Framework Core**: ORM for database operations
- **MediatR**: CQRS implementation
- **FluentValidation**: Input validation
- **Serilog**: Structured logging
- **SignalR**: Real-time communication
- **Hangfire**: Background job processing
- **Swagger/OpenAPI**: API documentation
- **Azure AD**: Authentication and authorization
- **xUnit**: Testing framework

## Authentication & Authorization

### Authentication
- **Provider**: Azure Active Directory (Azure AD)
- **Scheme**: JWT Bearer tokens
- **Implementation**: Microsoft Identity Web

### Authorization Policies
- `RequireAdminRole`: Administrator access
- `RequireUserRole`: Authenticated user access
- `RequireBoardMember`: Board member access
- `RequireBoardOwner`: Board owner access
- `RequireBoardMemberOrOwner`: Board member or owner access
- `RequireCommentOwner`: Comment owner access
- `RequireSelfUpdate`: User can only update their own information

### User Roles
- `Admin`: System administrator
- `User`: Regular authenticated user

### Board Member Roles
- `Owner`: Board owner with full permissions
- `Admin`: Board administrator
- `Member`: Regular board member

## Data Models

### Core Entities

#### User
```csharp
{
    "id": "guid",
    "entraId": "string",
    "username": "string",
    "email": "string",
    "role": "string", // Admin, User
    "createdAt": "datetime",
    "updatedAt": "datetime"
}
```

#### Board
```csharp
{
    "id": "guid",
    "name": "string",
    "description": "string?",
    "ownerId": "guid",
    "isArchived": "boolean",
    "archivedAt": "datetime?",
    "archivalReason": "string?",
    "createdAt": "datetime",
    "updatedAt": "datetime"
}
```

#### TaskItem
```csharp
{
    "id": "guid",
    "boardId": "guid",
    "title": "string",
    "description": "string?",
    "stateId": "int",
    "assigneeId": "guid?",
    "dueDate": "datetime?",
    "tagId": "int?",
    "priority": "int", // Default: 2
    "isArchived": "boolean",
    "createdAt": "datetime",
    "updatedAt": "datetime"
}
```

#### State
```csharp
{
    "id": "int",
    "name": "string",
    "color": "string",
    "boardId": "guid",
    "position": "int"
}
```

#### BoardMember
```csharp
{
    "id": "int",
    "boardId": "guid",
    "userId": "guid",
    "role": "string", // Owner, Admin, Member
    "joinedAt": "datetime"
}
```

#### Comment
```csharp
{
    "id": "int",
    "taskItemId": "guid",
    "authorId": "guid",
    "content": "string",
    "createdAt": "datetime",
    "updatedAt": "datetime"
}
```

#### Tag
```csharp
{
    "id": "int",
    "name": "string",
    "color": "string",
    "boardId": "guid"
}
```

#### Notification
```csharp
{
    "id": "guid",
    "userId": "guid",
    "title": "string",
    "content": "string",
    "isRead": "boolean",
    "type": "string",
    "relatedEntityId": "string?",
    "createdAt": "datetime"
}
```

#### Meeting
```csharp
{
    "id": "guid",
    "title": "string",
    "description": "string?",
    "startTime": "datetime",
    "endTime": "datetime",
    "location": "string?",
    "isOnline": "boolean",
    "meetingUrl": "string?",
    "organizerId": "guid",
    "boardId": "guid?",
    "createdAt": "datetime",
    "updatedAt": "datetime"
}
```

## API Endpoints

### Base URL
- Development: `https://localhost:7291`
- Swagger UI: `https://localhost:7291/swagger`

### Board Management

#### GET /api/boards
- **Description**: Get all boards
- **Authorization**: RequireUserRole
- **Response**: Array of Board objects

#### GET /api/boards/{id}
- **Description**: Get board by ID
- **Authorization**: RequireBoardMemberOrOwner
- **Parameters**: 
  - `id` (path, guid): Board ID
- **Response**: Board object

#### POST /api/boards
- **Description**: Create a new board
- **Authorization**: RequireUserRole
- **Request Body**:
```json
{
    "name": "string",
    "description": "string?",
    "ownerId": "guid"
}
```
- **Response**: Created board ID (guid)

#### PUT /api/boards/{id}
- **Description**: Update board
- **Authorization**: RequireBoardOwner
- **Parameters**: 
  - `id` (path, guid): Board ID
- **Request Body**: Board update data
- **Response**: No content (204)

#### DELETE /api/boards/{id}
- **Description**: Delete board
- **Authorization**: RequireBoardOwner
- **Parameters**: 
  - `id` (path, guid): Board ID
- **Response**: No content (204)

#### GET /api/boards/user/{userId}
- **Description**: Get boards by user ID
- **Authorization**: RequireUserRole
- **Parameters**: 
  - `userId` (path, guid): User ID
- **Response**: Array of Board objects

#### GET /api/boards/archived/owner/{ownerId}
- **Description**: Get archived boards by owner
- **Authorization**: RequireUserRole
- **Parameters**: 
  - `ownerId` (path, guid): Owner ID
- **Response**: Array of archived Board objects

#### POST /api/boards/{id}/archive
- **Description**: Archive board
- **Authorization**: RequireBoardOwner
- **Parameters**: 
  - `id` (path, guid): Board ID
- **Response**: No content (204)

#### POST /api/boards/{id}/dearchive
- **Description**: Dearchive board
- **Authorization**: RequireBoardOwner
- **Parameters**: 
  - `id` (path, guid): Board ID
- **Response**: No content (204)

### Task Management

#### GET /api/tasks
- **Description**: Get all tasks
- **Authorization**: RequireUserRole
- **Response**: Array of TaskItem objects

#### GET /api/tasks/{id}
- **Description**: Get task by ID
- **Authorization**: RequireBoardMemberOrOwner
- **Parameters**: 
  - `id` (path, guid): Task ID
- **Response**: TaskItem object

#### POST /api/tasks
- **Description**: Create a new task
- **Authorization**: RequireBoardMemberOrOwner
- **Request Body**:
```json
{
    "boardId": "guid",
    "title": "string",
    "description": "string?",
    "stateId": "int",
    "tagId": "int?",
    "priority": "int",
    "assigneeId": "guid?",
    "dueDate": "datetime?"
}
```
- **Response**: Created task ID (guid)

#### PUT /api/tasks/{id}
- **Description**: Update task
- **Authorization**: RequireBoardMemberOrOwner
- **Parameters**: 
  - `id` (path, guid): Task ID
- **Request Body**: TaskItem update data
- **Response**: No content (204)

#### DELETE /api/tasks/{id}
- **Description**: Delete task
- **Authorization**: RequireBoardMemberOrOwner
- **Parameters**: 
  - `id` (path, guid): Task ID
- **Response**: No content (204)

#### GET /api/tasks/board/{boardId}
- **Description**: Get tasks by board ID
- **Authorization**: RequireBoardMemberOrOwner
- **Parameters**: 
  - `boardId` (path, guid): Board ID
- **Response**: Array of TaskItem objects

#### GET /api/tasks/calendar
- **Description**: Get tasks for calendar view
- **Authorization**: RequireUserRole
- **Response**: Array of calendar-formatted task objects

#### POST /api/tasks/{id}/archive
- **Description**: Archive task
- **Authorization**: RequireBoardMemberOrOwner
- **Parameters**: 
  - `id` (path, guid): Task ID
- **Response**: No content (204)

#### POST /api/tasks/{id}/restore
- **Description**: Restore archived task
- **Authorization**: RequireBoardMemberOrOwner
- **Parameters**: 
  - `id` (path, guid): Task ID
- **Response**: No content (204)

### User Management

#### GET /api/users
- **Description**: Get all users
- **Authorization**: RequireUserRole
- **Response**: Array of User objects

#### GET /api/users/current
- **Description**: Get current authenticated user
- **Authorization**: RequireUserRole
- **Response**: User object

#### POST /api/users
- **Description**: Create a new user
- **Authorization**: RequireUserRole
- **Request Body**: User creation data
- **Response**: Created user ID (guid)

### Board Members

#### GET /api/board-members/board/{boardId}
- **Description**: Get board members by board ID
- **Authorization**: RequireBoardMemberOrOwner
- **Parameters**: 
  - `boardId` (path, guid): Board ID
- **Response**: Array of BoardMember objects

#### POST /api/board-members
- **Description**: Add member to board
- **Authorization**: RequireBoardOwner
- **Request Body**: BoardMember data
- **Response**: Created member ID

#### PUT /api/board-members/{id}
- **Description**: Update board member
- **Authorization**: RequireBoardOwner
- **Parameters**: 
  - `id` (path, int): Member ID
- **Request Body**: BoardMember update data
- **Response**: No content (204)

#### DELETE /api/board-members/{id}
- **Description**: Remove member from board
- **Authorization**: RequireBoardOwner
- **Parameters**: 
  - `id` (path, int): Member ID
- **Response**: No content (204)

### States

#### GET /api/states/board/{boardId}
- **Description**: Get states by board ID
- **Authorization**: RequireBoardMemberOrOwner
- **Parameters**: 
  - `boardId` (path, guid): Board ID
- **Response**: Array of State objects

#### POST /api/states
- **Description**: Create a new state
- **Authorization**: RequireBoardOwner
- **Request Body**: State creation data
- **Response**: Created state ID

#### PUT /api/states/{id}
- **Description**: Update state
- **Authorization**: RequireBoardOwner
- **Parameters**: 
  - `id` (path, int): State ID
- **Request Body**: State update data
- **Response**: No content (204)

#### DELETE /api/states/{id}
- **Description**: Delete state
- **Authorization**: RequireBoardOwner
- **Parameters**: 
  - `id` (path, int): State ID
- **Response**: No content (204)

### Tags

#### GET /api/tags/board/{boardId}
- **Description**: Get tags by board ID
- **Authorization**: RequireBoardMemberOrOwner
- **Parameters**: 
  - `boardId` (path, guid): Board ID
- **Response**: Array of Tag objects

#### POST /api/tags
- **Description**: Create a new tag
- **Authorization**: RequireBoardOwner
- **Request Body**: Tag creation data
- **Response**: Created tag ID

#### PUT /api/tags/{id}
- **Description**: Update tag
- **Authorization**: RequireBoardOwner
- **Parameters**: 
  - `id` (path, int): Tag ID
- **Request Body**: Tag update data
- **Response**: No content (204)

#### DELETE /api/tags/{id}
- **Description**: Delete tag
- **Authorization**: RequireBoardOwner
- **Parameters**: 
  - `id` (path, int): Tag ID
- **Response**: No content (204)

### Comments

#### GET /api/comments/task/{taskId}
- **Description**: Get comments by task ID
- **Authorization**: RequireBoardMemberOrOwner
- **Parameters**: 
  - `taskId` (path, guid): Task ID
- **Response**: Array of Comment objects

#### POST /api/comments
- **Description**: Create a new comment
- **Authorization**: RequireBoardMemberOrOwner
- **Request Body**: Comment creation data
- **Response**: Created comment ID

#### PUT /api/comments/{id}
- **Description**: Update comment
- **Authorization**: RequireCommentOwner
- **Parameters**: 
  - `id` (path, int): Comment ID
- **Request Body**: Comment update data
- **Response**: No content (204)

#### DELETE /api/comments/{id}
- **Description**: Delete comment
- **Authorization**: RequireCommentOwner
- **Parameters**: 
  - `id` (path, int): Comment ID
- **Response**: No content (204)

### Notifications

#### GET /api/notifications/user/{userId}
- **Description**: Get notifications by user ID
- **Authorization**: RequireUserRole
- **Parameters**: 
  - `userId` (path, guid): User ID
- **Response**: Array of Notification objects

#### GET /api/notifications/user/{userId}/unread-count
- **Description**: Get unread notifications count
- **Authorization**: RequireUserRole
- **Parameters**: 
  - `userId` (path, guid): User ID
- **Response**: Integer count

#### POST /api/notifications/user/{userId}/mark-all-read
- **Description**: Mark all notifications as read
- **Authorization**: RequireUserRole
- **Parameters**: 
  - `userId` (path, guid): User ID
- **Response**: No content (204)

### Meetings

#### GET /api/meetings/user/{userId}
- **Description**: Get meetings by user ID
- **Authorization**: RequireUserRole
- **Parameters**: 
  - `userId` (path, guid): User ID
- **Response**: Array of Meeting objects

#### GET /api/meetings/calendar-items
- **Description**: Get meeting calendar items
- **Authorization**: RequireUserRole
- **Response**: Array of calendar-formatted meeting objects

#### POST /api/meetings
- **Description**: Create a new meeting
- **Authorization**: RequireUserRole
- **Request Body**: Meeting creation data
- **Response**: Created meeting ID

#### PUT /api/meetings/{id}
- **Description**: Update meeting
- **Authorization**: RequireUserRole
- **Parameters**: 
  - `id` (path, guid): Meeting ID
- **Request Body**: Meeting update data
- **Response**: No content (204)

#### DELETE /api/meetings/{id}
- **Description**: Delete meeting
- **Authorization**: RequireUserRole
- **Parameters**: 
  - `id` (path, guid): Meeting ID
- **Response**: No content (204)

### Meeting Members

#### POST /api/meeting-members
- **Description**: Add member to meeting
- **Authorization**: RequireUserRole
- **Request Body**: MeetingMember data
- **Response**: Created member ID

#### PUT /api/meeting-members/{id}/status
- **Description**: Update meeting member status
- **Authorization**: RequireUserRole
- **Parameters**: 
  - `id` (path, int): Member ID
- **Request Body**: Status update data
- **Response**: No content (204)

### Backlog

#### GET /api/backlog/board/{boardId}
- **Description**: Get backlog items by board ID
- **Authorization**: RequireBoardMemberOrOwner
- **Parameters**: 
  - `boardId` (path, guid): Board ID
- **Response**: Array of Backlog objects

### Attachments

#### POST /api/attachments/upload
- **Description**: Upload file attachment
- **Authorization**: RequireUserRole
- **Request Body**: Multipart form data with file
- **Response**: Attachment metadata

### Avatars

#### GET /api/avatars/{userId}
- **Description**: Get user avatar
- **Authorization**: RequireUserRole
- **Parameters**: 
  - `userId` (path, guid): User ID
- **Response**: Avatar image

#### POST /api/avatars/upload
- **Description**: Upload user avatar
- **Authorization**: RequireUserRole
- **Request Body**: Multipart form data with image
- **Response**: Avatar metadata

### Chat

Chat functionality is likely implemented through SignalR hubs for real-time communication.

## Real-time Communication (SignalR)

### Hubs

#### BoardHub (`/hubs/board`)
- Real-time updates for board changes
- Task updates and notifications
- Member activity tracking

#### NotificationHub (`/hubs/notification`)
- Real-time notification delivery
- User presence status
- System announcements

#### WebRtcHub (`/webrtc`)
- Video/audio call functionality
- Screen sharing capabilities
- WebRTC signaling

## Background Jobs

### Archival System
- **ArchivalBackgroundJob**: Handles automatic archival of old tasks/boards
- **ArchivalJobScheduler**: Schedules recurring archival operations
- **Hangfire Dashboard**: Available at `/hangfire` for job monitoring

### Job Types
- Automatic task archival based on age
- Board cleanup operations
- Notification cleanup
- Data maintenance tasks

## Configuration

### Required Environment Variables
```
CONNECTION_STRING=Server=...;Database=TaskPilot;...
AZURE_AD_INSTANCE=https://login.microsoftonline.com/
AZURE_AD_TENANT_ID=your-tenant-id
AZURE_AD_CLIENT_ID=your-client-id
AZURE_AD_AUDIENCE=your-audience
```

### Additional Configuration (from Bicep template)
```
serviceBusConnectionString=...
cosmosDbConnectionString=...
azureBlobConnectionString=...
azureOpenAIEndpoint=...
azureOpenAIApiKey=...
azureSearchEndpoint=...
azureSearchApiKey=...
```

### CORS Configuration
- Configured for frontend applications
- Allows credentials
- Configurable allowed origins

## Logging

### Serilog Configuration
- **Console**: Development logging
- **File**: Structured JSON logs in `logs/` directory
- **Format**: Compact JSON with machine name and thread ID
- **Levels**: Information (default), Warning (Microsoft/System)

## Error Handling

### Global Exception Middleware
- Centralized error handling
- Structured error responses
- Problem details format (RFC 7807)
- Automatic error logging

### HTTP Status Codes
- `200 OK`: Successful GET requests
- `201 Created`: Successful POST requests
- `204 No Content`: Successful PUT/DELETE requests
- `400 Bad Request`: Validation errors
- `401 Unauthorized`: Authentication required
- `403 Forbidden`: Insufficient permissions
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server errors

## Testing

### Unit Tests (`Application.Tests`)
- **Framework**: xUnit
- **Mocking**: Moq
- **Test Data**: AutoFixture
- **Assertions**: FluentAssertions
- **Coverage**: Application layer business logic

### Integration Tests (`Persistence.Tests`)
- **Framework**: xUnit
- **Database**: In-memory Entity Framework
- **Scope**: Repository layer and database operations
- **Test Builders**: Custom builders for entity creation

### Test Commands
```bash
# Run unit tests
dotnet test tests/Application.Tests

# Run integration tests
dotnet test tests/Persistence.Tests

# Run all tests
dotnet test
```

## Development Setup

### Prerequisites
- .NET 9 SDK
- SQL Server or compatible database
- Azure AD tenant (for authentication)

### Local Development
1. Clone repository
2. Configure `.env` file with connection strings
3. Run database migrations: `dotnet run --project src/WebApi`
4. Access API: `https://localhost:7291`
5. Access Swagger UI: `https://localhost:7291/swagger`

### Docker Support
- **Azurite**: Local Azure Blob Storage emulator
- **Container**: `taskpilotst-blob-storage`
- **Port**: 10000

## Deployment

### Azure Infrastructure (Bicep)
- **App Service Plan**: Hosting web applications
- **Web API**: TaskPilot API service
- **Blazor App**: TaskPilot UI application
- **Function App**: Background archival service
- **SQL Database**: Primary data storage
- **Application Insights**: Monitoring and diagnostics
- **Log Analytics**: Centralized logging

### External Dependencies
- **Service Bus**: Message queuing
- **Cosmos DB**: Document storage
- **Azure Blob Storage**: File storage
- **Azure OpenAI**: AI capabilities
- **Azure Search**: Search functionality

## Security Considerations

### Authentication
- JWT token validation
- Azure AD integration
- Token expiration handling

### Authorization
- Role-based access control
- Resource-based permissions
- Policy-driven authorization

### Data Protection
- HTTPS enforcement
- Secure connection strings
- Environment-based configuration

## API Versioning
- Current version: v1
- Swagger endpoint: `/swagger/v1/swagger.json`
- Future versions can be added with versioning strategy

## Rate Limiting
No explicit rate limiting implementation found in the current codebase.

## Monitoring & Observability

### Application Insights
- Performance monitoring
- Error tracking
- Usage analytics
- Custom telemetry

### Structured Logging
- Request/response logging
- Business event logging
- Error tracking
- Performance metrics

---

*This documentation is based on the actual codebase analysis and reflects the current implementation as of the examination date.*
