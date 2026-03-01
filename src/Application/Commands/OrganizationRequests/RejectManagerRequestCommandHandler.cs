using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Enums;
using MediatR;

namespace Application.Commands.OrganizationRequests;

public record RejectManagerRequestCommand(Guid RequestId, Guid ReviewerId, string? ReviewNotes) : ICommand<Unit>;

public class RejectManagerRequestCommandHandler : BaseCommandHandler, IRequestHandler<RejectManagerRequestCommand, Unit>
{
    private readonly IOrganizationManagerRequestRepository _requestRepository;
    private readonly IEmailService _emailService;

    public RejectManagerRequestCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IOrganizationManagerRequestRepository requestRepository,
        IEmailService emailService) 
        : base(unitOfWorkFactory)
    {
        _requestRepository = requestRepository;
        _emailService = emailService;
    }

    public async Task<Unit> Handle(RejectManagerRequestCommand request, CancellationToken cancellationToken)
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
                throw new ValidationException("Only administrators can reject manager requests");

            // Update request status
            managerRequest.Status = ManagerRequestStatus.Rejected;
            managerRequest.ReviewedBy = request.ReviewerId;
            managerRequest.ReviewedAt = DateTime.UtcNow;
            managerRequest.ReviewNotes = request.ReviewNotes;
            managerRequest.UpdatedAt = DateTime.UtcNow;

            // Send email notification to user
            var user = await unitOfWork.Users.GetByIdAsync(managerRequest.UserId, cancellationToken);
            var organization = await unitOfWork.Organizations.GetByIdAsync(managerRequest.OrganizationId, cancellationToken);

            if (user != null && organization != null)
            {
                var emailBody = $@"
                    <p>Your request to become a manager has been reviewed.</p>
                    <p><strong>Organization:</strong> {organization.Name}</p>
                    <p><strong>Status:</strong> Rejected</p>
                    {(!string.IsNullOrWhiteSpace(request.ReviewNotes) ? $"<p><strong>Notes:</strong> {request.ReviewNotes}</p>" : "")}
                    <p>You may submit a new request after 5 days.</p>";

                await _emailService.SendSystemEmailAsync(
                    user.Email,
                    $"Manager Request Update - {organization.Name}",
                    emailBody,
                    cancellationToken);
            }

            return Unit.Value;
        }, cancellationToken);
    }
}
