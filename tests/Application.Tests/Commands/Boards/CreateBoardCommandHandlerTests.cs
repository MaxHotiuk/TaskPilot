using Application.Abstractions.Messaging;
using Application.Commands.Boards;
using Domain.Enums;

namespace Application.Tests.Commands.Boards;

public class CreateBoardCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IBoardRepository> _boardRepositoryMock;
    private readonly Mock<IOrganizationRepository> _organizationRepositoryMock;
    private readonly Mock<IOrganizationMemberRepository> _organizationMemberRepositoryMock;
    private readonly Mock<IChatRepository> _chatRepositoryMock;
    private readonly Mock<IChatMemberRepository> _chatMemberRepositoryMock;
    private readonly Mock<IBacklogRepository> _backlogRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactoryMock;
    private readonly Mock<IBoardNotifier> _boardNotifierMock;
    private readonly CreateBoardCommandHandler _handler;
    private readonly Mock<IAiSyncEnqueuer> _aiSyncEnqueuerMock;

    public CreateBoardCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _boardRepositoryMock = _fixture.Freeze<Mock<IBoardRepository>>();
        _organizationRepositoryMock = _fixture.Freeze<Mock<IOrganizationRepository>>();
        _organizationMemberRepositoryMock = _fixture.Freeze<Mock<IOrganizationMemberRepository>>();
        _chatRepositoryMock = _fixture.Freeze<Mock<IChatRepository>>();
        _chatMemberRepositoryMock = _fixture.Freeze<Mock<IChatMemberRepository>>();
        _backlogRepositoryMock = _fixture.Freeze<Mock<IBacklogRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _unitOfWorkFactoryMock = _fixture.Freeze<Mock<IUnitOfWorkFactory>>();

        _unitOfWorkMock.Setup(x => x.Boards).Returns(_boardRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.Organizations).Returns(_organizationRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.OrganizationMembers).Returns(_organizationMemberRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.Chats).Returns(_chatRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.ChatMembers).Returns(_chatMemberRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.Backlogs).Returns(_backlogRepositoryMock.Object);
        _unitOfWorkFactoryMock.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_unitOfWorkMock.Object);

        _organizationRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Organization { Id = Guid.NewGuid(), Name = "Test Org" });

        _organizationMemberRepositoryMock.Setup(x => x.IsMemberOfOrganizationAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _organizationMemberRepositoryMock.Setup(x => x.GetOrganizationMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OrganizationMember { Role = OrganizationMemberRole.Member });

        _boardNotifierMock = _fixture.Freeze<Mock<IBoardNotifier>>();
        _aiSyncEnqueuerMock = new Mock<IAiSyncEnqueuer>();
        _handler = new CreateBoardCommandHandler(_unitOfWorkFactoryMock.Object, _boardNotifierMock.Object, _organizationMemberRepositoryMock.Object, _aiSyncEnqueuerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateBoardAndReturnId()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var command = new CreateBoardCommand(
            Name: "Test Board",
            Description: "Test Description",
            OwnerId: Guid.NewGuid(),
            OrganizationId: organizationId
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        
        _boardRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Board>(b => 
                b.Name == command.Name &&
                b.Description == command.Description &&
                b.OwnerId == command.OwnerId &&
                b.Id != Guid.Empty
            ), It.IsAny<CancellationToken>()), 
            Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNullDescription_ShouldCreateBoardWithNullDescription()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var command = new CreateBoardCommand(
            Name: "Test Board",
            Description: null,
            OwnerId: Guid.NewGuid(),
            OrganizationId: organizationId
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        
        _boardRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Board>(b => 
                b.Name == command.Name &&
                b.Description == null &&
                b.OwnerId == command.OwnerId
            ), It.IsAny<CancellationToken>()), 
            Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldSetCorrectTimestamps()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var command = new CreateBoardCommand(
            Name: "Test Board",
            Description: "Test Description",
            OwnerId: Guid.NewGuid(),
            OrganizationId: organizationId
        );

        var beforeExecution = DateTime.UtcNow;

        // Act
        await _handler.Handle(command, CancellationToken.None);

        var afterExecution = DateTime.UtcNow;

        // Assert
        _boardRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Board>(b => 
                b.CreatedAt >= beforeExecution &&
                b.CreatedAt <= afterExecution &&
                b.UpdatedAt >= beforeExecution &&
                b.UpdatedAt <= afterExecution
            ), It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}
