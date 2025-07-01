using Microsoft.EntityFrameworkCore;
using PWA_Lead_Capture_API.Models;

namespace PWA_Lead_Capture_API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<Lead> Leads { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Lead>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.EmailSent).IsRequired().HasDefaultValue(false);
            entity.Property(e => e.Source).HasMaxLength(500);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50).HasDefaultValue("New");
            
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.CreatedAt);
        });
    }
}
