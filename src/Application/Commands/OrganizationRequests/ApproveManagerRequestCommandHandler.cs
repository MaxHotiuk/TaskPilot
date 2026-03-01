using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Enums;
using MediatR;

namespace Application.Commands.OrganizationRequests;

public record ApproveManagerRequestCommand(Guid RequestId, Guid ReviewerId) : ICommand<Unit>;

public class ApproveManagerRequestCommandHandler : BaseCommandHandler, IRequestHandler<ApproveManagerRequestCommand, Unit>
{
    private readonly IOrganizationManagerRequestRepository _requestRepository;
    private readonly IOrganizationMemberRepository _organizationMemberRepository;
    private readonly IEmailService _emailService;

    public ApproveManagerRequestCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IOrganizationManagerRequestRepository requestRepository,
        IOrganizationMemberRepository organizationMemberRepository,
        IEmailService emailService) 
        : base(unitOfWorkFactory)
    {
        _requestRepository = requestRepository;
        _organizationMemberRepository = organizationMemberRepository;
        _emailService = emailService;
    }

    public async Task<Unit> Handle(ApproveManagerRequestCommand request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var managerRequest = await _requestRepository.GetByIdAsync(request.RequestId, cancellationToken);
            if (managerRequest == null)
                throw new NotFoundException("Manager request not found");

            if (managerRequest.Status != ManagerRequestStatus.Pending)
                throw new ValidationException("This request has already been reviewed");

            // Verify reviewer is admin
            var reviewer = await unitOfWork.Users.GetByIdAsync(request.ReviewerId, cancellationToken);
            if (reviewer == null || reviewer.Role != Domain.Common.Authorization.Roles.Admin)
                throw new ValidationException("Only administrators can approve manager requests");

            // Update organization member role
            var orgMember = await _organizationMemberRepository.GetOrganizationMemberAsync(
                managerRequest.OrganizationId, 
                managerRequest.UserId, 
                cancellationToken);

            if (orgMember == null)
                throw new NotFoundException("Organization member not found");

            orgMember.Role = OrganizationMemberRole.Manager;
            orgMember.UpdatedAt = DateTime.UtcNow;

            // Update request status
            managerRequest.Status = ManagerRequestStatus.Approved;
            managerRequest.ReviewedBy = request.ReviewerId;
            managerRequest.ReviewedAt = DateTime.UtcNow;
            managerRequest.UpdatedAt = DateTime.UtcNow;

            // Send email notification to user
            var user = await unitOfWork.Users.GetByIdAsync(managerRequest.UserId, cancellationToken);
            var organization = await unitOfWork.Organizations.GetByIdAsync(managerRequest.OrganizationId, cancellationToken);

            if (user != null && organization != null)
            {
                var emailBody = $@"
                    <p>Congratulations! Your request to become a manager has been approved.</p>
                    <p><strong>Organization:</strong> {organization.Name}</p>
                    <p>You now have manager privileges for this organization.</p>";

                await _emailService.SendSystemEmailAsync(
                    user.Email,
                    $"Manager Request Approved - {organization.Name}",
                    emailBody,
                    cancellationToken);
            }

            return Unit.Value;
        }, cancellationToken);
    }
}
