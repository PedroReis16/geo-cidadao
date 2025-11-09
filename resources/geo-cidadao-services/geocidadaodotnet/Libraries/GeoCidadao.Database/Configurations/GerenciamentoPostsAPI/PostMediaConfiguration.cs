using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoCidadao.Database.Configurations.GerenciamentoPostsAPI
{
    public class PostMediaConfiguration : BaseConfiguration<PostMedia>
    {
        public override void Configure(EntityTypeBuilder<PostMedia> builder)
        {
            base.Configure(builder);

            builder
                .Property(pm => pm.MediaType)
                .HasColumnName("media_type")
                .IsRequired();

            builder
                .Property(pm => pm.Order)
                .HasColumnName("order")
                .IsRequired();

            builder
                .Property(pm => pm.FileSize)
                .HasColumnName("file_size")
                .IsRequired();

            builder
                .HasOne(pm => pm.Post)
                .WithMany(p => p.Medias);


            builder.HasIndex(pm => pm.MediaType);
            builder.HasIndex(pm => pm.Order);
        }
    }
}