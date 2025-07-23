using Domain.Dtos.Tasks;
using MediatR;

namespace Application.Queries.Tasks;

public record SearchArchivedRangeTaskItemsQuery(int Page, int PageSize, Guid BoardId, string SearchTerm) : IRequest<IEnumerable<ArchivedTaskDto>>;
