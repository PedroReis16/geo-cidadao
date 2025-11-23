using GeoCidadao.Models.Entities.EngagementServiceAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoCidadao.Database.Configurations.EngagementServiceAPI
{
    public class PostLikesConfiguration : BaseConfiguration<PostLike>
    {
        public override void Configure(EntityTypeBuilder<PostLike> builder)
        {
            base.Configure(builder);

            builder.HasKey(x => new { x.UserId, x.PostId });

            builder
                .Property(pl => pl.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.HasIndex(pl => pl.UserId);
            builder.HasIndex(pl => pl.PostId);
            builder.HasIndex(pl => new { pl.Id, pl.UserId }).IsUnique();
        }
    }
}