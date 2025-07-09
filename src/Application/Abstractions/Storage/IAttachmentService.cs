using Application.Common.Dtos.Attachments;

namespace Application.Abstractions.Storage;

public interface IAttachmentService
{
    Task<AttachmentDto> UploadAttachmentAsync(Guid entityId, Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<AttachmentDto?> GetAttachmentAsync(string fileName, CancellationToken cancellationToken = default);
    Task DeleteAttachmentAsync(string fileName, CancellationToken cancellationToken = default);
    Task<IEnumerable<AttachmentDto>> GetAttachmentsForEntityAsync(Guid entityId, CancellationToken cancellationToken = default);
}
