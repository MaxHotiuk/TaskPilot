using Application.Abstractions.Storage;
using Application.Common.Dtos.Avatars;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System.Diagnostics;

namespace Infrastructure.Services.Storage;

public class AvatarService : IAvatarService
{
    private const string AvatarOriginalFileNameTemplate = "avatars/{0}/original.png";
    private const string AvatarCompressedFileNameTemplate = "avatars/{0}/compressed.png";
    private const string DefaultAvatarContentType = "image/png";

    private readonly IBlobStorageService _blobStorageService;
    private readonly IConfiguration _configuration;
    private readonly int _avatarSize;

    public AvatarService(IBlobStorageService blobStorageService, IConfiguration configuration)
    {
        _blobStorageService = blobStorageService;
        _configuration = configuration;
        _avatarSize = int.TryParse(_configuration["Avatar:Size"], out var size) ? size : 256;
    }

    public async Task<AvatarDto> UploadAsync(Guid userId, Stream imageStream, string contentType, CancellationToken cancellationToken = default)
    {
        using var image = await Image.LoadAsync(imageStream, cancellationToken);
        if (image.Width != image.Height)
            throw new InvalidOperationException("Avatar image must be square.");

        image.Mutate(x => x.Resize(_avatarSize, _avatarSize));
        using var compressedStream = new MemoryStream();
        await image.SaveAsync(compressedStream, new PngEncoder { CompressionLevel = PngCompressionLevel.BestCompression }, cancellationToken);
        compressedStream.Position = 0;

        var originalFileName = string.Format(AvatarOriginalFileNameTemplate, userId);
        var compressedFileName = string.Format(AvatarCompressedFileNameTemplate, userId);

        imageStream.Position = 0;
        var originalUrl = await _blobStorageService.UploadFileAsync(imageStream, originalFileName, contentType, cancellationToken);
        var compressedUrl = await _blobStorageService.UploadFileAsync(compressedStream, compressedFileName, DefaultAvatarContentType, cancellationToken);

        return new AvatarDto
        {
            UserId = userId.ToString(),
            OriginalUrl = originalUrl,
            CompressedUrl = compressedUrl,
            Size = _avatarSize,
            ContentType = contentType,
            UploadedAt = DateTime.UtcNow
        };
    }

    public async Task<AvatarDto?> GetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var originalFileName = string.Format(AvatarOriginalFileNameTemplate, userId);
        var compressedFileName = string.Format(AvatarCompressedFileNameTemplate, userId);
        var original = await _blobStorageService.GetFileMetadataAsync(originalFileName, cancellationToken);
        var compressed = await _blobStorageService.GetFileMetadataAsync(compressedFileName, cancellationToken);
        if (original == null || compressed == null || compressed.Url == null || original.Url == null)
            return null;
        return new AvatarDto
        {
            UserId = userId.ToString(),
            OriginalUrl = original.Url,
            CompressedUrl = compressed.Url,
            Size = _avatarSize,
            ContentType = original.ContentType ?? DefaultAvatarContentType,
            UploadedAt = original.UploadedAt,
        };
    }

    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var originalFileName = string.Format(AvatarOriginalFileNameTemplate, userId);
        var compressedFileName = string.Format(AvatarCompressedFileNameTemplate, userId);
        await _blobStorageService.DeleteFileAsync(originalFileName, cancellationToken);
        await _blobStorageService.DeleteFileAsync(compressedFileName, cancellationToken);
    }
}
