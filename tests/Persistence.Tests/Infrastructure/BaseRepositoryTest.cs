using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Persistence;

namespace Persistence.Tests.Infrastructure;

public abstract class BaseRepositoryTest : IDisposable
{
    protected readonly ApplicationDbContext Context;
    private bool _disposed = false;

    protected BaseRepositoryTest()
    {
        // Use InMemory database for simpler setup
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        Context = new ApplicationDbContext(options);
    }

    protected async Task SeedDatabaseAsync()
    {
        // Seed users
        var users = new[]
        {
            new Domain.Entities.User 
            { 
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), 
                EntraId = "entra-user-1",
                Email = "user1@test.com", 
                Username = "user1",
                Role = "User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Domain.Entities.User 
            { 
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), 
                EntraId = "entra-user-2",
                Email = "user2@test.com", 
                Username = "user2",
                Role = "User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        Context.Users.AddRange(users);

        // Seed boards
        var boards = new[]
        {
            new Domain.Entities.Board 
            { 
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 
                Name = "Test Board 1", 
                Description = "Test board description",
                OwnerId = users[0].Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Domain.Entities.Board 
            { 
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 
                Name = "Test Board 2", 
                Description = "Another test board",
                OwnerId = users[1].Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        Context.Boards.AddRange(boards);

        // Seed states
        var states = new[]
        {
            new Domain.Entities.State { Id = 1, BoardId = boards[0].Id, Name = "To Do", Order = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Domain.Entities.State { Id = 2, BoardId = boards[0].Id, Name = "In Progress", Order = 2, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Domain.Entities.State { Id = 3, BoardId = boards[0].Id, Name = "Done", Order = 3, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        Context.States.AddRange(states);

        await Context.SaveChangesAsync();
    }

    protected async Task ClearDatabaseAsync()
    {
        Context.Comments.RemoveRange(Context.Comments);
        Context.Tasks.RemoveRange(Context.Tasks);
        Context.BoardMembers.RemoveRange(Context.BoardMembers);
        Context.Boards.RemoveRange(Context.Boards);
        Context.Users.RemoveRange(Context.Users);
        Context.States.RemoveRange(Context.States);
        await Context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            Context?.Dispose();
            _disposed = true;
        }
    }
}
