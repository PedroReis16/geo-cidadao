using GeoCidadao.Models.Entities.EngagementServiceAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoCidadao.Database.Configurations.EngagementServiceAPI
{
    public class PostCommentsConfiguration : BaseConfiguration<PostComment>
    {
        public override void Configure(EntityTypeBuilder<PostComment> builder)
        {
            base.Configure(builder);

            builder
                .Property(pc => pc.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder
                .Property(pc => pc.PostId)
                .HasColumnName("post_id")
                .IsRequired();

            builder
                .Property(pc => pc.Content)
                .HasColumnName("content")
                .IsRequired();

            builder
             .HasMany(pc => pc.Likes)
             .WithOne(cl => cl.Comment);

            builder.HasIndex(pc => pc.UserId);
            builder.HasIndex(pc => pc.PostId);

        }
    }
}