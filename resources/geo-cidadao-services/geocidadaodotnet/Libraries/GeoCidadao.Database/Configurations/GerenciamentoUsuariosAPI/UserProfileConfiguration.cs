
using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoCidadao.Database.Configurations.GerenciamentoUsuariosAPI
{
    public class UserProfileConfiguration : BaseConfiguration<UserProfile>
    {
        public override void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            base.Configure(builder);

            _ = builder
                .Property(x => x.Username)
                .HasMaxLength(50)
                .HasColumnName("username")
                .IsRequired();

            _ = builder
                .Property(x => x.Email)
                .HasColumnName("email")
                .HasMaxLength(100)
                .IsRequired();

            _ = builder
                .Property(x => x.FirstName)
                .HasColumnName("first_name")
                .IsRequired();

            _ = builder
                .Property(x => x.LastName)
                .HasColumnName("last_name")
                .IsRequired();

            builder.HasOne(x => x.ProfilePicture)
                .WithOne(up => up.User)
                .HasForeignKey<UserPicture>(up => up.Id);

            builder.HasIndex(x => x.Username).IsUnique();
            builder.HasIndex(x => x.Email);
        }
    }
}