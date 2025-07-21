using Domain.Dtos.Avatars;

namespace Application.Abstractions.Storage;

public interface IAvatarService
{
    Task<AvatarDto> UploadAsync(Guid userId, Stream imageStream, string contentType, CancellationToken cancellationToken = default);
    Task<AvatarDto?> GetAsync(Guid userId, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default);
}
