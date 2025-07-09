using Application.Common.Dtos.Attachments;

namespace Application.Abstractions.Storage;

public interface IAttachmentService
{
    Task<AttachmentDto> UploadAsync(Guid entityId, Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<AttachmentDto?> GetAsync(string fileName, CancellationToken cancellationToken = default);
    Task DeleteAsync(string fileName, CancellationToken cancellationToken = default);
    Task<IEnumerable<AttachmentDto>> GetForEntityAsync(Guid entityId, CancellationToken cancellationToken = default);
}
