using Application.Common.Dtos.Avatars;

namespace Application.Abstractions.Storage;

public interface IAvatarService
{
    Task<AvatarDto> UploadAvatarAsync(Guid userId, Stream imageStream, string contentType, CancellationToken cancellationToken = default);
    Task<AvatarDto?> GetAvatarAsync(Guid userId, CancellationToken cancellationToken = default);
    Task DeleteAvatarAsync(Guid userId, CancellationToken cancellationToken = default);
}
