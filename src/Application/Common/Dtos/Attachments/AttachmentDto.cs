namespace Application.Common.Dtos.Attachments;

public record AttachmentDto
{
    public string FileName { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long Size { get; init; }
    public DateTime UploadedAt { get; init; }
}
