using Domain.Dtos.Tags;
using MediatR;

namespace Application.Queries.Tags;

public record GetTagsByBoardIdQuery(Guid BoardId) : IRequest<IEnumerable<TagDto>>;
