using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;
using GeoCidadao.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoCidadao.Database.Configurations.GerenciamentoUsuariosAPI
{
    public class UserInterestsConfiguration : BaseConfiguration<UserInterests>
    {
        public override void Configure(EntityTypeBuilder<UserInterests> builder)
        {
            base.Configure(builder);

            _ = builder
                .Property(x => x.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            _ = builder
                .Property(x => x.Region)
                .HasColumnName("region")
                .HasMaxLength(100)
                .IsRequired(false);

            _ = builder
                .Property(x => x.City)
                .HasColumnName("city")
                .HasMaxLength(100)
                .IsRequired(false);

            _ = builder
                .Property(x => x.State)
                .HasColumnName("state")
                .HasMaxLength(50)
                .IsRequired(false);

            _ = builder
                .Property(x => x.Categories)
                .HasColumnName("categories")
                .IsRequired(false);

            builder.HasIndex(x => x.UserId).IsUnique();
        }
    }
}
