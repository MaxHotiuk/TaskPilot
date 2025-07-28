using MediatR;
using Domain.Dtos.Meetings;

namespace Application.Commands.Meetings;

public record CreateMeetingCommand(CreateMeetingRequestDto Dto) : IRequest<string>;
