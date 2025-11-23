using GeoCidadao.Database.Entities.GerenciamentoPostsAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoCidadao.Database.Configurations.GerenciamentoPostsAPI
{
    public class PostLocationConfiguration : BaseConfiguration<PostLocation>
    {
        public override void Configure(EntityTypeBuilder<PostLocation> builder)
        {
            base.Configure(builder);

            builder.Property(pl => pl.PostId)
                .HasColumnName("post_id")
                .IsRequired();

            builder.Property(pl => pl.Position)
                .HasColumnName("position")
                .IsRequired();

            builder.Property(pl => pl.Category)
                .HasColumnName("category")
                .IsRequired();

            builder.HasIndex(pl => pl.Position);
            builder.HasIndex(pl => pl.Category);
            builder.HasIndex(pl => pl.PostId);
        }
    }
}