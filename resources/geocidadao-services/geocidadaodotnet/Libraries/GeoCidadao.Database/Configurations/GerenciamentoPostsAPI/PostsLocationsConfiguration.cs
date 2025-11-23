using GeoCidadao.Database.Entities.GerenciamentoPostsAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoCidadao.Database.Configurations.GerenciamentoPostsAPI
{
    public class PostLocationsConfiguration : BaseConfiguration<PostLocation>
    {
        public override void Configure(EntityTypeBuilder<PostLocation> builder)
        {
            base.Configure(builder);

            builder.Property(l => l.Location)
                .HasColumnType("geometry (Point)")
                .HasColumnName("position")
                .IsRequired();

            builder.Property(l => l.Address)
                .HasMaxLength(250)
                .HasColumnName("address")
                .IsRequired();

            builder.Property(l => l.City)
                .HasMaxLength(100)
                .HasColumnName("city")
                .IsRequired();

            builder.Property(l => l.State)
                .HasMaxLength(100)
                .HasColumnName("state")
                .IsRequired();

            builder.Property(l => l.Country)
                .HasMaxLength(100)
                .HasColumnName("country")
                .IsRequired();

            builder.Property(l => l.Suburb)
                .HasMaxLength(100)
                .HasColumnName("suburb")
                .IsRequired();

            builder.HasOne(l => l.Post)
                .WithOne(p => p.Location);

            builder.HasIndex(l => l.Location);
            builder.HasIndex(l => l.City);
            builder.HasIndex(l => l.State);
            builder.HasIndex(l => l.Country);
            builder.HasIndex(l => l.Suburb);

        }
    }
}