using Application.Abstractions.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace Infrastructure.Services.Storage;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureBlob:ConnectionString"];
        _containerName = configuration["AzureBlob:ContainerName"] ?? "taskpilot-files";
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);
        var blobClient = containerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);
        return blobClient.Uri.ToString();
    }

    public async Task<Stream?> GetFileAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(fileName);
        if (!await blobClient.ExistsAsync(cancellationToken))
            return null;
        var response = await blobClient.OpenReadAsync(cancellationToken: cancellationToken);
        return response;
    }

    public async Task DeleteFileAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(fileName);
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task<bool> FileExistsAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(fileName);
        return await blobClient.ExistsAsync(cancellationToken);
    }

    public async Task<IEnumerable<string>> ListFilesAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var results = new List<string>();
        await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: prefix, cancellationToken: cancellationToken))
        {
            results.Add(blobItem.Name);
        }
        return results;
    }

    public async Task<BlobFileMetadata?> GetFileMetadataAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(fileName);
        if (!await blobClient.ExistsAsync(cancellationToken))
            return null;
        var properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
        return new BlobFileMetadata
        {
            FileName = fileName,
            BlobName = fileName,
            Url = blobClient.Uri.ToString(),
            ContentType = properties.Value.ContentType,
            Size = properties.Value.ContentLength,
            UploadedAt = properties.Value.CreatedOn.UtcDateTime
        };
    }

    public async Task<IEnumerable<BlobFileMetadata>> ListFilesWithMetadataAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var results = new List<BlobFileMetadata>();
        await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: prefix, cancellationToken: cancellationToken))
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            var properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
            results.Add(new BlobFileMetadata
            {
                FileName = blobItem.Name.Substring(prefix.Length),
                BlobName = blobItem.Name,
                Url = blobClient.Uri.ToString(),
                ContentType = properties.Value.ContentType,
                Size = properties.Value.ContentLength,
                UploadedAt = properties.Value.CreatedOn.UtcDateTime
            });
        }
        return results;
    }
}
