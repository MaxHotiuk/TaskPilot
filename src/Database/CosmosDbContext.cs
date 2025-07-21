using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class CosmosDbContext : DbContext
{
    public CosmosDbContext(DbContextOptions<CosmosDbContext> options) : base(options)
    {
    }

    public DbSet<ArchivalJob> ArchivalJobs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ArchivalJob>(entity =>
        {
            entity.ToContainer("ArchivalJobs");
            entity.HasKey(e => e.Id);
            entity.HasPartitionKey(e => e.BoardId);
            entity.Property(e => e.Id).ToJsonProperty("id");
            entity.Ignore(e => e.Board);
        });
    }
}
