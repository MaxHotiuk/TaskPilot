using Application.Common.Dtos.States;
using Domain.Entities;

namespace Application.Common.Mappings;

public static class StateMappingExtensions
{
    public static StateDto ToDto(this State state)
    {
        return new StateDto
        {
            Id = state.Id,
            BoardId = state.BoardId,
            Name = state.Name,
            Order = state.Order,
            CreatedAt = state.CreatedAt,
            UpdatedAt = state.UpdatedAt
        };
    }

    public static IEnumerable<StateDto> ToDto(this IEnumerable<State> states)
    {
        return states.Select(ToDto);
    }
}
