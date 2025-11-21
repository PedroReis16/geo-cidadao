using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoCidadao.Database.Configurations.GerenciamentoUsuariosAPI
{
    public class UsersPictureConfiguration : BaseConfiguration<UserPicture>
    {
        public override void Configure(EntityTypeBuilder<UserPicture> builder)
        {
            base.Configure(builder);

            builder
                .Property(up => up.FileHash)
                .IsRequired();

            builder.Property(up => up.FileExtension)
                .IsRequired();


            builder.HasIndex(up => up.FileHash);
        }
    }
}