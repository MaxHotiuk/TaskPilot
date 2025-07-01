using Domain.Entities;
using FluentAssertions;
using Persistence.Repositories;
using Persistence.Tests.Builders;
using Persistence.Tests.Infrastructure;

namespace Persistence.Tests.Repositories;

public class StateRepositoryTests : BaseRepositoryTest
{
    private readonly StateRepository _stateRepository;

    public StateRepositoryTests()
    {
        _stateRepository = new StateRepository(Context);
    }

    [Fact]
    public async Task GetStatesByBoardIdAsync_WithValidBoardId_ShouldReturnStatesInOrder()
    {
        // Arrange
        await SeedDatabaseAsync();
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        // Add additional states with different orders
        var additionalStates = new[]
        {
            StateBuilder.CreateForBoard(boardId)
                .WithId(4)
                .WithName("Review")
                .WithOrder(4)
                .Build(),
            StateBuilder.CreateForBoard(boardId)
                .WithId(5)
                .WithName("Testing")
                .WithOrder(2) // Insert in middle order
                .Build()
        };

        await Context.States.AddRangeAsync(additionalStates);
        await Context.SaveChangesAsync();

        // Act
        var result = await _stateRepository.GetStatesByBoardIdAsync(boardId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(5); // 3 seeded + 2 additional
        result.Should().OnlyContain(s => s.BoardId == boardId);
        result.Should().BeInAscendingOrder(s => s.Order);
        
        // Verify specific ordering
        var statesList = result.ToList();
        statesList[0].Name.Should().Be("To Do"); // Order 1
        statesList[1].Name.Should().Be("In Progress"); // Order 2
        statesList[2].Name.Should().Be("Testing"); // Order 2 (should come after In Progress due to insertion order)
        statesList[3].Name.Should().Be("Done"); // Order 3
        statesList[4].Name.Should().Be("Review"); // Order 4
    }

    [Fact]
    public async Task GetStatesByBoardIdAsync_WithNonExistentBoardId_ShouldReturnEmptyCollection()
    {
        // Arrange
        await SeedDatabaseAsync();
        var nonExistentBoardId = Guid.NewGuid();

        // Act
        var result = await _stateRepository.GetStatesByBoardIdAsync(nonExistentBoardId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetStatesByBoardIdAsync_WithEmptyBoard_ShouldReturnEmptyCollection()
    {
        // Arrange
        await SeedDatabaseAsync();
        var emptyBoardId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"); // Board 2 has no states

        // Act
        var result = await _stateRepository.GetStatesByBoardIdAsync(emptyBoardId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetStateByBoardAndNameAsync_WithValidBoardAndName_ShouldReturnState()
    {
        // Arrange
        await SeedDatabaseAsync();
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        const string stateName = "To Do";

        // Act
        var result = await _stateRepository.GetStateByBoardAndNameAsync(boardId, stateName);

        // Assert
        result.Should().NotBeNull();
        result!.BoardId.Should().Be(boardId);
        result.Name.Should().Be(stateName);
        result.Order.Should().Be(1);
    }

    [Fact]
    public async Task GetStateByBoardAndNameAsync_WithValidBoardInvalidName_ShouldReturnNull()
    {
        // Arrange
        await SeedDatabaseAsync();
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        const string nonExistentStateName = "Non-Existent State";

        // Act
        var result = await _stateRepository.GetStateByBoardAndNameAsync(boardId, nonExistentStateName);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetStateByBoardAndNameAsync_WithInvalidBoardValidName_ShouldReturnNull()
    {
        // Arrange
        await SeedDatabaseAsync();
        var nonExistentBoardId = Guid.NewGuid();
        const string stateName = "To Do";

        // Act
        var result = await _stateRepository.GetStateByBoardAndNameAsync(nonExistentBoardId, stateName);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("to do")] // Different case
    [InlineData("TO DO")] // All caps
    [InlineData("To Do")] // Exact match
    public async Task GetStateByBoardAndNameAsync_WithDifferentCasing_ShouldBeCaseSensitive(string searchName)
    {
        // Arrange
        await SeedDatabaseAsync();
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        // Act
        var result = await _stateRepository.GetStateByBoardAndNameAsync(boardId, searchName);

        // Assert
        if (searchName == "To Do")
        {
            result.Should().NotBeNull();
        }
        else
        {
            result.Should().BeNull(); // Current implementation is case-sensitive
        }
    }

    [Fact]
    public async Task IsValidStateForBoardAsync_WithValidStateAndBoard_ShouldReturnTrue()
    {
        // Arrange
        await SeedDatabaseAsync();
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        const int stateId = 1; // "To Do" state from seeded data

        // Act
        var result = await _stateRepository.IsValidStateForBoardAsync(stateId, boardId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsValidStateForBoardAsync_WithValidStateInvalidBoard_ShouldReturnFalse()
    {
        // Arrange
        await SeedDatabaseAsync();
        var invalidBoardId = Guid.NewGuid();
        const int stateId = 1; // Valid state but for different board

        // Act
        var result = await _stateRepository.IsValidStateForBoardAsync(stateId, invalidBoardId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsValidStateForBoardAsync_WithInvalidStateValidBoard_ShouldReturnFalse()
    {
        // Arrange
        await SeedDatabaseAsync();
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        const int invalidStateId = 999; // Non-existent state

        // Act
        var result = await _stateRepository.IsValidStateForBoardAsync(invalidStateId, boardId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsValidStateForBoardAsync_WithStateFromDifferentBoard_ShouldReturnFalse()
    {
        // Arrange
        await SeedDatabaseAsync();
        var boardId1 = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var boardId2 = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        // Add a state to board2
        var stateForBoard2 = StateBuilder.CreateForBoard(boardId2)
            .WithId(10)
            .WithName("Board 2 State")
            .WithOrder(1)
            .Build();

        await Context.States.AddAsync(stateForBoard2);
        await Context.SaveChangesAsync();

        // Act - Try to use board2's state with board1
        var result = await _stateRepository.IsValidStateForBoardAsync(10, boardId1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Repository_WithCancellationToken_ShouldRespectCancellation()
    {
        // Arrange
        await SeedDatabaseAsync();
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _stateRepository.GetStatesByBoardIdAsync(boardId, cts.Token));

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _stateRepository.GetStateByBoardAndNameAsync(boardId, "To Do", cts.Token));

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _stateRepository.IsValidStateForBoardAsync(1, boardId, cts.Token));
    }

    [Fact]
    public async Task Repository_ShouldInheritBaseRepositoryFunctionality()
    {
        // Arrange
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        await SeedDatabaseAsync();

        var state = StateBuilder.CreateForBoard(boardId)
            .WithId(100)
            .WithName("Inheritance Test State")
            .WithOrder(99)
            .Build();

        // Act - Test inherited AddAsync functionality
        await _stateRepository.AddAsync(state);
        await Context.SaveChangesAsync();

        // Assert
        var addedState = await _stateRepository.GetByIdAsync(state.Id);
        addedState.Should().NotBeNull();
        addedState!.Name.Should().Be("Inheritance Test State");

        // Act - Test specific StateRepository functionality
        var stateByBoardAndName = await _stateRepository.GetStateByBoardAndNameAsync(boardId, "Inheritance Test State");
        var isValidState = await _stateRepository.IsValidStateForBoardAsync(100, boardId);

        // Assert
        stateByBoardAndName.Should().NotBeNull();
        stateByBoardAndName!.Id.Should().Be(100);
        
        isValidState.Should().BeTrue();
    }

    [Fact]
    public async Task GetStatesByBoardIdAsync_WithStatesHavingSameOrder_ShouldMaintainConsistentOrdering()
    {
        // Arrange
        await SeedDatabaseAsync();
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        // Add states with same order to test consistency
        var statesWithSameOrder = new[]
        {
            StateBuilder.CreateForBoard(boardId)
                .WithId(10)
                .WithName("State A")
                .WithOrder(5)
                .Build(),
            StateBuilder.CreateForBoard(boardId)
                .WithId(11)
                .WithName("State B")
                .WithOrder(5)
                .Build(),
            StateBuilder.CreateForBoard(boardId)
                .WithId(12)
                .WithName("State C")
                .WithOrder(5)
                .Build()
        };

        await Context.States.AddRangeAsync(statesWithSameOrder);
        await Context.SaveChangesAsync();

        // Act - Call multiple times to ensure consistency
        var result1 = await _stateRepository.GetStatesByBoardIdAsync(boardId);
        var result2 = await _stateRepository.GetStatesByBoardIdAsync(boardId);

        // Assert
        result1.Should().BeInAscendingOrder(s => s.Order);
        result2.Should().BeInAscendingOrder(s => s.Order);
        
        // Results should be consistent between calls
        result1.Select(s => s.Id).Should().Equal(result2.Select(s => s.Id));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task GetStateByBoardAndNameAsync_WithEmptyOrNullName_ShouldReturnNull(string? stateName)
    {
        // Arrange
        await SeedDatabaseAsync();
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        // Act
        var result = await _stateRepository.GetStateByBoardAndNameAsync(boardId, stateName!);

        // Assert
        result.Should().BeNull();
    }
}
