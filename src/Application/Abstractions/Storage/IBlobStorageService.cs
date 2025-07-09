namespace Application.Abstractions.Storage;

public interface IBlobStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<Stream?> GetFileAsync(string fileName, CancellationToken cancellationToken = default);
    Task DeleteFileAsync(string fileName, CancellationToken cancellationToken = default);
    Task<bool> FileExistsAsync(string fileName, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> ListFilesAsync(string prefix, CancellationToken cancellationToken = default);
    Task<BlobFileMetadata?> GetFileMetadataAsync(string fileName, CancellationToken cancellationToken = default);
    Task<IEnumerable<BlobFileMetadata>> ListFilesWithMetadataAsync(string prefix, CancellationToken cancellationToken = default);
}
