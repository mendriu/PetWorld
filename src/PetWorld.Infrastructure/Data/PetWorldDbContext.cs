using Microsoft.EntityFrameworkCore;
using PetWorld.Domain.Entities;

namespace PetWorld.Infrastructure.Data;

public class PetWorldDbContext : DbContext
{
    public PetWorldDbContext(DbContextOptions<PetWorldDbContext> options) : base(options)
    {
    }

    public DbSet<ChatHistory> ChatHistories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ChatHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Question).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Answer).IsRequired().HasColumnType("TEXT");
            entity.Property(e => e.CreatedAt).IsRequired();
        });
    }
}
