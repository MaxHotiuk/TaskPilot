using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Enums;
using MediatR;

namespace Application.Commands.OrganizationMembers;

public record PromoteToManagerCommand(Guid OrganizationId, Guid UserId, Guid PromoterId) : IRequest<Unit>;

public class PromoteToManagerCommandHandler : BaseCommandHandler, IRequestHandler<PromoteToManagerCommand, Unit>
{
    private readonly IOrganizationMemberRepository _organizationMemberRepository;

    public PromoteToManagerCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IOrganizationMemberRepository organizationMemberRepository) 
        : base(unitOfWorkFactory)
    {
        _organizationMemberRepository = organizationMemberRepository;
    }

    public async Task<Unit> Handle(PromoteToManagerCommand request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            // Verify promoter is a manager
            var isPromoterManager = await _organizationMemberRepository.IsManagerOfOrganizationAsync(
                request.OrganizationId, 
                request.PromoterId, 
                cancellationToken);

            if (!isPromoterManager)
            {
                // Check if promoter is admin
                var promoter = await unitOfWork.Users.GetByIdAsync(request.PromoterId, cancellationToken);
                if (promoter == null || promoter.Role != Domain.Common.Authorization.Roles.Admin)
                    throw new ValidationException("Only managers or administrators can promote members to manager role");
            }

            // Get member to promote
            var member = await _organizationMemberRepository.GetOrganizationMemberAsync(
                request.OrganizationId, 
                request.UserId, 
                cancellationToken);

            if (member == null)
                throw new NotFoundException("Organization member not found");

            if (member.Role == OrganizationMemberRole.Guest)
                throw new ValidationException("Guest users cannot be promoted to manager role");

            if (member.Role == OrganizationMemberRole.Manager)
                throw new ValidationException("User is already a manager");

            // Promote to manager
            member.Role = OrganizationMemberRole.Manager;
            member.UpdatedAt = DateTime.UtcNow;

            return Unit.Value;
        }, cancellationToken);
    }
}
