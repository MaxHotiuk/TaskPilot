# UserProfile Commands and Queries Implementation

This document summarizes the implementation of CQRS pattern for UserProfile functionality, following the existing patterns in the TaskPilot application.

## Files Created

### Domain Transfer Objects (DTOs)
- `src/Domain/Dtos/UserProfiles/UserProfileDto.cs` - Data transfer object for UserProfile entity

### Commands
- `src/Application/Commands/UserProfile/CreateUserProfileCommand.cs` - Command to create a new user profile
- `src/Application/Commands/UserProfile/CreateUserProfileCommandHandler.cs` - Handler for create command
- `src/Application/Commands/UserProfile/UpdateUserProfileCommand.cs` - Command to update an existing user profile
- `src/Application/Commands/UserProfile/UpdateUserProfileCommandHandler.cs` - Handler for update command
- `src/Application/Commands/UserProfile/DeleteUserProfileCommand.cs` - Command to delete a user profile
- `src/Application/Commands/UserProfile/DeleteUserProfileCommandHandler.cs` - Handler for delete command

### Queries
- `src/Application/Queries/UserProfile/GetUserProfileByIdQuery.cs` - Query to get user profile by ID
- `src/Application/Queries/UserProfile/GetUserProfileByIdQueryHandler.cs` - Handler for get by ID query
- `src/Application/Queries/UserProfile/GetUserProfileByUserIdQuery.cs` - Query to get user profile by User ID
- `src/Application/Queries/UserProfile/GetUserProfileByUserIdQueryHandler.cs` - Handler for get by User ID query
- `src/Application/Queries/UserProfile/GetAllUserProfilesQuery.cs` - Query to get all user profiles
- `src/Application/Queries/UserProfile/GetAllUserProfilesQueryHandler.cs` - Handler for get all query
- `src/Application/Queries/UserProfile/GetUserProfilesByDepartmentQuery.cs` - Query to get user profiles by department
- `src/Application/Queries/UserProfile/GetUserProfilesByDepartmentQueryHandler.cs` - Handler for get by department query
- `src/Application/Queries/UserProfile/GetUserProfilesByLocationQuery.cs` - Query to get user profiles by location
- `src/Application/Queries/UserProfile/GetUserProfilesByLocationQueryHandler.cs` - Handler for get by location query

### Validators
#### Command Validators
- `src/Application/Validators/Commands/UserProfile/CreateUserProfileCommandValidator.cs` - Validator for create command
- `src/Application/Validators/Commands/UserProfile/UpdateUserProfileCommandValidator.cs` - Validator for update command
- `src/Application/Validators/Commands/UserProfile/DeleteUserProfileCommandValidator.cs` - Validator for delete command

#### Query Validators
- `src/Application/Validators/Queries/UserProfile/GetUserProfileByIdQueryValidator.cs` - Validator for get by ID query
- `src/Application/Validators/Queries/UserProfile/GetUserProfileByUserIdQueryValidator.cs` - Validator for get by User ID query
- `src/Application/Validators/Queries/UserProfile/GetUserProfilesByDepartmentQueryValidator.cs` - Validator for get by department query
- `src/Application/Validators/Queries/UserProfile/GetUserProfilesByLocationQueryValidator.cs` - Validator for get by location query

### Mappings
- `src/Application/Common/Mappings/UserProfileMappingExtensions.cs` - Extension methods to map between UserProfile entity and UserProfileDto

## Files Updated

### Repository Interface
- `src/Application/Abstractions/Persistence/IUserProfileRepository.cs` - Enhanced with custom methods:
  - `GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)`
  - `ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken)`
  - `GetByDepartmentAsync(string department, CancellationToken cancellationToken)`
  - `GetByLocationAsync(string location, CancellationToken cancellationToken)`

### Repository Implementation
- `src/Persistence/Repositories/UserProfileRepository.cs` - Implemented the custom methods defined in the interface

## Key Features Implemented

### Commands
1. **CreateUserProfileCommand**: Creates a new user profile for a user
   - Validates that the user exists
   - Ensures no duplicate profiles per user
   - Supports all UserProfile properties

2. **UpdateUserProfileCommand**: Updates an existing user profile
   - Uses partial update pattern (nullable parameters)
   - Only updates provided fields
   - Automatically updates `UpdatedAt` timestamp

3. **DeleteUserProfileCommand**: Deletes a user profile by ID
   - Validates profile exists before deletion
   - Uses unit of work for transaction management

### Queries
1. **GetUserProfileByIdQuery**: Retrieves user profile by profile ID
2. **GetUserProfileByUserIdQuery**: Retrieves user profile by associated user ID
3. **GetAllUserProfilesQuery**: Retrieves all user profiles
4. **GetUserProfilesByDepartmentQuery**: Filters user profiles by department
5. **GetUserProfilesByLocationQuery**: Filters user profiles by location

### Unit of Work Pattern
All commands and queries use the `IUnitOfWorkFactory` for proper transaction management:
- Commands use `ExecuteInTransactionAsync` with automatic commit/rollback
- Queries use `ExecuteQueryAsync` for read operations
- Follows the same pattern as existing User, Board, and other entity handlers

### Validation
- All commands and queries have corresponding FluentValidation validators
- Validation includes:
  - Required field validation
  - String length limits matching database constraints
  - Conditional validation for optional fields

### Repository Enhancements
Extended `IUserProfileRepository` with domain-specific methods for better performance:
- Direct user ID lookups avoid complex LINQ expressions
- Department and location filtering for common use cases
- Existence checks for validation scenarios

## Usage Examples

### Creating a User Profile
```csharp
var command = new CreateUserProfileCommand(
    UserId: userId,
    Bio: "Software Developer with 5 years experience",
    JobTitle: "Senior Developer",
    Department: "Engineering",
    Location: "Remote",
    PhoneNumber: "+1234567890",
    AddToBoardAutomatically: true,
    ShowEmail: true,
    ShowPhoneNumber: false
);

var profileId = await mediator.Send(command);
```

### Updating a User Profile
```csharp
var command = new UpdateUserProfileCommand(
    Id: profileId,
    Bio: "Updated bio",
    JobTitle: "Lead Developer"
    // Only specified fields will be updated
);

await mediator.Send(command);
```

### Querying User Profiles
```csharp
// Get by user ID
var query = new GetUserProfileByUserIdQuery(userId);
var profile = await mediator.Send(query);

// Get by department
var departmentQuery = new GetUserProfilesByDepartmentQuery("Engineering");
var engineeringProfiles = await mediator.Send(departmentQuery);
```

## Notes
- All handlers follow the existing BaseCommandHandler and BaseQueryHandler patterns
- Error handling uses the same exceptions as other parts of the application
- Mapping extensions provide clean conversion between entities and DTOs
- The implementation is consistent with existing patterns for Users, Boards, Tasks, etc.
