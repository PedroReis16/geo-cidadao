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

            builder
                .HasMany(p => p.Medias)
                .WithOne(pm => pm.Post);

            builder
                .HasMany(p => p.Likes)
                .WithOne(pl => pl.Post);

            builder
                .HasMany(p => p.Comments)
                .WithOne(pc => pc.Post);


            builder.HasIndex(p => p.UserId);
            builder.HasIndex(p => p.Category);
            builder.HasIndex(p => p.RelevanceScore);
        }
    }
}