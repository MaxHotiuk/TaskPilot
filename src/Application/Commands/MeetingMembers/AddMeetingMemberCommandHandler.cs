using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Entities;
using MediatR;

namespace Application.Commands.MeetingMembers;

public class AddMeetingMemberCommandHandler : BaseCommandHandler, IRequestHandler<AddMeetingMemberCommand>
{
    public AddMeetingMemberCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory) { }

    public async Task Handle(AddMeetingMemberCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var meeting = await unitOfWork.Meetings.GetByIdAsync(request.MeetingId, cancellationToken)
                ?? throw new NotFoundException($"Meeting with ID {request.MeetingId} was not found");

            var member = await unitOfWork.MeetingMembers.FirstOrDefaultAsync(
                m => m.MeetingId == request.MeetingId && m.UserId == request.UserId, cancellationToken);

            if (member != null)
            {
                throw new InvalidOperationException("User is already a member of the meeting.");
            }

            var newMember = new MeetingMember
            {
                MeetingId = request.MeetingId,
                UserId = request.UserId,
                Status = "Invited"
            };
            await unitOfWork.MeetingMembers.AddAsync(newMember, cancellationToken);
        }, cancellationToken);
    }
}
