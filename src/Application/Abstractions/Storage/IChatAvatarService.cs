using Domain.Dtos.Avatars;

namespace Application.Abstractions.Storage;

public interface IChatAvatarService
{
    Task<ChatAvatarDto> UploadAsync(Guid chatId, Stream imageStream, string contentType, CancellationToken cancellationToken = default);
    Task<ChatAvatarDto?> GetAsync(Guid chatId, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid chatId, CancellationToken cancellationToken = default);
}
