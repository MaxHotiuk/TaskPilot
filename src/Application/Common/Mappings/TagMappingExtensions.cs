using Domain.Dtos.Tags;
using Domain.Entities;

namespace Application.Common.Mappings;

public static class TagMappingExtensions
{
    public static IEnumerable<TagDto> ToDto(this IEnumerable<Tag> tags)
    {
        return tags.Select(tag => new TagDto
        {
            Id = tag.Id,
            BoardId = tag.BoardId,
            Name = tag.Name,
            Color = tag.Color,
            CreatedAt = tag.CreatedAt,
            UpdatedAt = tag.UpdatedAt
        });
    }
}
