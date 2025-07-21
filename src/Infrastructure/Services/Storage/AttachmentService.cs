using Application.Abstractions.Storage;
using Domain.Dtos.Attachments;
using System.Diagnostics;

namespace Infrastructure.Services.Storage;

public class AttachmentService : IAttachmentService
{
    private const string AttachmentBlobNameTemplate = "attachments/{0}/{1}";
    private const string AttachmentPrefixTemplate = "attachments/{0}/";
    private const string AttachmentApiUrlTemplate = "/api/attachments/{0}";
    private const string DefaultContentType = "application/octet-stream";

    private readonly IBlobStorageService _blobStorageService;

    public AttachmentService(IBlobStorageService blobStorageService)
    {
        _blobStorageService = blobStorageService;
    }

    public async Task<AttachmentDto> UploadAsync(Guid entityId, Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var blobName = string.Format(AttachmentBlobNameTemplate, entityId, fileName);
        var url = await _blobStorageService.UploadFileAsync(fileStream, blobName, contentType, cancellationToken);
        return new AttachmentDto
        {
            FileName = fileName,
            Url = url,
            ContentType = contentType,
            Size = fileStream.Length,
            UploadedAt = DateTime.UtcNow
        };
    }

    public async Task<AttachmentDto?> GetAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var metadata = await _blobStorageService.GetFileMetadataAsync(fileName, cancellationToken);
        if (metadata == null) return null;
        return new AttachmentDto
        {
            FileName = fileName,
            Url = metadata.Url ?? string.Format(AttachmentApiUrlTemplate, fileName),
            ContentType = metadata.ContentType ?? DefaultContentType,
            Size = metadata.Size,
            UploadedAt = metadata.UploadedAt
        };
    }

    public async Task DeleteAsync(string fileName, CancellationToken cancellationToken = default)
    {
        await _blobStorageService.DeleteFileAsync(fileName, cancellationToken);
    }

    public async Task<IEnumerable<AttachmentDto>> GetForEntityAsync(Guid entityId, CancellationToken cancellationToken = default)
    {
        var prefix = string.Format(AttachmentPrefixTemplate, entityId);
        var blobs = await _blobStorageService.ListFilesWithMetadataAsync(prefix, cancellationToken);
        var attachments = new List<AttachmentDto>();
        foreach (var blob in blobs)
        {
            attachments.Add(new AttachmentDto
            {
                FileName = blob.FileName,
                Url = blob.Url ?? string.Format(AttachmentApiUrlTemplate, blob.BlobName),
                ContentType = blob.ContentType ?? DefaultContentType,
                Size = blob.Size,
                UploadedAt = blob.UploadedAt
            });
        }
        return attachments;
    }
}
