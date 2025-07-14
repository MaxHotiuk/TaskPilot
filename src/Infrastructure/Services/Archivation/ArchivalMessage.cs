using System;

namespace Infrastructure.Services.Archivation;

public class ArchivalMessage
{
    public Guid BoardId { get; set; }
    public Guid JobId { get; set; }
    public string JobType { get; set; } = "BoardArchival";
    public DateTime EnqueuedAt { get; set; } = DateTime.UtcNow;
}
