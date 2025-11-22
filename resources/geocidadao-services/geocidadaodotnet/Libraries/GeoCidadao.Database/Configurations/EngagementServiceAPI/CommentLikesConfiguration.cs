using GeoCidadao.Models.Entities.EngagementServiceAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoCidadao.Database.Configurations.EngagementServiceAPI
{
    public class CommentLikesConfiguration : BaseConfiguration<CommentLike>
    {
        public override void Configure(EntityTypeBuilder<CommentLike> builder)
        {
            base.Configure(builder);

            builder
                .Property(cl => cl.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder
                .HasOne(cl => cl.Comment)
                .WithMany(c => c.Likes);

            builder.HasIndex(cl => cl.UserId);
            builder.HasIndex(cl => new { cl.Id, cl.UserId }).IsUnique();
        }
    }
}