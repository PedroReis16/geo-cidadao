using GeoCidadao.Models.Entities.AnalyticsServiceAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoCidadao.Database.Configurations.AnalyticsServiceAPI
{
    public class ProblemEventConfiguration : BaseConfiguration<ProblemEvent>
    {
        public override void Configure(EntityTypeBuilder<ProblemEvent> builder)
        {
            base.Configure(builder);

            builder
                .Property(p => p.PostId)
                .HasColumnName("post_id")
                .IsRequired();

            builder
                .Property(p => p.Title)
                .HasColumnName("title")
                .HasMaxLength(500)
                .IsRequired();

            builder
                .Property(p => p.Description)
                .HasColumnName("description")
                .HasMaxLength(5000)
                .IsRequired();

            builder
                .Property(p => p.Category)
                .HasColumnName("category")
                .IsRequired();

            builder
                .Property(p => p.Region)
                .HasColumnName("region")
                .HasMaxLength(200);

            builder
                .Property(p => p.City)
                .HasColumnName("city")
                .HasMaxLength(200);

            builder
                .Property(p => p.State)
                .HasColumnName("state")
                .HasMaxLength(200);

            builder
                .Property(p => p.Latitude)
                .HasColumnName("latitude");

            builder
                .Property(p => p.Longitude)
                .HasColumnName("longitude");

            builder
                .Property(p => p.EventTimestamp)
                .HasColumnName("event_timestamp")
                .IsRequired();

            builder
                .Property(p => p.LikesCount)
                .HasColumnName("likes_count")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(p => p.CommentsCount)
                .HasColumnName("comments_count")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(p => p.RelevanceScore)
                .HasColumnName("relevance_score")
                .HasDefaultValue(0.0)
                .IsRequired();

            // Indexes for common queries
            builder.HasIndex(p => p.PostId);
            builder.HasIndex(p => p.Category);
            builder.HasIndex(p => p.EventTimestamp);
            builder.HasIndex(p => p.City);
            builder.HasIndex(p => p.State);
            builder.HasIndex(p => p.RelevanceScore);
        }
    }
}
