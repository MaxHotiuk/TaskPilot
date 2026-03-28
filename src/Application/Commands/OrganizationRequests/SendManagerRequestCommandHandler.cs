using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.OrganizationRequests;

public record SendManagerRequestCommand(string UserId, string OrganizationId, string Message) : ICommand<Unit>;

public class SendManagerRequestCommandHandler : BaseCommandHandler, IRequestHandler<SendManagerRequestCommand, Unit>
{
    private readonly IEmailService _emailService;
    private readonly IOrganizationManagerRequestRepository _requestRepository;
    private readonly IOrganizationMemberRepository _organizationMemberRepository;

    public SendManagerRequestCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IEmailService emailService,
        IOrganizationManagerRequestRepository requestRepository,
        IOrganizationMemberRepository organizationMemberRepository) 
        : base(unitOfWorkFactory)
    {
        _emailService = emailService;
        _requestRepository = requestRepository;
        _organizationMemberRepository = organizationMemberRepository;
    }

    public async Task<Unit> Handle(SendManagerRequestCommand request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var userId = Guid.Parse(request.UserId);
            var organizationId = Guid.Parse(request.OrganizationId);

            // Validate user exists
            var user = await unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                throw new NotFoundException("User not found");

            // Validate organization exists
            var organization = await unitOfWork.Organizations.GetByIdAsync(organizationId, cancellationToken);
            if (organization == null)
                throw new NotFoundException("Organization not found");

            // Check if user is member of organization
            var orgMember = await _organizationMemberRepository.GetOrganizationMemberAsync(organizationId, userId, cancellationToken);
            if (orgMember == null)
                throw new ValidationException("You must be a member of this organization to request manager role");

            // Guests cannot become managers
            if (orgMember.Role == OrganizationMemberRole.Guest)
                throw new ValidationException("Guest users cannot request manager role");

            // Check if organization has managers - if yes, only managers can promote
            var hasManagers = await _organizationMemberRepository.OrganizationHasManagersAsync(organizationId, cancellationToken);
            if (hasManagers)
            {
                throw new ValidationException("This organization already has managers. Please contact them to request manager role.");
            }

            // Check for recent or pending requests (5 day threshold)
            var hasRecentRequest = await _requestRepository.HasPendingOrRecentRequestAsync(userId, organizationId, 5, cancellationToken);
            if (hasRecentRequest)
            {
                throw new ValidationException("You already have a pending request or sent a request in the last 5 days for this organization");
            }

            // Create manager request
            var managerRequest = new OrganizationManagerRequest
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                OrganizationId = organizationId,
                Message = request.Message,
                Status = ManagerRequestStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _requestRepository.AddAsync(managerRequest, cancellationToken);

            // Send email notification to admin
            const string adminEmail = "dad.max.dad@gmail.com";

            var emailBody = $@"
                <p>A user has requested to become a manager for an organization.</p>
                <p><strong>User:</strong> {user.Username} ({user.Email})</p>
                <p><strong>Organization:</strong> {organization.Name}</p>
                <p><strong>Message:</strong></p>
                <p>{request.Message}</p>
                <p>Please review this request in the admin panel.</p>";

            await _emailService.SendSystemEmailAsync(
                adminEmail,
                $"Manager Request for {organization.Name}",
                emailBody,
                cancellationToken);

            return Unit.Value;
        }, cancellationToken);
    }
}
