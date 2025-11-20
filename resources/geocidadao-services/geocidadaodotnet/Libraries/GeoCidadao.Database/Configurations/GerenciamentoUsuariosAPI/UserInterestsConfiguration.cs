using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoCidadao.Database.Configurations.GerenciamentoUsuariosAPI
{
    public class UserInterestsConfiguration : BaseConfiguration<UserInterests>
    {
        public override void Configure(EntityTypeBuilder<UserInterests> builder)
        {
            base.Configure(builder);

            builder
                .Property(ui=>ui.FollowedCategories)
                .HasColumnName("followed_categories");

            builder
                .Property(ui => ui.FollowedUsers)
                .HasColumnName("followed_users");
            builder
                .Property(ui => ui.FollowedCities)
                .HasColumnName("followed_cities");

            builder
                .Property(ui => ui.FollowedDistricts)
                .HasColumnName("followed_districts");
            builder
                .Property(ui => ui.InterestRange)
                .HasColumnName("interest_range");
        }
    }
}
