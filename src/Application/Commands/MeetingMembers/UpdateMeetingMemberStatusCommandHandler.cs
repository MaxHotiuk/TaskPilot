using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.MeetingMembers;

public class UpdateMeetingMemberStatusCommandHandler : BaseCommandHandler, IRequestHandler<UpdateMeetingMemberStatusCommand>
{
    public UpdateMeetingMemberStatusCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory) { }

    public async Task Handle(UpdateMeetingMemberStatusCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var member = await unitOfWork.MeetingMembers.FirstOrDefaultAsync(
                m => m.MeetingId == request.MeetingId && m.UserId == request.UserId, cancellationToken)
                ?? throw new NotFoundException($"Meeting member not found.");

            member.Status = request.Status;
            unitOfWork.MeetingMembers.Update(member);
        }, cancellationToken);
    }
}
