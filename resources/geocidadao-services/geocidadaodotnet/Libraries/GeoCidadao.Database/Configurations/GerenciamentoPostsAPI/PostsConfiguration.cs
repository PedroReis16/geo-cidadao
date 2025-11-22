using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoCidadao.Database.Configurations.GerenciamentoPostsAPI
{
    public class PostsConfiguration : BaseConfiguration<Post>
    {
        public override void Configure(EntityTypeBuilder<Post> builder)
        {
            base.Configure(builder);

            builder
                .Property(p => p.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder
                .Property(p => p.Content)
                .HasColumnName("content")
                .IsRequired();

            builder
                .Property(p => p.Category)
                .HasColumnName("category")
                .IsRequired();

            builder
                .HasMany(p => p.Medias)
                .WithOne(pm => pm.Post);

            builder.HasIndex(p => p.UserId);
            builder.HasIndex(p => p.Category);
        }
    }
}