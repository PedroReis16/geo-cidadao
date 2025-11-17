using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoCidadao.Database.Configurations.GerenciamentoPostsAPI
{
    public class PostCommentConfiguration : BaseConfiguration<PostComment>
    {
        public override void Configure(EntityTypeBuilder<PostComment> builder)
        {
            base.Configure(builder);

            builder
                .Property(pc => pc.PostId)
                .HasColumnName("post_id")
                .IsRequired();

            builder
                .Property(pc => pc.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder
                .Property(pc => pc.Content)
                .HasColumnName("content")
                .HasMaxLength(500)
                .IsRequired();

            builder
                .HasOne(pc => pc.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(pc => pc.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(pc => pc.PostId);
            builder.HasIndex(pc => pc.UserId);
        }
    }
}
