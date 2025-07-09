namespace Application.Common.Dtos.Storage;

public record BlobFileDto
{
    public string FileName { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long Size { get; init; }
}
