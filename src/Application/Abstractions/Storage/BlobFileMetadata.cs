namespace Application.Abstractions.Storage;

public class BlobFileMetadata
{
    public string FileName { get; set; } = string.Empty;
    public string? BlobName { get; set; }
    public string? Url { get; set; }
    public string? ContentType { get; set; }
    public long Size { get; set; }
    public DateTime UploadedAt { get; set; }
}
