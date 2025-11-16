using Microsoft.EntityFrameworkCore;
using GeoCidadao.AnalyticsServiceAPI.Model.Entities;
using System.Text.Json;

namespace GeoCidadao.AnalyticsServiceAPI.Database
{
    public class AnalyticsDbContext : DbContext
    {
        public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : base(options)
        {
        }

        public DbSet<PostAnalytics> PostAnalytics { get; set; }
        public DbSet<RegionMetrics> RegionMetrics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PostAnalytics>(entity =>
            {
                entity.ToTable("post_analytics");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.PostId).IsRequired();
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Category).IsRequired();
                
                entity.HasIndex(e => e.PostId).IsUnique();
                entity.HasIndex(e => e.City);
                entity.HasIndex(e => e.State);
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.CreatedAt);
            });

            modelBuilder.Entity<RegionMetrics>(entity =>
            {
                entity.ToTable("region_metrics");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.RegionIdentifier).IsRequired();
                entity.Property(e => e.PostsByCategory)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                        v => JsonSerializer.Deserialize<Dictionary<GeoCidadao.Models.Enums.PostCategory, int>>(v, (JsonSerializerOptions)null!) ?? new Dictionary<GeoCidadao.Models.Enums.PostCategory, int>()
                    );
                
                entity.HasIndex(e => e.RegionIdentifier).IsUnique();
                entity.HasIndex(e => e.City);
                entity.HasIndex(e => e.State);
            });
        }
    }
}
