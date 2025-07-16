using System;

namespace Application.Common.Dtos.Boards;

public record ArchivalJobDto
{
    public Guid Id { get; init; }
    public Guid BoardId { get; init; }
    public string JobType { get; init; } = string.Empty;
    public string? Metadata { get; init; }
    public string? BlobPath { get; init; }
    public string? ProcessedBy { get; init; }
    public string? ErrorMessage { get; init; }
    public int Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }
}
