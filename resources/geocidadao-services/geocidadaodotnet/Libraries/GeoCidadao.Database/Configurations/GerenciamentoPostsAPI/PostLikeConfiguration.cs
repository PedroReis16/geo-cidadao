using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoCidadao.Database.Configurations.GerenciamentoPostsAPI
{
    public class PostLikeConfiguration : BaseConfiguration<PostLike>
    {
        public override void Configure(EntityTypeBuilder<PostLike> builder)
        {
            base.Configure(builder);

            builder
                .Property(pl => pl.PostId)
                .HasColumnName("post_id")
                .IsRequired();

            builder
                .Property(pl => pl.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder
                .HasOne(pl => pl.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(pl => pl.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ensure one user can only like a post once
            builder.HasIndex(pl => new { pl.PostId, pl.UserId }).IsUnique();
            builder.HasIndex(pl => pl.UserId);
            builder.HasIndex(pl => pl.PostId);
        }
    }
}
